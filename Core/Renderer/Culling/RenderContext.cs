using System.Runtime.InteropServices;
using Manifold.Core.Renderer.Maths;
using Manifold.Core.Renderer.Types;
using Manifold.Core.SceneSystem;
using Manifold.Internal;
using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Culling;
public struct CullItem
{
    public int Id;               // Renderable ID / index
    public AABB WorldBounds;
}


public sealed class RenderContext {
    public const float CULL_FOV_PADDING = 1.25f; // 25% more FOV
    public const float CULL_DISTANCE_PADDING = 1.10f; // 10% more far plane

    private volatile int _activeResultIndex;
    private volatile int _writeResultIndex = 1;

    private readonly List<int>[] _visibleSets = {
        new List<int>(4096), // 0 = front (render thread reads this)
        new List<int>(4096) // 2 = back (culling thread writes here)
    };

    private Task? _cullTask;

    public ReadOnlySpan<int> VisibleRenderables =>
        CollectionsMarshal.AsSpan(_visibleSets[_activeResultIndex]);

    /// <summary>
    /// Kicks off a cull job if none is running. Triple-buffered, no stalls.
    /// </summary>
    public void Cull(Scene scene, Camera camera) {
        // Skip new job if previous hasn't finished
        if (_cullTask is { IsCompleted: false })
            return;

        RenderSceneSnapshot snapshot = scene.GetCullSnapshot();
        Matrix4 vp = camera.GetViewProjectionCulling(1.05f);

        // Schedule async culling job
        _cullTask = Task.Run(() => { PerformCulling(snapshot, vp); });
    }

    private void PerformCulling(RenderSceneSnapshot snapshot, Matrix4 viewProjection) {
        Frustum frustum = Frustum.FromMatrix(viewProjection);

        int writeIndex = _writeResultIndex;
        var visible = _visibleSets[writeIndex];
        visible.Clear();

        var items = snapshot.Items;

        // Thread-local lists for collecting visible IDs
        Parallel.For(0, items.Length,
            () => new List<int>(1024), // Thread-local init
            (i, loop, localList) =>
            {
                ref readonly var item = ref items[i];

                if (Frustum.IsVisible(item.WorldBounds, frustum)) {
                    localList.Add(item.Id);
                }
                else {
                }


                return localList;
            },
            localList =>
            {
                // Merge each thread's results into the output list ONCE
                lock (visible) {
                    visible.AddRange(localList);
                }
            });

        // ---- Triple Buffer Swap ----
        int oldFront = _activeResultIndex;
        _activeResultIndex = writeIndex;
        _writeResultIndex = oldFront;
    }

    public void WaitForCulling(int maxMilliseconds) {
        if (_cullTask == null) return;

        _cullTask.Wait(maxMilliseconds);
    }
}