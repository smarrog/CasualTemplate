using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Smr.Common;
using Smr.Extensions;

namespace Smr.Files {
    public class DiskDirectories : IDirectories {
        private readonly IChannelLogger _logger;
        
        public DiskDirectories(ILogService logger) {
            _logger = logger.GetChannel(LogChannel.Files);
        }

        public virtual bool CreateDirectorySafe(string dir, out Exception exception) {
            exception = null;
            try {
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                return true;
            } catch (Exception e) {
                exception = e;
                return false;
            }
        }

        public virtual async Task DeleteDirectoriesRecursiveSafeAsync(string basePath, params string[] folderNames) {
            try {
                var folderToDelete = folderNames.Select(x => Path.Combine(basePath, x));
                await UniTask.RunOnThreadPool(() => {
                    foreach (var path in folderToDelete) {
                        if (Directory.Exists(path)) {
                            Directory.Delete(path, true);
                        }
                    }
                });
            } catch (Exception e) {
                _logger.LogError(e, $"basePath= {basePath} folders={folderNames.ToDebugString()}");
            }
        }

        public virtual bool DeleteDirectoryRecursiveSafe(string path) {
            try {
                if (!Directory.Exists(path)) {
                    return false;
                }

                Directory.Delete(path, true);
                return true;
            } catch (UnauthorizedAccessException e) {
                _logger.LogError(e);
            } catch (Exception e) {
                var errorMessage = $"Delete Directory Exception. File={path}";
                _logger.LogError(e, errorMessage);
            }

            return false;
        }

        public virtual bool DeleteEmptyDirectoriesRecursive(string startFolder) {
            try {
                if (!Directory.Exists(startFolder)) {
                    return false;
                }

                foreach (var directory in Directory.GetDirectories(startFolder)) {
                    DeleteEmptyDirectoriesRecursive(directory);
                    if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0) {
                        Directory.Delete(directory, false);
                    }
                }

                return true;
            } catch (UnauthorizedAccessException e) {
                _logger.LogError(e);
            } catch (Exception e) {
                var errorMessage = $"Delete empty folders Exception. StartFolder={startFolder}";
                _logger.LogError(e, errorMessage);
            }

            return false;
        }

        public bool Exists(string path) {
            return Directory.Exists(path);
        }
        public string[] GetFilesPaths(string path, string filesMask, SearchOption searchOption) {
            return Directory.GetFiles(path, filesMask, searchOption);
        }

        public FileInfo[] GetFiles(string path, string filesMask, SearchOption searchOption) {
            var directory = new DirectoryInfo(path);
            return directory.Exists
                ? directory.GetFiles(filesMask, searchOption)
                : Array.Empty<FileInfo>();
        }

        public DirectoryInfo GetDirectory(string path) {
            return new DirectoryInfo(path);
        }

        public DirectoryInfo[] GetDirectories(string path, string directoryMask, SearchOption searchOption) {
            var directory = new DirectoryInfo(path);
            return directory.Exists
                ? directory.GetDirectories(directoryMask, searchOption)
                : Array.Empty<DirectoryInfo>();
        }

        public virtual bool CopyDirectory(string src, string dst, bool overwrite = true) {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dst)) {
                _logger.LogError($"Trying to copy directory, but path is empty src={src} dst={dst}");
                return false;
            }

            if (!Directory.Exists(src)) {
                _logger.LogError($"{this} Source dir {src} does not exist");
                return false;
            }

            if (!CreateDirectorySafe(dst, out _)) {
                return false;
            }

            try {
                var directoryInfo = new DirectoryInfo(src);
                foreach (var info in directoryInfo.GetDirectories()) {
                    CopyDirectory(info.FullName, Path.Combine(dst, info.Name));
                }

                foreach (var info in directoryInfo.GetFiles()) {
                    info.CopyTo(Path.Combine(dst, info.Name), overwrite);
                }
            } catch (Exception e) {
                _logger.LogError(e);
                return false;
            }

            return true;
        }
    }
}