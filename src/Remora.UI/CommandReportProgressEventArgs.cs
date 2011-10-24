using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.UI
{
    public class CommandReportProgressEventArgs : EventArgs
    {
        public CommandReportProgressEventArgs(string message, int current, int total)
        {
            Message = message;
            Current = current;
            Total = total;
        }

        public string Message { get; private set; }
        public int Current { get; private set; }
        public int Total { get; private set; }
    }
}
