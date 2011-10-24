namespace Remora.UI
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._ribbonControl = new RibbonLib.Ribbon();
            this._panelContent = new System.Windows.Forms.Panel();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._panelContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ribbonControl
            // 
            this._ribbonControl.Location = new System.Drawing.Point(0, 0);
            this._ribbonControl.Minimized = false;
            this._ribbonControl.Name = "_ribbonControl";
            this._ribbonControl.ResourceName = "Remora.UI.Ribbon.ribbon";
            this._ribbonControl.ShortcutTableResourceName = null;
            this._ribbonControl.Size = new System.Drawing.Size(761, 118);
            this._ribbonControl.TabIndex = 0;
            // 
            // _panelContent
            // 
            this._panelContent.Controls.Add(this._tableLayoutPanel);
            this._panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelContent.Location = new System.Drawing.Point(0, 118);
            this._panelContent.Name = "_panelContent";
            this._panelContent.Size = new System.Drawing.Size(761, 408);
            this._panelContent.TabIndex = 1;
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 1;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 1;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(761, 408);
            this._tableLayoutPanel.TabIndex = 0;
            // 
            // _backgroundWorker
            // 
            this._backgroundWorker.WorkerReportsProgress = true;
            this._backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._backgroundWorker_DoWork);
            this._backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._backgroundWorker_ProgressChanged);
            this._backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._backgroundWorker_RunWorkerCompleted);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 526);
            this.Controls.Add(this._panelContent);
            this.Controls.Add(this._ribbonControl);
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "Remora";
            this._panelContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RibbonLib.Ribbon _ribbonControl;
        private System.Windows.Forms.Panel _panelContent;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
    }
}