namespace PicPreview
{
    partial class LicensesDialog
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbxComponent = new System.Windows.Forms.GroupBox();
            this.lbxComponents = new System.Windows.Forms.ListBox();
            this.tbxLicense = new System.Windows.Forms.TextBox();
            this.linkProjectPage = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbxComponent.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbxComponent);
            this.splitContainer1.Size = new System.Drawing.Size(842, 540);
            this.splitContainer1.SplitterDistance = 242;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbxComponents);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 540);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Component";
            // 
            // gbxComponent
            // 
            this.gbxComponent.Controls.Add(this.label1);
            this.gbxComponent.Controls.Add(this.linkProjectPage);
            this.gbxComponent.Controls.Add(this.tbxLicense);
            this.gbxComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxComponent.Location = new System.Drawing.Point(0, 0);
            this.gbxComponent.Name = "gbxComponent";
            this.gbxComponent.Size = new System.Drawing.Size(596, 540);
            this.gbxComponent.TabIndex = 0;
            this.gbxComponent.TabStop = false;
            this.gbxComponent.Text = "License";
            // 
            // lbxComponents
            // 
            this.lbxComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxComponents.FormattingEnabled = true;
            this.lbxComponents.IntegralHeight = false;
            this.lbxComponents.Location = new System.Drawing.Point(3, 16);
            this.lbxComponents.Name = "lbxComponents";
            this.lbxComponents.Size = new System.Drawing.Size(236, 521);
            this.lbxComponents.TabIndex = 0;
            this.lbxComponents.SelectedIndexChanged += new System.EventHandler(this.lbxComponents_SelectedIndexChanged);
            // 
            // tbxLicense
            // 
            this.tbxLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxLicense.BackColor = System.Drawing.SystemColors.Window;
            this.tbxLicense.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxLicense.Location = new System.Drawing.Point(3, 41);
            this.tbxLicense.Multiline = true;
            this.tbxLicense.Name = "tbxLicense";
            this.tbxLicense.ReadOnly = true;
            this.tbxLicense.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxLicense.Size = new System.Drawing.Size(590, 496);
            this.tbxLicense.TabIndex = 0;
            this.tbxLicense.WordWrap = false;
            // 
            // linkProjectPage
            // 
            this.linkProjectPage.AutoSize = true;
            this.linkProjectPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkProjectPage.Location = new System.Drawing.Point(95, 17);
            this.linkProjectPage.Name = "linkProjectPage";
            this.linkProjectPage.Size = new System.Drawing.Size(11, 15);
            this.linkProjectPage.TabIndex = 1;
            this.linkProjectPage.TabStop = true;
            this.linkProjectPage.Text = "-";
            this.linkProjectPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkProjectPage_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Project URL:";
            // 
            // LicensesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 540);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "LicensesDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Licenses";
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.LicensesDialog_PreviewKeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.gbxComponent.ResumeLayout(false);
            this.gbxComponent.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbxComponent;
        private System.Windows.Forms.ListBox lbxComponents;
        private System.Windows.Forms.TextBox tbxLicense;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkProjectPage;
    }
}