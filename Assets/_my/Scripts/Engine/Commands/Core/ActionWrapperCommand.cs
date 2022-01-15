using System;

namespace Smr.Commands {
    public class ActionWrapperCommand : AbstractCommand {
        private readonly Action _action;

        public ActionWrapperCommand(Action action) {
            _action = action;
        }

        protected override void ExecuteInternal() {
            _action?.Invoke();
            NotifyComplete();
        }
    }
}