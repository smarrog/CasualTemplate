using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smr.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Smr.Files {
    public class DiskFiles : IFiles {
        private const string JAR_PATH_MARK = "jar:file";
        
        private readonly IChannelLogger _logger;
        
        public DiskFiles(ILogService logger) {
            _logger = logger.GetChannel(LogChannel.Files);
        }
        
        public bool Exists(string path) {
            if (!IsPathInJar(path)) {
                return File.Exists(path);
            }
            
            using var request = UnityWebRequest.Get(path);
            request.SendWebRequest();

            while (!request.isDone) {}

            return request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError;

        }

        private bool IsPathInJar(string path) {
            if (string.IsNullOrEmpty(path)) {
                return false;
            }

            return Application.platform == RuntimePlatform.Android && path.Contains(JAR_PATH_MARK);
        }

        public virtual bool Delete(string path) {
            if (string.IsNullOrEmpty(path)) {
                _logger.LogError("Trying to delete on empty path.");
                return false;
            }

            try {
                if (!File.Exists(path)) {
                    return false;
                }

                File.Delete(path);
                return true;
            } catch (Exception e) {
                _logger.LogError(e, "Delete File Exception");
                return false;
            }
        }

        public long GetLength(string path) {
            var fileInfo = new FileInfo(path);
            return fileInfo.Exists ? fileInfo.Length : 0;
        }

        public virtual void DeleteFilesByMask(string path, string searchPattern) {
            var files = Directory.GetFiles(path, searchPattern);
            foreach (var f in files) {
                Delete(f);
            }
        }

        public byte[] LoadBytes(string path) {
            string error = null;
            var result = LoadBytes(path, ref error);
            if (!string.IsNullOrEmpty(error)) {
                _logger.LogError(error);
            }

            return result;
        }

        public byte[] LoadBytes(string path, ref string error) {
            if (IsPathInJar(path)) {
                return LoadFromJar<byte[]>(path, ref error);
            } else {
                return LoadBytesFromIO(path, ref error);
            }
        }

        private T LoadFromJar<T>(string path, ref string error) where T : class {
            using var request = UnityWebRequest.Get(path);
            request.SendWebRequest();

            while (!request.isDone) {}

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError) {
                error = $"{nameof(DiskFiles)} {nameof(LoadFromJar)} error = {request.error} path ={path}";
                return default;
            }
            
            if (typeof(T) == typeof(string)) {
                return request.downloadHandler?.text as T;
            }

            if (typeof(T) == typeof(byte[])) {
                return request.downloadHandler?.data as T;
            }

            throw new NotSupportedException("Only string and byte[] supported");
        }

        private byte[] LoadBytesFromIO(string path, ref string error) {
            if (!File.Exists(path)) {
                error = "file does not exist at " + path;
                return null;
            }
            try {
                return File.ReadAllBytes(path);
            } catch (Exception e) {
                error = e.Message;
                return null;
            }
        }

        public byte[] LoadBytesFromStream(string path, long position, int length) {
            try {
                using var fileStream = File.Open(path, FileMode.Open);
                var fileData = new byte[length];
                fileStream.Position = position;
                fileStream.Read(fileData, 0, length);
                fileStream.Close();
                return fileData;
            } catch (Exception e) {
                _logger.LogError(e);
                return null;
            }
        }

        public string LoadText(string path) {
            string error = null;
            var result = LoadText(path, ref error);
            if (!string.IsNullOrEmpty(error)) {
                _logger.LogError(error);
            }

            return result;
        }

        public string LoadText(string path, ref string error) {
            return IsPathInJar(path)
                ? LoadFromJar<string>(path, ref error)
                : LoadTextFromIO(path, ref error);
        }

        private string LoadTextFromIO(string path, ref string error) {
            if (!File.Exists(path)) {
                error = "file does not exist at " + path;
                return null;
            }

            try {
                return File.ReadAllText(path, Encoding.UTF8);
            } catch (Exception e) {
                error = e.Message;
                return null;
            }
        }

        public string LoadTextFromStream(string path, long position, int length) {
            var fileData = new byte[length];
            try {
                using var fileStream = File.Open(path, FileMode.Open);
                fileStream.Position = position;
                fileStream.Read(fileData, 0, length);
                fileStream.Close();
            } catch (Exception e) {
                _logger.LogError(e);
                return string.Empty;
            }
            return Encoding.UTF8.GetString(fileData);
        }

        public virtual bool WriteBytes(string path, byte[] bytes) {
            return PerformOperationWithDisk(() => {
                File.WriteAllBytes(path, bytes);
            }, path);
        }

        public virtual bool WriteText(string path, string contents) {
            return PerformOperationWithDisk(() => {
                File.WriteAllText(path, contents);
            }, contents);
        }

        public virtual bool Copy(string source, string dest) {
            return PerformOperationWithDisk(() => {
                File.Copy(source, dest);
            }, dest);
        }

        public virtual bool Copy(string source, string dest, bool overwrite) {
            return PerformOperationWithDisk(() => {
                File.Copy(source, dest, overwrite);
            }, dest);
        }

        public virtual bool Move(string sourceFileName, string destFileName) {
            return PerformOperationWithDisk(() => {
                File.Move(sourceFileName, destFileName);
            }, destFileName);
        }
        
        public IEnumerator<string> GetEnumerator() {
            throw new NotImplementedException();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private bool PerformOperationWithDisk(Action action, string payload) {
            try {
                action.Invoke();
                return true;
            } catch (Exception e) {
                _logger.LogError(e, $"Disk operation exception for {payload}");
                return false;
            }
        }
    }
}