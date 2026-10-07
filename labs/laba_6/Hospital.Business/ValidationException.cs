namespace Hospital.Business;

public sealed class ValidationException(string message) : Exception(message);
