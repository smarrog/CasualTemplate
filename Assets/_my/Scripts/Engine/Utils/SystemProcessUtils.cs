using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Smr.Common;

namespace Smr.Utils {
    public static class SystemProcessUtils {
        public enum ProcessResult {
            Failed = 0,
            Completed = 1,
            TimedOut = 2,
        }

        public static Process Create(ProcessStartInfo processStartInfo) {
            var process = new Process();
            process.StartInfo = processStartInfo;
            process.StartInfo.Environment["PATH"] = MakePath(PathUtils.GetDotnetPath(), System.Environment.GetEnvironmentVariable("PATH"));

            return process;
        }
        
        //  https://stackoverflow.com/a/7608823
        public static ProcessResult RunProcess(this Process process, string processLabel, IChannelLogger logger, int timeout = Timeout.Infinite) {
            var processFilePath = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
            if (!File.Exists(processFilePath)) {
                logger.LogError($"Process file '{processFilePath}' not found");
                return ProcessResult.Failed;
            }

            var outputBuilder = new StringBuilder();
            var errBuilder = new StringBuilder();

            var outputWaitHandle = new AutoResetEvent(false);
            var errorWaitHandle = new AutoResetEvent(false);

            process.OutputDataReceived += (_, e) => {
                if (e.Data == null) {
                    outputWaitHandle.Set();
                } else {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (_, e) => {
                if (e.Data == null) {
                    errorWaitHandle.Set();
                } else {
                    outputBuilder.AppendLine($"<color=red>{e.Data}</color>");
                    errBuilder.AppendLine($"<color=red>{e.Data}</color>");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            ProcessResult processResult;

            var processFinished = process.WaitForExit(timeout) &&
                outputWaitHandle.WaitOne(timeout) &&
                errorWaitHandle.WaitOne(timeout);

            if (processFinished) {
                processResult = process.ExitCode != 0 ? ProcessResult.Failed : ProcessResult.Completed;
            } else {
                processResult = ProcessResult.TimedOut;
            }

            if (outputBuilder.Length == 0) {
                outputBuilder.Append($"Process {process.StartInfo.FileName} {process.StartInfo.Arguments} finished with {processResult} result.");
                if (processFinished) {
                    outputBuilder.Append($" Exit code = {process.ExitCode:X}.");
                }
            }

            PrintProcessOutput(processResult, outputBuilder.ToString(), errBuilder.ToString(), processLabel, logger);
            
            return processResult;
        }

        private static string MakePath(params string[] parts) {
            var separator = PathUtils.GetPathSeparator();
            return string.Join(separator, parts);
        }

        private static void PrintProcessOutput(ProcessResult processResult, string output, string error, string processLabel, IChannelLogger logger) {
            switch (processResult) {
                case ProcessResult.Completed:
                    logger.Log($"Output received:\n{output}");
                    logger.Log($"{processLabel} complete");
                    break;

                case ProcessResult.TimedOut:
                    logger.LogError($"Output received:\n{output}");
                    logger.LogError($"{processLabel} timed out");
                    break;

                case ProcessResult.Failed:
                default:
                    logger.LogError($"Output received:\n{output}");
                    logger.LogError($"Errors received:\n{error}");
                    logger.LogError($"{processLabel} finished with errors");
                    break;
            }
        }
    }
}