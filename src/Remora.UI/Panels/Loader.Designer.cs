namespace Remora.UI.Panels
{
    partial class Loader
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._labelLoading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._progressBar.Location = new System.Drawing.Point(105, 134);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(218, 23);
            this._progressBar.TabIndex = 0;
            // 
            // _labelLoading
            // 
            this._labelLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._labelLoading.AutoSize = true;
            this._labelLoading.Location = new System.Drawing.Point(185, 115);
            this._labelLoading.Name = "_labelLoading";
            this._labelLoading.Size = new System.Drawing.Size(54, 13);
            this._labelLoading.TabIndex = 1;
            this._labelLoading.Text = "Loading...";
            // 
            // Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._labelLoading);
            this.Controls.Add(this._progressBar);
            this.Name = "Loader";
            this.Size = new System.Drawing.Size(428, 291);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Label _labelLoading;
    }
}
