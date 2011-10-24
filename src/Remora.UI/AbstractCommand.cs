using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.UI
{
    public abstract class AbstractCommand<TCmdType, TPanel> : ICommand
    {
        protected AbstractCommand(uint commandId)
        {
            CommandId = commandId;
        }

        public uint CommandId { get; private set; }

        public Type RibbonControlType { get { return typeof (TCmdType); } }

        public Type TargetPanelType { get { return typeof(TPanel); } }

        public object TargetPanel { get; set; }

        protected TPanel CastTargetPanel { get { return (TPanel)TargetPanel; } }

        public abstract int ExecuteSync(int applicationMode);

        public abstract int Execute(int applicationMode);

        public event EventHandler<CommandReportProgressEventArgs> ReportProgress;

        protected void InvokeReportProgress(string message, int current, int total)
        {
            if (ReportProgress == null) return;

            var eventArgs = new CommandReportProgressEventArgs(message, current, total);
            ReportProgress(this, eventArgs);
        }
    }
}
