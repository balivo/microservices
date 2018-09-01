using System;

namespace Balivo.Microservices.Messages
{
    public abstract class MessageBase
    {
        public string SystemKey { get; set; }
        public string Message { get; set; }

        public MessageBase()
        {

        }

        public MessageBase(string systemKey, string message) : this()
        {
            if (string.IsNullOrWhiteSpace(systemKey))
                throw new ArgumentNullException(nameof(systemKey));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            SystemKey = systemKey;
            Message = message;
        }
    }
}
