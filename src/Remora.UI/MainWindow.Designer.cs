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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 526);
            this.Controls.Add(this._ribbonControl);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private RibbonLib.Ribbon _ribbonControl;
    }
}