using System.Runtime.Serialization;

namespace EasyApiSecurity.Core
{
    [Serializable]
    internal class JwtProviderNotInizializedException : Exception
    {
        public JwtProviderNotInizializedException()
        {
        }

        public JwtProviderNotInizializedException(string? message) : base(message)
        {
        }

        public JwtProviderNotInizializedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected JwtProviderNotInizializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}