namespace Manifold.Internal;

public static class Version {
    public static readonly int Major = 0;
    public static readonly int Minor = 0;
    public static readonly int Patch = 1;
    public static readonly string Name = "Kitten";

    public static string GetVersion() {
        return $"Version {Major}.{Minor}.{Patch} - '{Name}'";
    }
} 