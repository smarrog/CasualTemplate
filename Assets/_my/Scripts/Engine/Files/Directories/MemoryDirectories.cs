using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Smr.Files {
    public class MemoryDirectories : IDirectories {
        public readonly MemoryFiles MemFiles;


        public MemoryDirectories(MemoryFiles memFiles) {
            MemFiles = memFiles;
        }

        public bool CreateDirectorySafe(string path, out Exception exception) {
            exception = null;
            return true;
        }

        public bool DeleteDirectoryRecursiveSafe(string path) {
            // TODO: remove files with dir
            return true;
        }

        public Task DeleteDirectoriesRecursiveSafeAsync(string basePath, params string[] foldersNames) {
            throw new NotImplementedException();
        }

        public bool DeleteEmptyDirectoriesRecursive(string startFolder) {
            return true;
        }

        public bool Exists(string path) {
            path = path.Replace("\\", "/");
            return MemFiles.Any(fileName => fileName.StartsWith(path));
        }
        
        public string[] GetFilesPaths(string path, string filesMask, SearchOption searchOption) {
            path = path.Replace("\\", "/");
            var regxMask = new Regex(filesMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));

            return MemFiles
                .Where(f => f.StartsWith(path) && regxMask.IsMatch(f))
                .ToArray();
        }

        public FileInfo[] GetFiles(string path, string filesMask, SearchOption searchOption) {
            path = path.Replace("\\", "/");
            var regxMask = new Regex(filesMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));

            return MemFiles
                .Where(f => f.StartsWith(path) && regxMask.IsMatch(f))
                .Select(f => new FileInfo(f))
                .ToArray();
        }

        public DirectoryInfo GetDirectory(string path) {
            throw new NotImplementedException();
        }
        
        public DirectoryInfo[] GetDirectories(string path, string directoryMask, SearchOption searchOption) {
            throw new NotImplementedException();
        }

        public bool CopyDirectory(string src, string dst, bool overwrite = true) {
            throw new NotImplementedException();
        }
    }
}