using System.Collections.Generic;

namespace Smr.Files {
    public interface IFiles : IEnumerable<string> {
        bool Exists(string path);
        bool Delete(string path);
        long GetLength(string path);

        void DeleteFilesByMask(string path, string fileMask);

        byte[] LoadBytes(string path);
        byte[] LoadBytes(string path, ref string error);
        byte[] LoadBytesFromStream(string path, long position, int length);

        string LoadText(string path);
        string LoadText(string path, ref string error);
        string LoadTextFromStream(string path, long position, int length);

        bool WriteBytes(string path, byte[] bytes);
        bool WriteText(string path, string contents);

        bool Copy(string source, string dest);
        bool Copy(string source, string dest, bool overwrite);
        bool Move(string sourceFileName, string destFileName);
    }
}