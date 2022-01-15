using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;

namespace MessagePackTools.Catalog {
    class Application : ConsoleAppBase {
        private static bool DebugMode = false;

        public static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder()
                .ConfigureLogging(logging => logging.ReplaceToSimpleConsole())
                .RunConsoleAppFrameworkAsync<Application>(args);
        }

        public async Task RunAsync([Option("d", "debug mode")] bool debug = false) {
            //TODO process custom project
            DebugMode = debug;

            var roots = new[] {
                "../Assets/_my"
            };

            var catalog = MessagePackObjectsCatalogBuilder.Build(roots, DebugMode);
            
            if (DebugMode) {
                var applicationDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var catalogFile = Path.Combine(applicationDir!, "catalog.txt");
                catalog.SaveReportToFile(catalogFile);
            }

            catalog.GenerateProjects("./Temp/mp_csprojs_generated/");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(catalog.AsmdefPathToHash));

            if (DebugMode) {
                Console.WriteLine("\nDone!");
            }
        }
    }
}