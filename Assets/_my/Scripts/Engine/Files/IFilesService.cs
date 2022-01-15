using System;
using System.IO;
using System.Threading.Tasks;

namespace Smr.Files {
    public interface IFilesService {
        bool CreateDirectorySafe(string dir, out Exception exception);
        bool CreateDirectorySafe(string dir);
        Task DeleteDirectoriesRecursiveSafeAsync(string basePath, params string[] foldersNames);
        bool DeleteDirectoryRecursiveSafe(string path);
        bool DeleteEmptyDirectoriesRecursive(string startFolder);
        string[] GetFilesPaths(string path, string filesMask, SearchOption searchOption = SearchOption.AllDirectories);
        FileInfo[] GetFiles(string path, string filesMask, SearchOption searchOption = SearchOption.AllDirectories);
        DirectoryInfo GetDirectory(string path);
        DirectoryInfo[] GetDirectories(string path, string directoryMask, SearchOption searchOption = SearchOption.AllDirectories);
        bool CopyDirectory(string src, string dst, bool overwrite = true);
        bool IsFileExists(string path);
        long GetFileLength(string path);
        bool IsDirectoryExists(string path);
        bool LoadBytesFile(string path, out byte[] resultBytes);
        bool LoadBytesFile(string path, out byte[] resultBytes, ref string error);
        byte[] LoadBytesFromStream(string path, long position, int length);
        string LoadTextFile(string path);
        bool LoadTextFile(string path, out string result);
        bool LoadTextFile(string path, out string result, ref string error);
        string LoadTextFromStream(string path, long position, int length);
        bool WriteAllBytesSafe(string path, byte[] bytes);
        bool WriteAllTextSafe(string path, string contents);
        void DeleteFilesByMask(string path, string fileMask);
        bool Copy(string source, string dest);
        bool Copy(string source, string dest, bool overwrite);
        bool Move(string sourceFileName, string destFileName);
        
        bool DeleteFileSafe(string path);
        bool IsFileHidden(string path);
        bool IsInHiddenDirectory(string path);
    }
}