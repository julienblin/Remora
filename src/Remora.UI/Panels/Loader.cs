using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Remora.UI.Panels
{
    public partial class Loader : UserControl
    {
        public Loader()
        {
            InitializeComponent();
        }

        public void ReportProgress(string message, int current, int total)
        {
            _labelLoading.Text = message;
            _progressBar.Maximum = total;
            _progressBar.Value = current;
        }
    }
}
