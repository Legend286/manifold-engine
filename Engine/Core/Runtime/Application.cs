using System.Diagnostics;
using Manifold.Core.Events;
using Manifold.Core.InputSystem;
using Manifold.Core.Layers;
using Manifold.Core.Renderer;
using Manifold.Core.SceneSystem;
using Manifold.Core.Windowing;
using Manifold.Core.Windowing.Platform;
using Manifold.Core.Windowing.Platform.Input;
using OpenTK.Mathematics;
using Version = Manifold.Core.Internal.Version;

namespace Manifold.Core.Runtime;

public abstract class Application : IDisposable {
    private IWindow _window;
    private bool _running = true;
    private Camera _mainCamera;
    public Camera MainCamera {get => _mainCamera; set => _mainCamera = value; }
    
    public static Application Instance { get; private set; }

    private readonly LayerStack _layerStack = new LayerStack();
    
    public LayerStack LayerStack => _layerStack;

    public void PushLayer(Layer layer) => _layerStack.PushLayer(layer);
    public void PushOverlay(Layer layer) => _layerStack.PushOverlay(layer);
    public int Width => _window.Width;
    public int Height => _window.Height;

    public OpenTKWindow OpenTKWindow;
    public Application(string title) {
        if (Instance != null) {
            throw new Exception("Application already exists!");
        }
        Instance = this;

        var props = new WindowProperties($"{title} - {Version.GetVersion()}", 1280, 720, true);
        
        var tkWindow = new OpenTKWindow(props);

        OpenTKWindow = tkWindow;
        _window = tkWindow; 

        _window.SetEventCallback(OnEvent);

        Input.Init(new OpenTKInput(tkWindow));
        
        RenderCommand.Init();
        
        Console.WriteLine($"{this.GetType().Name} Initialized.");
    }

    private void OnEvent(Event e) {
        EventDispatcher dispatcher = new EventDispatcher(e);
        dispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
        dispatcher.Dispatch<WindowResizeEvent>(OnWindowResize);

        foreach (var layer in _layerStack.Reverse()) {
            layer.OnEvent(e);

            if (e.Handled) {
                break;
            }
        }
    }
    public void Run() {

        Stopwatch timer = new Stopwatch();
        timer.Start();
        TimeSpan lastTime = timer.Elapsed;
        
        while (_running) {
            _window.OnUpdate();
            TimeSpan currentTime = timer.Elapsed;
            float deltaTime = (float)(currentTime - lastTime).TotalSeconds;
            lastTime = currentTime;

// Clamp dt so first-frame jitter cannot occur
            deltaTime = Math.Clamp(deltaTime, 0.0f, 0.05f);
            
            OnUpdate(deltaTime);
            OnRender();
            
            OpenTKWindow.SwapBuffers();
        }

        Close();
    }

    protected virtual void OnUpdate(float deltaTime) {
        foreach (var layer in _layerStack) {
            if(layer.IsVisible)
                layer.OnUpdate(deltaTime);
        }
    }

    protected virtual void OnRender() {
        RenderCommand.SetClearColor(Color4.Black);
        RenderCommand.Clear();

        foreach (var layer in _layerStack) {
            if(layer.IsVisible)
                layer.OnRender();
        }
        
        
    }

    private bool OnWindowClose(WindowCloseEvent e) {
        _running = false;

        return true;
    }

    private bool OnWindowResize(WindowResizeEvent e) {
        Console.WriteLine($"Resized {this.GetType().Name} to: {e.Width} x {e.Height}");
        
        return false;
    }

    public void Close() {
        Console.WriteLine($"Closing {this.GetType().Name}.");
        _running = false;
    }

    public void SetCamera(Camera camera) {
        _mainCamera = camera;
    }
    public void Dispose() {
        
    }
}