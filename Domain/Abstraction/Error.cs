namespace Domain.Abstraction;

public record Error(string code, string message)
{
    public static Error None = new Error(string.Empty, string.Empty);

    public static Error NullValue = new Error("Error.NullValue", "Null value was provided");
    public static Error DataBaseError = new Error("Error.DataBaseError", "There is some problem with the DataBase");
    public static Error BadRequest = new Error("Error.BadRequest", "Somewhere the query didn't work");
    public static Error NotFound = new Error("Error.NotFound", "NotFound");
    public static Error Unauthorized = new Error("Error.Unauthorized", "Unauthorized");
    public static Error Forbidden = new Error("Error.Forbidden", "Forbidden");
    public static Error Conflict = new Error("Error.Conflict", "Conflict");
    public static Error InternalServerError = new Error("Error.InternalServerError", "Internal Server Error");
    public static Error LoginFaild = new Error("Error.LoginFaild", "Username or Password is incorrect");
    public static Error TechnicalWorks = new Error("Error.TechnicalWorks", "Ushbu Apida texnik ishlar olib borilmoqda");
    public static Error DatabaseError = new Error("Error.DatabaseError", "Database error");
    public static Error InvalidOperation = new Error("Error.InvalidOperation", "Invalid operation");
    public static Error TimeSlotOverlap = new Error("Error.TimeSlotOverlap", "Time slot overlaps with existing appointment");
    public static Error ScheduleHasAppointments = new Error("Error.ScheduleHasAppointments", "Schedule has appointments");
    public static Error TimeSlotNotAvailable = new Error("Error.TimeSlotNotAvailable", "Time slot is not available");

    // Wallet specific errors
    public static Error DuplicateWallet = new Error("Error.DuplicateWallet", "User already has a wallet");
    public static Error InvalidCardNumber = new Error("Error.InvalidCardNumber", "Invalid card number");
    public static Error ExpiredCard = new Error("Error.ExpiredCard", "Card has expired");
    public static Error InsufficientFunds = new Error("Error.InsufficientFunds", "Insufficient funds in wallet");
    public static Error InvalidAmount = new Error("Error.InvalidAmount", "Invalid amount specified");
    public static Error WalletNotFound = new Error("Error.WalletNotFound", "Wallet not found");
    public static Error TransferFailed = new Error("Error.TransferFailed", "Transfer operation failed");
    public static Error InvalidCurrency = new Error("Error.InvalidCurrency", "Invalid currency specified");
    public static Error CardholderNameRequired = new Error("Error.CardholderNameRequired", "Cardholder name is required");
    public static Error InvalidExpiryDate = new Error("Error.InvalidExpiryDate", "Invalid expiry date");
    public static Error WalletLocked = new Error("Error.WalletLocked", "Wallet is locked");
    public static Error InvalidTransactionAmount = new Error("Error.InvalidTransactionAmount", "Invalid transaction amount");
    public static Error SameWalletTransfer = new Error("Error.SameWalletTransfer", "Cannot transfer to same wallet");
    public static Error MaxBalanceExceeded = new Error("Error.MaxBalanceExceeded", "Maximum balance limit exceeded");
    public static Error MinBalanceRequired = new Error("Error.MinBalanceRequired", "Minimum balance requirement not met");

    // Transaction specific errors
    public static Error TransactionNotFound = new Error("Error.TransactionNotFound", "Transaction not found");
    public static Error InvalidTransactionType = new Error("Error.InvalidTransactionType", "Invalid transaction type");
    public static Error TransactionFailed = new Error("Error.TransactionFailed", "Transaction failed");
    public static Error DuplicateTransaction = new Error("Error.DuplicateTransaction", "Duplicate transaction detected");
    public static Error TransactionTimeout = new Error("Error.TransactionTimeout", "Transaction timed out");
    public static Error InvalidTransactionStatus = new Error("Error.InvalidTransactionStatus", "Invalid transaction status");
    public static Error TransactionLimitExceeded = new Error("Error.TransactionLimitExceeded", "Transaction limit exceeded");
    public static Error InvalidDateRange = new Error("Error.InvalidDateRange", "Invalid date range for transaction search");
    public static Error TransactionProcessing = new Error("Error.TransactionProcessing", "Transaction is still processing");
    public static Error InvalidTransactionId = new Error("Error.InvalidTransactionId", "Invalid transaction ID");
    public static Error InvalidTransaction = new Error("Error.InvalidTransaction", "Invalid transaction");
    public static Error InsufficientBalance = new Error("Error.InsufficientBalance", "Insufficient Balance");
    public static Error WithdrawalFailed = new Error("Error.WithdrawalFailed", "Withdrawal Failed");
    public static Error SameCardTransfer = new Error("Error.SameCardTransfer", "Cannot transfer to the same card");
    public static Error InvalidInput = new Error("Error.InvalidInput", "Invalid input");    
}
