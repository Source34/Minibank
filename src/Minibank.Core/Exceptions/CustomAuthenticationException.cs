namespace Minibank.Core.Exceptions
{
    public class CustomAuthenticationException : Exception
    {
        public CustomAuthenticationException(string message) : base(message) { }
    }
}