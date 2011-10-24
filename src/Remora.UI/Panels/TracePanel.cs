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
    public partial class TracePanel : UserControl
    {
        public TracePanel()
        {
            InitializeComponent();
        }

        public DirectoryIndex DirectoryIndex { get; set; }
    }
}
