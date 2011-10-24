using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Remora.UI.Panels;
using Remora.UI.Trace;
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
                CastTargetPanel.DirectoryIndex = new DirectoryIndex();
                CastTargetPanel.DirectoryIndex.AddToIndex(CastTargetPanel.FolderBrowserDialog.SelectedPath, (msg, traceFile,current, total) => {
                    InvokeReportProgress(string.Format("Loading {0}...", traceFile), current, total);
                });
                return (int) ApplicationModes.DirectoryIndexed;
            }

            return applicationMode;
        }
    }
}
