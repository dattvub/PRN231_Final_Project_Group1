namespace PDMS.Shared.Exceptions;

public class InvalidTokenException : Exception {
    public InvalidTokenException(string message = "Invalid token") : base(message) { }
}