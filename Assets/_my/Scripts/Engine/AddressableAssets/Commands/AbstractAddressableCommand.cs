#if SMR_ADDRESSABLES
using Smr.Commands;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Smr.AddressableAssets {
    public abstract class AbstractAddressableCommand<T> : AbstractCommand {
        private AsyncOperationHandle<T> _operationHandle;
        public T Result { get; private set; }

        protected override void ExecuteInternal() {
            try {
                _operationHandle = CreateOperationHandle();
            } catch (InvalidKeyException) {
                NotifyFail();
                return;
            }

            if (!_operationHandle.IsValid()) {
                NotifyFail();
                return;
            }

            if (!_operationHandle.IsDone) {
                _operationHandle.Completed += OnCompleteOperation;
                return;
            }

            OnCompleteOperation(_operationHandle);
        }

        protected override void CleanUpInternal(CommandResult result) {
            if (result != CommandResult.Completed) {
                Addressables.Release(_operationHandle);
            }
            base.CleanUpInternal(result);
        }

        protected abstract AsyncOperationHandle<T> CreateOperationHandle();

        protected void SetResultAndComplete(T result) {
            Result = result;
            if (Result == null) {
                NotifyFail();
                return;
            }

            NotifyComplete();
        }

        private void OnCompleteOperation(AsyncOperationHandle<T> operationHandle) {
            if (operationHandle.Status == AsyncOperationStatus.Failed) {
                NotifyFail();
                return;
            }

            SetResultAndComplete(operationHandle.Result);
        }
    }
}
#endif