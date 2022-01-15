namespace Smr.Operations {
    public abstract class Operation {
        public bool CanExecute() {
            return !GetErrors().HasError;
        }

        public bool CanHandle() {
            return !GetErrors().HasUnhandledError;
        }

        public bool Execute() {
            var result = false;
            var errors = GetErrors();
            SetUp();

            if (!errors.HasError) {
                ExecuteInternal();
                result = true;
            } else if (!errors.HasUnhandledError) {
                errors.Handle();
            }

            TearDown();
            return result;
        }

        protected virtual void SetUp() {}

        protected virtual void TearDown() {}

        protected abstract void FillErrors(OperationErrors errors);
        protected abstract void ExecuteInternal();

        private OperationErrors GetErrors() {
            var errors = new OperationErrors();
            FillErrors(errors);
            return errors;
        }
    }
}