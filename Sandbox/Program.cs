using Sandbox.Application;

namespace Sandbox;

class Program {
    static void Main(string[] args) {
        using (var app = new SandboxApp("Engine Sandbox")) {
            app.Run();
        }
    }
}