namespace PicPreview
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbZoomModeFit = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbZoomModeOriginal = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbPrevious = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbNext = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsbZoomModeFit,
            this.tsbZoomModeOriginal,
            this.toolStripStatusLabel3,
            this.tsbPrevious,
            this.tsbNext,
            this.toolStripStatusLabel2});
            this.statusStrip.Location = new System.Drawing.Point(0, 537);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(774, 61);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(290, 56);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // tsbZoomModeFit
            // 
            this.tsbZoomModeFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomModeFit.DropDownButtonWidth = 0;
            this.tsbZoomModeFit.Image = global::PicPreview.Properties.Resources.Conflicts_large__11131;
            this.tsbZoomModeFit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbZoomModeFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbZoomModeFit.Name = "tsbZoomModeFit";
            this.tsbZoomModeFit.Size = new System.Drawing.Size(37, 59);
            this.tsbZoomModeFit.Text = "Fit to Window";
            this.tsbZoomModeFit.ButtonClick += new System.EventHandler(this.tsbZoomModeFit_ButtonClick);
            // 
            // tsbZoomModeOriginal
            // 
            this.tsbZoomModeOriginal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomModeOriginal.DropDownButtonWidth = 0;
            this.tsbZoomModeOriginal.Image = global::PicPreview.Properties.Resources.resource_32xLG;
            this.tsbZoomModeOriginal.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbZoomModeOriginal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbZoomModeOriginal.Name = "tsbZoomModeOriginal";
            this.tsbZoomModeOriginal.Size = new System.Drawing.Size(37, 59);
            this.tsbZoomModeOriginal.Text = "Original Size";
            this.tsbZoomModeOriginal.ButtonClick += new System.EventHandler(this.tsbZoomModeOriginal_ButtonClick);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 8);
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(31, 50);
            this.toolStripStatusLabel3.Text = "|";
            // 
            // tsbPrevious
            // 
            this.tsbPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPrevious.DropDownButtonWidth = 0;
            this.tsbPrevious.Image = global::PicPreview.Properties.Resources.arrow_back_color_32xLG;
            this.tsbPrevious.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPrevious.Name = "tsbPrevious";
            this.tsbPrevious.Size = new System.Drawing.Size(37, 59);
            this.tsbPrevious.Text = "toolStripSplitButton1";
            // 
            // tsbNext
            // 
            this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNext.DropDownButtonWidth = 0;
            this.tsbNext.Image = global::PicPreview.Properties.Resources.arrow_Forward_color_32xLG;
            this.tsbNext.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(37, 59);
            this.tsbNext.Text = "toolStripSplitButton1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(290, 56);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(774, 598);
            this.Controls.Add(this.statusStrip);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "PicPreview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSplitButton tsbPrevious;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripSplitButton tsbZoomModeFit;
        private System.Windows.Forms.ToolStripSplitButton tsbZoomModeOriginal;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripSplitButton tsbNext;
    }
}

