using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Smr.Common;
using UnityEngine;

namespace Smr.Extensions {
    public static class TaskExtensions {
        public static async UniTask<T> ContinueWith<T>(this UniTask<T> task, Action<T> onComplete, Action<Exception> onException = null) {
            var result = default(T);
            try {
                result = await task;
                onComplete?.Invoke(result);
            } catch (Exception e) {
                onException?.Invoke(e);
            }
            return result;
        }

        public static void HandleException<T>(this UniTask<T> task, Action<Exception> onException = null) {
            task.HandleInternal(onException).Forget();
        }

        public static void HandleException(this UniTask task, Action<Exception> onException = null) {
            task.HandleInternal(onException).Forget();
        }

        private static async UniTask HandleInternal(this UniTask task, Action<Exception> onException) {
            try {
                await task;
            } catch (Exception e) {
                EngineDependencies.Logger.LogError(e);
                onException.Invoke(e);
            }
        }

        private static async UniTask<T> HandleInternal<T>(this UniTask<T> task, Action<Exception> onException) {
            var result = default(T);
            try {
                result = await task;
            } catch (Exception e) {
                onException?.Invoke(e);
            }
            return result;
        }

        public static UniTask WhenAll<T>(this IEnumerable<T> items, Func<T, UniTask> action) => UniTask.WhenAll(Enumerable.Select(items, action));
    }
}