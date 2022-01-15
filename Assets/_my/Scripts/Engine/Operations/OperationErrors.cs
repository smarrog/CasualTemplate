using System;
using System.Collections.Generic;

namespace Smr.Operations {
    public class OperationErrors {
        private readonly List<OperationError> _unhandledErrors = new List<OperationError>();
        private readonly List<OperationErrorWithHandler> _handledErrors = new List<OperationErrorWithHandler>();

        public bool HasUnhandledError => _unhandledErrors.Count > 0;
        public bool HasError => _unhandledErrors.Count > 0 || _handledErrors.Count > 0;

        public void AddError(OperationError error, Action<OperationError> handler = null) {
            if (handler == null) {
                _unhandledErrors.Add(error);
            } else {
                _handledErrors.Add(new OperationErrorWithHandler(error, handler));
            }
        }

        public void AddErrors(OperationErrors errors) {
            foreach (var error in errors._unhandledErrors) {
                AddError(error);
            }
            foreach (var errorWrapper in errors._handledErrors) {
                AddError(errorWrapper.Error, errorWrapper.Handler);
            }
        }

        public void Handle() {
            if (_handledErrors.Count > 0) {
                _handledErrors[0].Call();
            }
        }

        public override string ToString() {
            if (!HasError) {
                return "No errors";
            }
            return "Unhandled errors: " + _unhandledErrors.Count + " Handled Errors: " + _handledErrors.Count;
        }
    }

    internal class OperationErrorWithHandler {
        public OperationError Error { get; }
        public Action<OperationError> Handler { get; }

        public OperationErrorWithHandler(OperationError error, Action<OperationError> handler = null) {
            Error = error;
            Handler = handler;
        }

        public void Call() {
            Handler?.Invoke(Error);
        }
    }
}