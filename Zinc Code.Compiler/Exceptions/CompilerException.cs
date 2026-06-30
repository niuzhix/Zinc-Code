namespace Zinc_Code.Compiler.Exceptions;

public class CompilerException : Exception
{
    public CompilerException() { }
    public CompilerException(string message) : base(message) { }
    public CompilerException(string message, Exception inner) : base(message, inner) { }
}