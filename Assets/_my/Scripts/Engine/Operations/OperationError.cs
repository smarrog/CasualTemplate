namespace Smr.Operations {
    public class OperationError {
        public static readonly OperationError EmptyError = new OperationError();

        public string Message { get; }

        public OperationError(string message = null) {
            Message = message;
        }
    }
}