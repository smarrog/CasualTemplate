using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Smr.Extensions;
using Smr.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Wdk.Dev {
    public static class GitUtils {
        private static string _projectPath;

        public static string GetBranchName() => CallGit("rev-parse --abbrev-ref HEAD").FirstOrDefault();
        public static string GetCommitGuid() => CallGit("rev-parse HEAD").FirstOrDefault();
        public static string GetMergeBase(string withCommit) => CallGit($"merge-base HEAD {withCommit}").FirstOrDefault();

        public static IEnumerable<string> GetBranchesList() => CallGit("branch --list");
        public static string GetBranchCommit(string branch) => CallGit($"rev-parse {branch}").FirstOrDefault();

        public static List<string> GetDiffFilePaths() {
            var files = CallGit("status --porcelain");
            var result = new List<string>();
            foreach (var file in files) {
                if (file == null) {
                    continue;
                }
                if (file[0] == 'D' || file[1] == 'D') {
                    continue;
                }
                string filePath;
                if (file.Contains("->")) {
                    filePath = file.Split("->")[1].Trim();
                } else {
                    filePath = file.Substring(3);
                }

                if (file.EndsWith('/')) {
                    var allFiles = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
                    result.AddRange(allFiles.Select(f => f.Replace("\\", "/")));
                    continue;
                }

                filePath = filePath.Replace("\"", "");
                result.Add(filePath);
            }
            return result;
        }

        public static List<string> GetDiffInIndexFilePaths() => CallGit("diff --diff-filter=d --cached --name-only");
        public static List<string> GetDiffInNotIndexFilePaths() => GetDiffInNotIndexModifiedFilePaths().Concat(GetDiffInNotIndexNewFilePaths()).ToList();
        public static List<string> GetDiffInNotIndexModifiedFilePaths() => CallGit("ls-files -m");
        public static List<string> GetDiffInNotIndexNewFilePaths() => CallGit("ls-files --others --exclude-standard");

        public static string GetOriginUrl() => CallGit("config --local remote.origin.url").JoinToString();

        public static string GetProjectName() {
            var originUrl = GetOriginUrl();
            try {
                var re = new Regex("^(.+)/(.+).git$");
                var strings = re.Split(originUrl);
                return strings[2];
            } catch (Exception) {
                return "";
            }
        }

        /// <summary> Возвращает автора с наибольшим числом коммитов </summary>
        public static string GetMostAuthor(string assetPath, bool withEmail = false) {
            var emailOption = withEmail ? " -e" : "";
            string output = CallGit($"--no-pager shortlog -sn HEAD \"{assetPath}\" {emailOption}").JoinToString();
            if (output.IsEmpty()) {
                return "";
            }
            string firstAuthorInfo = output.Split("     ", StringSplitOptions.RemoveEmptyEntries)[0];
            if (firstAuthorInfo.IsEmpty()) {
                return "";
            }
            var authorInfos = firstAuthorInfo.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            return authorInfos.Length > 1 ? authorInfos[1] : "";
        }

        public static List<string> CallGit(string args) => CallShell("git", args);
        
        public static List<string> GetConflictedFiles() => CallGit("diff --name-only --diff-filter=U");

        public static void ResolveConflict(string path, bool theirs = false) => CallGit(theirs ? $"checkout --theirs \"{path}\"" : $"checkout --ours \"{path}\"");
        
        public static void StageFile(string path) => CallGit($"add \"{path}\"");

        private static List<string> CallShell(string cmd, string args) {
            List<string> output = new List<string>();
            List<string> error = new List<string>();

#if UNITY_EDITOR
            var startInfo = new ProcessStartInfo(cmd) {
                WorkingDirectory = GetProjectPath(),
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = SystemProcessUtils.Create(startInfo);
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (_, evt) => {
                if (evt.Data != null) {
                    output.Add(evt.Data);
                }
            };

            process.ErrorDataReceived += (_, evt) => {
                if (evt.Data != null) {
                    error.Add(evt.Data);
                }
            };

            try {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (error.IsFilled() && !error.All(string.IsNullOrEmpty)) {
                    Debug.LogError($"{cmd} process error: {error.JoinToString("\n")}");
                }
            } catch (Exception ex) {
                Debug.LogError($"{cmd} process exception: {ex.Message}");
            } finally {
                process.Close();
            }
#endif
            return output;
        }

        private static string GetProjectPath() => _projectPath ??= Application.dataPath.Replace("Assets", "").Replace("\\", "/");
    }
}