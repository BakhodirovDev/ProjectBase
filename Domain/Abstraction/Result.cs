namespace Domain.Abstraction
{
    // Result class representing success or failure.
    public class Result
    {
        protected internal Result(bool isSuccess, string? successMessage, Error? error)
        {
            // Check for valid success and error combination
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Success result cannot have an error.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Failure result must have an error.");

            IsSuccess = isSuccess;
            SuccessMessage = successMessage;
            Error = error ?? Error.None;  // Ensure error is never null
        }

        public bool IsSuccess { get; private set; }
        public string? SuccessMessage { get; private set; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; private set; }

        // Create a successful result with no message
        public static Result Success() => new Result(true, string.Empty, null);

        // Create a successful result
        public static Result Success(string successMessage = null) => new Result(true, successMessage, null);
        
        // Create a failure result with no message
        public static Result Failure() => new Result(false, string.Empty, Error.None);

        // Create a failure result with an error
        public static Result Failure(Error error) => new Result(false, string.Empty, error);
        
        // Create a failure result with an error
        public static Result Failure(Error error, string failureMessage) => new Result(false, failureMessage, error);

    }

    // Result<TValue> class for handling success or failure with a value
    public class Result<TValue> : Result
    {
        private TValue? _value;

        protected internal Result(TValue? value, string? successMessage, bool isSuccess, Error? error)
            : base(isSuccess, successMessage, error)
        {
            _value = value;
        }

        // The value of a successful result
        public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

        // Sort method for collections
        public void Sort(Func<TValue, TValue, int> comparator)
        {
            if (_value is IEnumerable<TValue> collection)
            {
                var sortedCollection = collection.OrderBy(item => item, new ComparisonComparer(comparator));
                _value = (TValue)(object)sortedCollection.ToList(); // Using List for sorting
            }
            else
            {
                throw new InvalidOperationException("Value must be a collection to sort.");
            }
        }

        // Create a successful result with a value
        public static Result<TValue> Success(TValue value, string? successMessage = null)
            => new Result<TValue>(value, successMessage, true, Error.None);

        // Create a failure result with an error
        public static Result<TValue> Failure(Error error, string? failureMessage = null)
            => new Result<TValue>(default, failureMessage, false, error);

        // Create a Result<TValue> from a nullable value
        public static Result<TValue> Create(TValue? value)
            => value != null ? Success(value) : Failure(Error.NullValue);

        // Implicit conversion operator to allow direct conversion from TValue to Result<TValue>
        public static implicit operator Result<TValue>(TValue? value) => Create(value);

        // Helper class for comparison
        private class ComparisonComparer : IComparer<TValue>
        {
            private readonly Func<TValue, TValue, int> _comparator;

            public ComparisonComparer(Func<TValue, TValue, int> comparator)
            {
                _comparator = comparator;
            }

            public int Compare(TValue x, TValue y) => _comparator(x, y);
        }
    }
}
