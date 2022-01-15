using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Game.Editor {
    public class CommonBuildPostprocessor : IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report) {
            //ArchiveWebGLBuild(report);
        }

        private void ArchiveWebGLBuild(BuildReport report) {
            if (report.summary.platform != BuildTarget.WebGL) {
                return;
            }
            
            var buildPath = report.summary.outputPath;
            var filePath = $"{buildPath}.zip";
            var prevFilePath = $"{buildPath}_prev.zip";
            
            if (File.Exists(prevFilePath)) {
                File.Delete(prevFilePath);
            }
            
            if (File.Exists(filePath)) {
                File.Move(filePath, prevFilePath);
            }
            
            ZipFile.CreateFromDirectory(buildPath, filePath);
        }
    }
}