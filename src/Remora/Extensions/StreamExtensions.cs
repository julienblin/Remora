using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Remora.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadFully(this Stream stream)
        {
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}
