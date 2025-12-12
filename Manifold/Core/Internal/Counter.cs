namespace Manifold.Internal;

public class Counter {
    private static int numDrawnRenderables = 0;
    
    public static int VisibleRenderables => numDrawnRenderables;

    public static void MarkRenderableVisible() {
        numDrawnRenderables++;
    }

    public static void MarkRenderableHidden() {
        numDrawnRenderables--;
    }

    public static void Reset() {
        numDrawnRenderables = 0;
    }
}