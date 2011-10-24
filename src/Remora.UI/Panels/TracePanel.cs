using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Remora.UI.Trace;

namespace Remora.UI.Panels
{
    public partial class TracePanel : UserControl, ICommandNotifier
    {
        public TracePanel()
        {
            InitializeComponent();
        }

        public DirectoryIndex DirectoryIndex { get; set; }

        public void CommandFinished(ICommand cmd)
        {
            _labelNothing.Visible = (DirectoryIndex == null);
            _dataGrid.Visible = (DirectoryIndex != null);
        }
    }
}
