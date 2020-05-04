using System;
using System.Windows.Forms;

namespace PicPreview
{
    public partial class ExportQualityDialog : Form
    {
        public int Quality
        {
            get { return (int)this.tbrQuality.Value; }
            set { this.tbrQuality.Value = value; }
        }

        public ExportQualityDialog()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.file_options;
        }

        private void ExportQualityDialog_Load(object sender, EventArgs e)
        {
            lblQuality.Text = tbrQuality.Value + " %";
        }

        private void tbrQuality_Scroll(object sender, EventArgs e)
        {
            lblQuality.Text = tbrQuality.Value + " %";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
