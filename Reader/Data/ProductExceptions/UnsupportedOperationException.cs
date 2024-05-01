namespace Reader.Data.ProductExceptions;

public class UnsupportedOperationException : Exception
{
    public UnsupportedOperationException(string operation, string message) : base($"Unsupported Operation: {operation} ({message})")
    {

    }

    public UnsupportedOperationException(string operation) : base($"Unsupported Operation: {operation}")
    {

    }
}
