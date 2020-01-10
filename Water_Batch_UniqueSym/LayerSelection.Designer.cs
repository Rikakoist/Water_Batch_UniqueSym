namespace Water_Batch_UniqueSym
{
    partial class LayerSelection
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
            this.LayerSelectionCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.SelectNoneButton = new System.Windows.Forms.Button();
            this.SelectInverseButton = new System.Windows.Forms.Button();
            this.AbortButton = new System.Windows.Forms.Button();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LayerSelectionCheckedListBox
            // 
            this.LayerSelectionCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LayerSelectionCheckedListBox.CheckOnClick = true;
            this.LayerSelectionCheckedListBox.FormattingEnabled = true;
            this.LayerSelectionCheckedListBox.Location = new System.Drawing.Point(12, 60);
            this.LayerSelectionCheckedListBox.Name = "LayerSelectionCheckedListBox";
            this.LayerSelectionCheckedListBox.Size = new System.Drawing.Size(460, 424);
            this.LayerSelectionCheckedListBox.TabIndex = 5;
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectAllButton.Location = new System.Drawing.Point(12, 12);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(78, 33);
            this.SelectAllButton.TabIndex = 2;
            this.SelectAllButton.Text = "全选";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.ChangeSelection);
            // 
            // SelectNoneButton
            // 
            this.SelectNoneButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectNoneButton.Location = new System.Drawing.Point(96, 12);
            this.SelectNoneButton.Name = "SelectNoneButton";
            this.SelectNoneButton.Size = new System.Drawing.Size(78, 33);
            this.SelectNoneButton.TabIndex = 3;
            this.SelectNoneButton.Text = "全不选";
            this.SelectNoneButton.UseVisualStyleBackColor = true;
            this.SelectNoneButton.Click += new System.EventHandler(this.ChangeSelection);
            // 
            // SelectInverseButton
            // 
            this.SelectInverseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectInverseButton.Location = new System.Drawing.Point(180, 12);
            this.SelectInverseButton.Name = "SelectInverseButton";
            this.SelectInverseButton.Size = new System.Drawing.Size(78, 33);
            this.SelectInverseButton.TabIndex = 4;
            this.SelectInverseButton.Text = "反选";
            this.SelectInverseButton.UseVisualStyleBackColor = true;
            this.SelectInverseButton.Click += new System.EventHandler(this.ChangeSelection);
            // 
            // AbortButton
            // 
            this.AbortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AbortButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AbortButton.Location = new System.Drawing.Point(400, 521);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(72, 28);
            this.AbortButton.TabIndex = 1;
            this.AbortButton.Text = "取消";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfirmButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConfirmButton.Location = new System.Drawing.Point(310, 521);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(72, 28);
            this.ConfirmButton.TabIndex = 0;
            this.ConfirmButton.Text = "确定";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // LayerSelection
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.AbortButton);
            this.Controls.Add(this.ConfirmButton);
            this.Controls.Add(this.SelectInverseButton);
            this.Controls.Add(this.SelectNoneButton);
            this.Controls.Add(this.SelectAllButton);
            this.Controls.Add(this.LayerSelectionCheckedListBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(500, 350);
            this.Name = "LayerSelection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择要进行唯一值化的图层";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox LayerSelectionCheckedListBox;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button SelectNoneButton;
        private System.Windows.Forms.Button SelectInverseButton;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button ConfirmButton;
    }
}