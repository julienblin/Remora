using System.IO;
using Remora.Exceptions;

namespace Remora.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadFully(this Stream stream, int maxMessageSize)
        {
            var buffer = new byte[32768];
            var currentMessageSize = 0;
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();

                    currentMessageSize += read;

                    if ((maxMessageSize > 0) && (currentMessageSize > maxMessageSize))
                        throw new MaxMessageSizeException(string.Format("The maximum message size has been reached. Current value: {0}. Adjust your configuration settings if needed.", maxMessageSize));

                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}
