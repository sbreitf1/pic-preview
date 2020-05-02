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
            this.tsbOptions = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbZoomModeFit = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbZoomModeOriginal = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbPrevious = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbNext = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbRotateCCW = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbRotateCW = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbImageEffects = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbSave = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlAnimation = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnAnimation = new System.Windows.Forms.Button();
            this.tbrImageFrame = new System.Windows.Forms.TrackBar();
            this.statusStrip.SuspendLayout();
            this.pnlAnimation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbrImageFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsbOptions,
            this.toolStripStatusLabel5,
            this.tsbZoomModeFit,
            this.tsbZoomModeOriginal,
            this.toolStripStatusLabel3,
            this.tsbPrevious,
            this.tsbNext,
            this.toolStripStatusLabel4,
            this.tsbRotateCCW,
            this.tsbRotateCW,
            this.tsbImageEffects,
            this.tsbSave,
            this.toolStripStatusLabel2});
            this.statusStrip.Location = new System.Drawing.Point(0, 390);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(707, 61);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(123, 56);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // tsbOptions
            // 
            this.tsbOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOptions.DropDownButtonWidth = 0;
            this.tsbOptions.Image = global::PicPreview.Properties.Resources.gear_32xLG;
            this.tsbOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOptions.Name = "tsbOptions";
            this.tsbOptions.Size = new System.Drawing.Size(37, 59);
            this.tsbOptions.Text = "toolStripSplitButton1";
            this.tsbOptions.ButtonClick += new System.EventHandler(this.tsbOptions_ButtonClick);
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Margin = new System.Windows.Forms.Padding(0, 3, 0, 8);
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(31, 50);
            this.toolStripStatusLabel5.Text = "|";
            // 
            // tsbZoomModeFit
            // 
            this.tsbZoomModeFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomModeFit.DropDownButtonWidth = 0;
            this.tsbZoomModeFit.Enabled = false;
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
            this.tsbZoomModeOriginal.Enabled = false;
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
            this.tsbPrevious.Enabled = false;
            this.tsbPrevious.Image = global::PicPreview.Properties.Resources.arrow_back_color_32xLG;
            this.tsbPrevious.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPrevious.Name = "tsbPrevious";
            this.tsbPrevious.Size = new System.Drawing.Size(37, 59);
            this.tsbPrevious.Text = "toolStripSplitButton1";
            this.tsbPrevious.ButtonClick += new System.EventHandler(this.tsbPrevious_ButtonClick);
            // 
            // tsbNext
            // 
            this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNext.DropDownButtonWidth = 0;
            this.tsbNext.Enabled = false;
            this.tsbNext.Image = global::PicPreview.Properties.Resources.arrow_Forward_color_32xLG;
            this.tsbNext.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(37, 59);
            this.tsbNext.Text = "toolStripSplitButton1";
            this.tsbNext.ButtonClick += new System.EventHandler(this.tsbNext_ButtonClick);
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Margin = new System.Windows.Forms.Padding(0, 3, 0, 8);
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(31, 50);
            this.toolStripStatusLabel4.Text = "|";
            // 
            // tsbRotateCCW
            // 
            this.tsbRotateCCW.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRotateCCW.DropDownButtonWidth = 0;
            this.tsbRotateCCW.Enabled = false;
            this.tsbRotateCCW.Image = global::PicPreview.Properties.Resources.Refresh_32x_flipped;
            this.tsbRotateCCW.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbRotateCCW.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRotateCCW.Name = "tsbRotateCCW";
            this.tsbRotateCCW.Size = new System.Drawing.Size(37, 59);
            this.tsbRotateCCW.Text = "toolStripSplitButton1";
            // 
            // tsbRotateCW
            // 
            this.tsbRotateCW.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRotateCW.DropDownButtonWidth = 0;
            this.tsbRotateCW.Enabled = false;
            this.tsbRotateCW.Image = global::PicPreview.Properties.Resources.Refresh_32x;
            this.tsbRotateCW.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbRotateCW.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRotateCW.Name = "tsbRotateCW";
            this.tsbRotateCW.Size = new System.Drawing.Size(37, 59);
            this.tsbRotateCW.Text = "toolStripSplitButton1";
            // 
            // tsbImageEffects
            // 
            this.tsbImageEffects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbImageEffects.DropDownButtonWidth = 20;
            this.tsbImageEffects.Enabled = false;
            this.tsbImageEffects.Image = global::PicPreview.Properties.Resources.Edit_32xMD;
            this.tsbImageEffects.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbImageEffects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImageEffects.Name = "tsbImageEffects";
            this.tsbImageEffects.Size = new System.Drawing.Size(57, 59);
            this.tsbImageEffects.Text = "toolStripSplitButton1";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.DropDownButtonWidth = 0;
            this.tsbSave.Enabled = false;
            this.tsbSave.Image = global::PicPreview.Properties.Resources.Save_32x;
            this.tsbSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(37, 59);
            this.tsbSave.Text = "toolStripSplitButton1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(123, 56);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // pnlAnimation
            // 
            this.pnlAnimation.Controls.Add(this.splitContainer1);
            this.pnlAnimation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAnimation.Location = new System.Drawing.Point(0, 341);
            this.pnlAnimation.Name = "pnlAnimation";
            this.pnlAnimation.Size = new System.Drawing.Size(707, 49);
            this.pnlAnimation.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnAnimation);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbrImageFrame);
            this.splitContainer1.Size = new System.Drawing.Size(707, 49);
            this.splitContainer1.SplitterDistance = 54;
            this.splitContainer1.TabIndex = 1;
            // 
            // btnAnimation
            // 
            this.btnAnimation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAnimation.Image = global::PicPreview.Properties.Resources.Run_32x;
            this.btnAnimation.Location = new System.Drawing.Point(0, 0);
            this.btnAnimation.Name = "btnAnimation";
            this.btnAnimation.Size = new System.Drawing.Size(54, 49);
            this.btnAnimation.TabIndex = 0;
            this.btnAnimation.UseVisualStyleBackColor = true;
            this.btnAnimation.Click += new System.EventHandler(this.btnAnimation_Click);
            // 
            // tbrImageFrame
            // 
            this.tbrImageFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbrImageFrame.Location = new System.Drawing.Point(0, 0);
            this.tbrImageFrame.Name = "tbrImageFrame";
            this.tbrImageFrame.Size = new System.Drawing.Size(649, 49);
            this.tbrImageFrame.TabIndex = 0;
            this.tbrImageFrame.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbrImageFrame.Scroll += new System.EventHandler(this.tbrImageFrame_Scroll);
            this.tbrImageFrame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbrImageFrame_MouseDown);
            this.tbrImageFrame.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbrImageFrame_MouseUp);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(707, 451);
            this.Controls.Add(this.pnlAnimation);
            this.Controls.Add(this.statusStrip);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "MainForm";
            this.Text = "PicPreview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.pnlAnimation.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbrImageFrame)).EndInit();
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
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripSplitButton tsbImageEffects;
        private System.Windows.Forms.ToolStripSplitButton tsbSave;
        private System.Windows.Forms.ToolStripSplitButton tsbOptions;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.Panel pnlAnimation;
        private System.Windows.Forms.TrackBar tbrImageFrame;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnAnimation;
        private System.Windows.Forms.ToolStripSplitButton tsbRotateCCW;
        private System.Windows.Forms.ToolStripSplitButton tsbRotateCW;
    }
}

