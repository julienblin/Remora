using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Remora.UI.Panels;
using RibbonLib.Controls;

namespace Remora.UI.Commands
{
    public class OpenTraceDirectoryCommand : AbstractCommand<RibbonButton, TracePanel>
    {
        public OpenTraceDirectoryCommand() : base(1111) {}

        public override int ExecuteSync(int applicationMode)
        {
            if (CastTargetPanel.FolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                return (int)ApplicationModes.DirectoryIndexing;
            }
            return applicationMode;
        }

        public override int Execute(int applicationMode)
        {
            if (applicationMode == (int)ApplicationModes.DirectoryIndexing)
            {
                for (var i = 0; i < 100; i++)
                {
                    Thread.Sleep(100);
                    InvokeReportProgress("Better...", i, 100);
                }

                return (int) ApplicationModes.DirectoryIndexed;
            }

            return applicationMode;
        }
    }
}
