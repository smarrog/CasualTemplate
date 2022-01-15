using System;
using System.IO;
using System.Threading.Tasks;

namespace Smr.Files {
    public class FilesService : IFilesService {
        private readonly IFiles _files;
        private readonly IDirectories _directories;
        
        public FilesService(IFiles files, IDirectories directories) {
            _files = files;
            _directories = directories;
        }
        
        public bool CreateDirectorySafe(string dir, out Exception exception) {
            return _directories.CreateDirectorySafe(dir, out exception);
        }

        public bool CreateDirectorySafe(string dir) {
            return CreateDirectorySafe(dir, out _);
        }
        
        public Task DeleteDirectoriesRecursiveSafeAsync(string basePath, params string[] foldersNames) {
            return _directories.DeleteDirectoriesRecursiveSafeAsync(basePath, foldersNames);
        }

        public bool DeleteDirectoryRecursiveSafe(string path) {
            return _directories.DeleteDirectoryRecursiveSafe(path);
        }

        public bool DeleteEmptyDirectoriesRecursive(string startFolder) {
            return _directories.DeleteEmptyDirectoriesRecursive(startFolder);
        }

        public string[] GetFilesPaths(string path, string filesMask, SearchOption searchOption) {
            return _directories.GetFilesPaths(path, filesMask, searchOption);
        }
        
        public FileInfo[] GetFiles(string path, string filesMask, SearchOption searchOption) {
            return _directories.GetFiles(path, filesMask, searchOption);
        }

        public DirectoryInfo GetDirectory(string path) {
            return _directories.GetDirectory(path);
        }
        
        public DirectoryInfo[] GetDirectories(string path, string directoryMask, SearchOption searchOption) {
            return _directories.GetDirectories(path, directoryMask, searchOption);
        }

        public bool CopyDirectory(string src, string dst, bool overwrite = true) {
            return _directories.CopyDirectory(src, dst, overwrite);
        }

        public bool IsFileExists(string path) {
            return _files.Exists(path);
        }
        
        public long GetFileLength(string path) {
            return _files.GetLength(path);
        }

        public bool IsDirectoryExists(string path) {
            return _directories.Exists(path);
        }

        public bool LoadBytesFile(string path, out byte[] resultBytes) {
            string error = null;
            resultBytes = _files.LoadBytes(path, ref error);
            return resultBytes != null;
        }

        public bool LoadBytesFile(string path, out byte[] resultBytes, ref string error) {
            resultBytes = _files.LoadBytes(path, ref error);
            return resultBytes != null;
        }

        public byte[] LoadBytesFromStream(string path, long position, int length) {
            return _files.LoadBytesFromStream(path, position, length);
        }

        public string LoadTextFile(string path) {
            LoadTextFile(path, out string contents);
            return contents;
        }

        public bool LoadTextFile(string path, out string result) {
            string error = null;
            var fileText = _files.LoadText(path, ref error);
            result = fileText?.Trim();
            return fileText != null;
        }

        public bool LoadTextFile(string path, out string result, ref string error) {
            var fileText = _files.LoadText(path, ref error);
            result = fileText?.Trim();
            return fileText != null;
        }

        public string LoadTextFromStream(string path, long position, int length) {
            return _files.LoadTextFromStream(path, position, length);
        }

        public bool WriteAllBytesSafe(string path, byte[] bytes) {
            return _files.WriteBytes(path, bytes);
        }

        public bool WriteAllTextSafe(string path, string contents) {
            return _files.WriteText(path, contents);
        }

        public void DeleteFilesByMask(string path, string fileMask) {
            _files.DeleteFilesByMask(path, fileMask);
        }

        public bool Copy(string source, string dest) {
            return _files.Copy(source, dest);
        }

        public bool Copy(string source, string dest, bool overwrite) {
            return _files.Copy(source, dest, overwrite);
        }

        public bool Move(string sourceFileName, string destFileName) {
            return _files.Move(sourceFileName, destFileName);
        }
        
        public bool DeleteFileSafe(string path) {
            return _files.Delete(path);
        }

        public bool IsFileHidden(string path) {
            var file = new FileInfo(path);
            return file.Attributes.HasFlag(FileAttributes.Hidden);
        }

        public bool IsInHiddenDirectory(string path) {
            var dir = Directory.GetParent(path);
            while (dir != null) {
                if (dir.Attributes.HasFlag(FileAttributes.Hidden)) {
                    return true;
                }
                
                if(dir.Root == dir) {
                    return false;
                }
                
                var parent = dir.Parent;
                dir = parent;
            }
            return false;
        }
    }
}