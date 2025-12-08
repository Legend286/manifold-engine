using Sandbox;

namespace Manifold.Test;

class Program {
    static void Main(string[] args) {
        using (var app = new SandboxApp("Manifold Sandbox")) {
            app.Run();
        }
    }
}