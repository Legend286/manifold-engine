using Manifold.Core.Renderer.Containers;
using Manifold.Core.Renderer.Culling;
using Manifold.Core.Renderer.Modules;
using Manifold.Core.Renderer.Culling;
using Manifold.Core.Renderer.Maths;
using OpenTK.Mathematics;

namespace Manifold.Core.SceneSystem;

public readonly struct RenderSceneSnapshot
{
    public readonly CullItem[] Items;

    public RenderSceneSnapshot(CullItem[] items)
    {
        Items = items;
    }
}

public sealed class Scene
{
    private readonly List<Renderable> _renderables = new();

    public IReadOnlyList<Renderable> Renderables => _renderables;

    private RenderSceneSnapshot _snapshot;

    private bool bIsSceneDirty = false;

    public int AddRenderable(Renderable renderable)
    {
        _renderables.Add(renderable);
        bIsSceneDirty = true;
        return _renderables.Count - 1; // ID = index
    }

    public int RemoveRenderable(Renderable renderable) {
        _renderables.Remove(renderable);
        bIsSceneDirty = true;
        return _renderables.Count - 1;
    }
    
    public RenderSceneSnapshot GetCullSnapshot() {
        if (bIsSceneDirty) {
            _snapshot = BuildCullSnapshot();
            bIsSceneDirty = false;
        }
        
        return _snapshot;
    }
    
    public Renderable GetRenderable(int id) => _renderables[id];

    /// <summary>
    /// Build a read-only snapshot for the culling thread.
    /// </summary>
    public RenderSceneSnapshot BuildCullSnapshot()
    {
        var items = new CullItem[_renderables.Count];

        for (int i = 0; i < _renderables.Count; i++)
        {
            var r = _renderables[i];
            
            AABB worldBounds = r.GetRenderBounds();

            items[i] = new CullItem
            {
                Id = i,
                WorldBounds = worldBounds
            };
        }

        return new RenderSceneSnapshot(items);
    }
}