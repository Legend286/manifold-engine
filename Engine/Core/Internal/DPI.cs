using Manifold.Core.Runtime;

namespace Manifold.Core.Internal;

public static class DPI
{
    public static float X;
    public static float Y;
    static DPI()
    {
        X = Application.Instance.OpenTKWindow.CurrentMonitor.HorizontalScale;
        Y = Application.Instance.OpenTKWindow.CurrentMonitor.VerticalScale;
    }
}