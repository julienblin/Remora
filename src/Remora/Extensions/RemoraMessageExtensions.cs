using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Extensions
{
    public static class RemoraMessageExtensions
    {
        public static string GetDataAsString(this IRemoraMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Contract.EndContractBlock();

            return message.ContentEncoding.GetString(message.Data);
        }

        public static void SetData(this IRemoraMessage message, string data)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (data == null) throw new ArgumentNullException("data");
            Contract.EndContractBlock();

            message.Data = message.ContentEncoding.GetBytes(data);
        }
    }
}
