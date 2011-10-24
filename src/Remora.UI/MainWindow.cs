using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Castle.MicroKernel;
using Castle.Windsor;
using Remora.UI.Panels;
using RibbonLib.Controls;
using RibbonLib.Controls.Events;

namespace Remora.UI
{
    public partial class MainWindow : Form
    {
        public IKernel Kernel { get; set; }

        public int CurrentApplicationMode { get; set; }

        private List<Control> _panels = new List<Control>();

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            var allcommands = Kernel.ResolveAll<ICommand>();

            foreach (var command in allcommands)
            {
                var ctrl = (IExecuteEventsProvider)Activator.CreateInstance(command.RibbonControlType, _ribbonControl, command.CommandId);
                var cmdLocalCopy = command;
                ctrl.ExecuteEvent += (sender, evt) =>
                {
                    var cmd = (ICommand)Kernel.Resolve(cmdLocalCopy.GetType());
                    cmd.TargetPanel = FindOrCreatePanel(cmd.TargetPanelType, true);
                    SetApplicationMode(cmd.ExecuteSync(CurrentApplicationMode));
                    _backgroundWorker.RunWorkerAsync(cmd);
                };
            }
        }

        public Control FindOrCreatePanel(Type panelType, bool display = false)
        {
            var panel = _panels.Where(control => control.GetType() == panelType).FirstOrDefault();

            if (panel == null)
            {
                panel = (Control)Activator.CreateInstance(panelType);
                _panels.Add(panel);
            }

            if (display && ((_tableLayoutPanel.Controls.Count == 0) || (_tableLayoutPanel.Controls[0] != panel)))
            { 
                _tableLayoutPanel.Controls.Clear();
                _tableLayoutPanel.Controls.Add(panel, 0, 0);
                panel.Dock = DockStyle.Fill;
            }

            return panel;
        }

        private void SetApplicationMode(int newApplicationMode)
        {
            if (newApplicationMode != CurrentApplicationMode)
            {
                _ribbonControl.SetModes(Convert.ToByte(newApplicationMode));
                CurrentApplicationMode = newApplicationMode;
            }
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var cmd = (ICommand)e.Argument;
            cmd.ReportProgress += (progressSender, progressE) => _backgroundWorker.ReportProgress(progressE.Current, progressE);
            cmd.TargetPanel = FindOrCreatePanel(cmd.TargetPanelType);
            var newApplicationMode = cmd.Execute(CurrentApplicationMode);
            e.Result = new KeyValuePair<ICommand, int>(cmd, newApplicationMode);
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var loaderPanel = (Loader)FindOrCreatePanel(typeof(Loader), true);
            var args = (CommandReportProgressEventArgs)e.UserState;
            loaderPanel.ReportProgress(args.Message, args.Current, args.Total);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (KeyValuePair<ICommand, int>) e.Result;

            SetApplicationMode(result.Value);

            FindOrCreatePanel(result.Key.TargetPanelType, true);
        }
    }
}
