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
            _labelLoading.Left = (Width - _labelLoading.Width)/2;
            _progressBar.Maximum = total;
            _progressBar.Value = current;
            _progressBar.Text = string.Format("{0} / {1}", current, total);
        }
    }
}
