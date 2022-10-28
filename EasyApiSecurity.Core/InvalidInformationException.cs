using System.Runtime.Serialization;

namespace EasyApiSecurity.Core
{
    [Serializable]
    internal class InvalidInformationException : Exception
    {
        public InvalidInformationException()
        {
        }

        public InvalidInformationException(string? message) : base(message)
        {
        }

        public InvalidInformationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidInformationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}