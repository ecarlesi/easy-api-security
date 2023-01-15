using System.Runtime.Serialization;

namespace EasyApiSecurity.Core
{
    [Serializable]
    internal class JwtProviderNotInitializedException : Exception
    {
        public JwtProviderNotInitializedException()
        {
        }

        public JwtProviderNotInitializedException(string? message) : base(message)
        {
        }

        public JwtProviderNotInitializedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected JwtProviderNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}