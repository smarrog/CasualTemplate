using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smr.Files {
    public class MemoryFiles : IFiles {
        private readonly Dictionary<string, byte[]> _files = new();
        
        public bool Exists(string path) {
            path = path.Replace("\\", "/");
            return _files.ContainsKey(path);
        }

        public bool Delete(string path) {
            path = path.Replace("\\", "/");
            return _files.Remove(path);
        }

        public long GetLength(string path) {
            path = path.Replace("\\", "/");
            return _files.ContainsKey(path) ? _files[path].Length : 0;
        }

        public void DeleteFilesByMask(string path, string fileMask) {
            throw new NotImplementedException();
        }

        public byte[] LoadBytes(string path) {
            path = path.Replace("\\", "/");
            return _files.ContainsKey(path) ? _files[path] : null;
        }

        public byte[] LoadBytes(string path, ref string error) {
            path = path.Replace("\\", "/");
            return _files.ContainsKey(path) ? _files[path] : null;
        }

        public byte[] LoadBytesFromStream(string path, long position, int length) {
            path = path.Replace("\\", "/");
            var fileData = _files[path];

            byte[] outputData = new byte[length];
            Array.Copy(fileData, position, outputData, 0, length);

            return outputData;
        }

        public string LoadText(string path) {
            path = path.Replace("\\", "/");
            var bytes = _files.ContainsKey(path) ? _files[path] : Array.Empty<byte>();
            return Encoding.UTF8.GetString(bytes);
        }

        public string LoadText(string path, ref string error) {
            path = path.Replace("\\", "/");
            var bytes = _files.ContainsKey(path) ? _files[path] : Array.Empty<byte>();
            return Encoding.UTF8.GetString(bytes);
        }

        public string LoadTextFromStream(string path, long position, int length) {
            throw new NotImplementedException();
        }

        public bool WriteBytes(string path, byte[] bytes) {
            path = path.Replace("\\", "/");
            _files[path] = bytes.ToArray();
            return true;
        }

        public bool WriteText(string path, string contents) {
            path = path.Replace("\\", "/");
            _files[path] = Encoding.UTF8.GetBytes(contents);
            return true;
        }

        public bool Copy(string source, string dest) {
            throw new NotImplementedException();
        }

        public bool Copy(string source, string dest, bool overwrite) {
            throw new NotImplementedException();
        }

        public bool Move(string sourceFileName, string destFileName) {
            throw new NotImplementedException();
        }
        
        public IEnumerator<string> GetEnumerator() {
            return _files.Keys.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}