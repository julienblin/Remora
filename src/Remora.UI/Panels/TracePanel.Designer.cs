namespace Remora.UI.Panels
{
    partial class TracePanel
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
            this._labelNothing = new System.Windows.Forms.Label();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._dataGrid = new System.Windows.Forms.DataGridView();
            this.OperationId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _labelNothing
            // 
            this._labelNothing.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._labelNothing.AutoSize = true;
            this._labelNothing.Location = new System.Drawing.Point(179, 167);
            this._labelNothing.Name = "_labelNothing";
            this._labelNothing.Size = new System.Drawing.Size(241, 13);
            this._labelNothing.TabIndex = 0;
            this._labelNothing.Text = "Use the open command to load a set of trace files";
            // 
            // _dataGrid
            // 
            this._dataGrid.AllowUserToAddRows = false;
            this._dataGrid.AllowUserToDeleteRows = false;
            this._dataGrid.AllowUserToOrderColumns = true;
            this._dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OperationId});
            this._dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGrid.Location = new System.Drawing.Point(0, 0);
            this._dataGrid.Name = "_dataGrid";
            this._dataGrid.ReadOnly = true;
            this._dataGrid.Size = new System.Drawing.Size(596, 369);
            this._dataGrid.TabIndex = 1;
            this._dataGrid.Visible = false;
            // 
            // OperationId
            // 
            this.OperationId.HeaderText = "OperationId";
            this.OperationId.Name = "OperationId";
            this.OperationId.ReadOnly = true;
            // 
            // TracePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dataGrid);
            this.Controls.Add(this._labelNothing);
            this.Name = "TracePanel";
            this.Size = new System.Drawing.Size(596, 369);
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelNothing;
        public System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.DataGridView _dataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperationId;
    }
}
