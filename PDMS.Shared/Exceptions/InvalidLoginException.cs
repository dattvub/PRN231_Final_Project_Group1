namespace PDMS.Shared.Exceptions;

public class InvalidLoginException : Exception {
    public InvalidLoginException(string message = "Email and password is not match any user") : base(message) { }
}