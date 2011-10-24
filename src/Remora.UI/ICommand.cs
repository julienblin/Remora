using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.UI
{
    public interface ICommand
    {
        uint CommandId { get; }

        Type RibbonControlType { get; }

        Type TargetPanelType { get; }

        object TargetPanel { set; }

        int ExecuteSync(int applicationMode);

        int Execute(int applicationMode);

        event EventHandler<CommandReportProgressEventArgs> ReportProgress;
    }
}
