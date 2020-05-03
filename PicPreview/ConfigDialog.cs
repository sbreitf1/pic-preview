using PicPreview.Properties;
using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PicPreview
{
    public partial class ConfigDialog : Form
    {
        public ConfigDialog()
        {
            InitializeComponent();
            this.Icon = Resources.app_options;

            cbxMaximizeFilter.SelectedIndex = GetListIndex(Settings.Default.MaximizeFilter);
            cbxMinimizeFilter.SelectedIndex = GetListIndex(Settings.Default.MinimizeFilter);
            cbxFastRenderAnimations.Checked = Settings.Default.FastRenderAnimations;
            cbxFastRenderInteraction.Checked = Settings.Default.FastRenderInteraction;
            cbxRenderTransparencyGrid.Checked = Settings.Default.RenderTransparencyGrid;
        }

        private int GetListIndex(InterpolationMode val)
        {
            switch (val)
            {
                case InterpolationMode.NearestNeighbor: return 0;
                case InterpolationMode.Bilinear: return 1;
                case InterpolationMode.HighQualityBicubic: return 2;
                default: return 0;
            }
        }

        private void btnAssociate_Click(object sender, EventArgs e)
        {
            try
            {
                FileAssociation.RegisterApplicationForUser();
                FileAssociation.AssociateForUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show("File association failed: " + ex.Message, "PicPreview", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings.Default.MaximizeFilter = GetInterpolationMode(cbxMaximizeFilter.SelectedIndex);
            Settings.Default.MinimizeFilter = GetInterpolationMode(cbxMinimizeFilter.SelectedIndex);
            Settings.Default.FastRenderAnimations = cbxFastRenderAnimations.Checked;
            Settings.Default.FastRenderInteraction = cbxFastRenderInteraction.Checked;
            Settings.Default.RenderTransparencyGrid = cbxRenderTransparencyGrid.Checked;
            Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private InterpolationMode GetInterpolationMode(int listIndex)
        {
            switch (listIndex)
            {
                case 0: return InterpolationMode.NearestNeighbor;
                case 1: return InterpolationMode.Bilinear;
                case 2: return InterpolationMode.HighQualityBicubic;
                default: return InterpolationMode.NearestNeighbor;
            }
        }
    }
}
