using OpenTK.Graphics.OpenGL.Compatibility;

namespace Manifold.Core.Renderer.MaterialSystem;

public enum RenderTargetFormat {
    RGBA8,
    RGBA16F,
    RGBA32F,
    Depth24F,
    Depth32F,
    Depth32Stencil8,
}

public struct RenderTargetAttachmentSpec {
    public RenderTargetFormat Format;
}

public struct RenderTargetSpec {
    public int Width;
    public int Height;
    public bool HasDepth;

    public RenderTargetAttachmentSpec[] ColorAttachments;
}

public class RenderTarget : IDisposable {
    private int _fbo;
    private int[] _colorAttachments = new int[4];
    private int _depthAttachment;

    private RenderTargetSpec _spec;
    
    public int Width => _spec.Width;
    public int Height => _spec.Height;
    
    public int GetColorTexture(int index = 0) => _colorAttachments[index];
    public int GetDepthTexture() => _depthAttachment;

    public RenderTarget(RenderTargetSpec spec) {
        _spec = spec;
        Invalidate();
    }

    private void Invalidate() {
        if (_fbo != 0) {
            DisposeInternal();
        }
        _fbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

        _colorAttachments = new int[_spec.ColorAttachments.Length];

        for (int i = 0; i < _colorAttachments.Length; i++) {
            _colorAttachments[i] = CreateColorAttachment(_spec.ColorAttachments[i]);

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                (FramebufferAttachment)((int)FramebufferAttachment.ColorAttachment0 + i),
                TextureTarget.Texture2d,
                _colorAttachments[i],
                0);
        }


        if (_spec.HasDepth) {
            _depthAttachment = CreateDepthAttachment();

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2d,
                _depthAttachment,
                0);
        }
        
        var drawBuffers = new DrawBufferMode[_colorAttachments.Length];
        for (int i = 0; i < drawBuffers.Length; i++) {
            drawBuffers[i] = (DrawBufferMode)((int)DrawBufferMode.ColorAttachment0 + i);
        }
        
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete) {
            throw new Exception("Framebuffer not complete");
        }
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    private int CreateColorAttachment(RenderTargetAttachmentSpec spec) {
        int tex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, tex);

        (InternalFormat internalFmt, PixelFormat fmt, PixelType type) =
            spec.Format switch {
                RenderTargetFormat.RGBA8 => (InternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte),
                RenderTargetFormat.RGBA16F => (InternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.Float),
                RenderTargetFormat.RGBA32F => (InternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float),
                _ => throw new NotSupportedException()
            };

        GL.TexImage2D(TextureTarget.Texture2d, 0, internalFmt, _spec.Width, _spec.Height, 0, fmt, type, IntPtr.Zero);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return tex;
    }

    private int CreateDepthAttachment() {
        int tex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, tex);
        
        GL.TexImage2D(TextureTarget.Texture2d, 0,
            InternalFormat.DepthComponent32f, _spec.Width, _spec.Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return tex;
    }

    public void Bind(bool clear = true, bool clearDepth = true) {
  
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        GL.Viewport(0, 0, _spec.Width, _spec.Height);
        if (clear && clearDepth) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        else if (clear) {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
        else if (clearDepth) {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }
    }

    public void Unbind() {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void Resize(int width, int height) {
        if (width == 0 || height == 0) {
            return;
        }
        
        _spec.Width = width;
        _spec.Height = height;
        Invalidate();
    }
    
    private void DisposeInternal() {
        foreach (var tex in _colorAttachments) {
            if (tex != 0) {
                GL.DeleteTexture(tex);
            }
        }

        if (_depthAttachment != 0) {
            GL.DeleteTexture(_depthAttachment);
            _depthAttachment = 0;
        }

        if (_fbo != 0) {
            GL.DeleteFramebuffer(_fbo);
            _fbo = 0;
        }
    }
    
    public void Dispose() {
        DisposeInternal();
    }
}