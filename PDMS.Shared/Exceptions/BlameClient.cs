namespace PDMS.Shared.Exceptions;

public class BlameClient : Exception {
    public BlameClient() { }

    public BlameClient(string? message) : base(message) { }

    public BlameClient(string? message, Exception innerException) : base(message, innerException) { }
}