namespace PicPreview
{
    partial class ConfigDialog
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
            this.btnAssociate = new System.Windows.Forms.Button();
            this.gbxRendering = new System.Windows.Forms.GroupBox();
            this.gbxIntegration = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbxMaximizeFilter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxMinimizeFilter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxFastRenderInteraction = new System.Windows.Forms.CheckBox();
            this.cbxFastRenderAnimations = new System.Windows.Forms.CheckBox();
            this.cbxRenderTransparencyGrid = new System.Windows.Forms.CheckBox();
            this.gbxRendering.SuspendLayout();
            this.gbxIntegration.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAssociate
            // 
            this.btnAssociate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssociate.Location = new System.Drawing.Point(6, 19);
            this.btnAssociate.Name = "btnAssociate";
            this.btnAssociate.Size = new System.Drawing.Size(309, 23);
            this.btnAssociate.TabIndex = 0;
            this.btnAssociate.Text = "Associate File Extensions with PicPreview";
            this.btnAssociate.UseVisualStyleBackColor = true;
            this.btnAssociate.Click += new System.EventHandler(this.btnAssociate_Click);
            // 
            // gbxRendering
            // 
            this.gbxRendering.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxRendering.Controls.Add(this.cbxFastRenderAnimations);
            this.gbxRendering.Controls.Add(this.cbxRenderTransparencyGrid);
            this.gbxRendering.Controls.Add(this.cbxFastRenderInteraction);
            this.gbxRendering.Controls.Add(this.label2);
            this.gbxRendering.Controls.Add(this.label1);
            this.gbxRendering.Controls.Add(this.cbxMinimizeFilter);
            this.gbxRendering.Controls.Add(this.cbxMaximizeFilter);
            this.gbxRendering.Location = new System.Drawing.Point(12, 12);
            this.gbxRendering.Name = "gbxRendering";
            this.gbxRendering.Size = new System.Drawing.Size(321, 152);
            this.gbxRendering.TabIndex = 1;
            this.gbxRendering.TabStop = false;
            this.gbxRendering.Text = "Rendering";
            // 
            // gbxIntegration
            // 
            this.gbxIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxIntegration.Controls.Add(this.btnAssociate);
            this.gbxIntegration.Location = new System.Drawing.Point(12, 170);
            this.gbxIntegration.Name = "gbxIntegration";
            this.gbxIntegration.Size = new System.Drawing.Size(321, 48);
            this.gbxIntegration.TabIndex = 2;
            this.gbxIntegration.TabStop = false;
            this.gbxIntegration.Text = "Windows Integration";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(258, 230);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(177, 230);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbxMaximizeFilter
            // 
            this.cbxMaximizeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMaximizeFilter.FormattingEnabled = true;
            this.cbxMaximizeFilter.Items.AddRange(new object[] {
            "Nearest Neighbor (Fastest)",
            "Linear",
            "Cubic (Nicest)"});
            this.cbxMaximizeFilter.Location = new System.Drawing.Point(130, 19);
            this.cbxMaximizeFilter.Name = "cbxMaximizeFilter";
            this.cbxMaximizeFilter.Size = new System.Drawing.Size(185, 21);
            this.cbxMaximizeFilter.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Maximize Filter:";
            // 
            // cbxMinimizeFilter
            // 
            this.cbxMinimizeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMinimizeFilter.FormattingEnabled = true;
            this.cbxMinimizeFilter.Items.AddRange(new object[] {
            "Nearest Neighbor (Fastest)",
            "Linear",
            "Cubic (Nicest)"});
            this.cbxMinimizeFilter.Location = new System.Drawing.Point(130, 46);
            this.cbxMinimizeFilter.Name = "cbxMinimizeFilter";
            this.cbxMinimizeFilter.Size = new System.Drawing.Size(185, 21);
            this.cbxMinimizeFilter.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minimize Filter";
            // 
            // cbxFastRenderInteraction
            // 
            this.cbxFastRenderInteraction.AutoSize = true;
            this.cbxFastRenderInteraction.Location = new System.Drawing.Point(9, 102);
            this.cbxFastRenderInteraction.Name = "cbxFastRenderInteraction";
            this.cbxFastRenderInteraction.Size = new System.Drawing.Size(143, 17);
            this.cbxFastRenderInteraction.TabIndex = 4;
            this.cbxFastRenderInteraction.Text = "Fast rendered interaction";
            this.cbxFastRenderInteraction.UseVisualStyleBackColor = true;
            // 
            // cbxFastRenderAnimations
            // 
            this.cbxFastRenderAnimations.AutoSize = true;
            this.cbxFastRenderAnimations.Location = new System.Drawing.Point(9, 75);
            this.cbxFastRenderAnimations.Name = "cbxFastRenderAnimations";
            this.cbxFastRenderAnimations.Size = new System.Drawing.Size(144, 17);
            this.cbxFastRenderAnimations.TabIndex = 4;
            this.cbxFastRenderAnimations.Text = "Fast rendered animations";
            this.cbxFastRenderAnimations.UseVisualStyleBackColor = true;
            // 
            // cbxRenderTransparencyGrid
            // 
            this.cbxRenderTransparencyGrid.AutoSize = true;
            this.cbxRenderTransparencyGrid.Location = new System.Drawing.Point(9, 129);
            this.cbxRenderTransparencyGrid.Name = "cbxRenderTransparencyGrid";
            this.cbxRenderTransparencyGrid.Size = new System.Drawing.Size(145, 17);
            this.cbxRenderTransparencyGrid.TabIndex = 4;
            this.cbxRenderTransparencyGrid.Text = "Render transparency grid";
            this.cbxRenderTransparencyGrid.UseVisualStyleBackColor = true;
            // 
            // ConfigDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(345, 265);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbxIntegration);
            this.Controls.Add(this.gbxRendering);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.gbxRendering.ResumeLayout(false);
            this.gbxRendering.PerformLayout();
            this.gbxIntegration.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAssociate;
        private System.Windows.Forms.GroupBox gbxRendering;
        private System.Windows.Forms.GroupBox gbxIntegration;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbxMaximizeFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxMinimizeFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbxFastRenderInteraction;
        private System.Windows.Forms.CheckBox cbxFastRenderAnimations;
        private System.Windows.Forms.CheckBox cbxRenderTransparencyGrid;
    }
}