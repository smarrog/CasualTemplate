using System;
using System.IO;
using System.Threading.Tasks;

namespace Smr.Files {
    public interface IDirectories {
        bool CreateDirectorySafe(string path, out Exception exception);
        bool DeleteDirectoryRecursiveSafe(string path);
        Task DeleteDirectoriesRecursiveSafeAsync(string basePath, params string[] foldersNames);
        bool DeleteEmptyDirectoriesRecursive(string startFolder);
        bool Exists(string path);
        string[] GetFilesPaths(string path, string filesMask, SearchOption searchOption);
        FileInfo[] GetFiles(string path, string filesMask, SearchOption searchOption);
        DirectoryInfo GetDirectory(string path);
        DirectoryInfo[] GetDirectories(string path, string directoryMask, SearchOption searchOption);
        bool CopyDirectory(string src, string dst, bool overwrite = true);
    }
}