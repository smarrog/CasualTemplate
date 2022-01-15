using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Smr.Utils {
    public static class PathUtils {
        public static string GetRelativePath(Uri baseUri, string path) {
            Uri absoluteUri = new Uri(path);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(absoluteUri).OriginalString);
        }

        public static string CombineForUnity(params string[] parts) {
            if (parts == null || parts.Length == 0) {
                return null;
            }

            string str = parts[0];
            for (int i = 1; i < parts.Length; ++i) {
                var hasEndSlash = str.EndsWith("\\") || str.EndsWith("/");

                string currPart = parts[i];

                while (currPart.StartsWith("\\") || currPart.StartsWith("/")) {
                    currPart = currPart.Substring(1);
                }

                if (hasEndSlash) {
                    str += currPart;
                } else {
                    str = str + "/" + currPart;
                }
            }

            str = str.Replace('\\', '/');
            return str;
        }


        public static string Directorify(string path) {
            char last = path[path.Length - 1];
            if (last == Path.DirectorySeparatorChar) {
                return path;
            }

            if (last == Path.AltDirectorySeparatorChar) {
                return path;
            }

            return path + Path.DirectorySeparatorChar;
        }

        public static bool IsFileNameValid(string fileName) {
            // empty or space name
            if (string.IsNullOrWhiteSpace(fileName)) {
                return false;
            }

            // bad chars
            var containsABadCharacter = new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]");
            if (containsABadCharacter.IsMatch(fileName)) {
                return false;
            }

            return true;
        }

        public static bool IsAssetExists(string assetPath) => File.Exists(Application.dataPath + assetPath["Assets".Length..]);
        
        public static string GetConsolePath() {
#if UNITY_EDITOR_WIN
            return @"C:\Program Files\Git\bin\bash.exe";
#else
            return "/bin/bash";
#endif
        }

        public static string GetDotnetPath() {
#if UNITY_EDITOR_WIN
            return @"C:\Program Files\dotnet";
#else
            return "/usr/local/bin" + GetPathSeparator() + "/opt/homebrew/bin" + GetPathSeparator() + "/usr/local/share" +
                   GetPathSeparator() + "/usr/local/share/dotnet";
#endif
        }        
        
        public static string GetPathSeparator() {
#if UNITY_EDITOR_WIN
            return ";";
#else
            return ":";
#endif
        }
    }
}