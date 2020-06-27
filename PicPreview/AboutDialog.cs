using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace PicPreview
{
    public partial class AboutDialog : Form
    {
        private enum FunStates
        {
            Inactive,
            Active,
            GameOver
        }

        private FunStates funState = FunStates.Inactive;
        private DateTime funStateBegin = DateTime.Now;
        private DateTime lastClicked;
        private int points, bestPoints;
        private Point defaultPos;


        public AboutDialog()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.info;
            lblTitle.Text = Program.AppName + " " + Program.AppVersion.NiceString();
            pbxIcon.Image = Properties.Resources.about_logo;
            this.defaultPos = pbxIcon.Location;

            this.bestPoints = Properties.Settings.Default.FunHighscore;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("mailto:sbreitf1@web.de?subject=PicPreview");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open e-mail client:\n\n" + ex.Message, "About", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://sbreitf1.de");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open e-mail client:\n\n" + ex.Message, "About", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbxIcon_Click(object sender, EventArgs e)
        {
            if (this.funState == FunStates.Inactive)
            {
                this.funState = FunStates.Active;
                btnClose.Enabled = false;
                btnLicenses.Enabled = false;
                linkContact.Enabled = false;
                linkWebsite.Enabled = false;
                this.funStateBegin = DateTime.Now;
                this.points = 0;
            }
            if (this.funState == FunStates.Active)
            {
                this.lastClicked = DateTime.Now;
                Random rnd = new Random();
                pbxIcon.Left = rnd.Next(this.ClientSize.Width - pbxIcon.Width);
                pbxIcon.Top = rnd.Next(this.ClientSize.Height - pbxIcon.Height);
                this.points++;
                if (this.points > this.bestPoints)
                    this.bestPoints = this.points;
                lblPoints.Text = points.ToString() + " (" + this.bestPoints + ")";
            }
        }

        private void tmrFun_Tick(object sender, EventArgs e)
        {
            double tState = (DateTime.Now - this.funStateBegin).TotalMilliseconds;

            if (this.funState == FunStates.Inactive)
            {
                const double animWait = 30000;
                const double animDuration = 4000;
                const double animSleep = 4000;
                const double maxDislocation = 1;

                double t = tState - animWait;
                if (t > 0 && t % (animDuration + animSleep) < animDuration)
                {
                    t = t % (animDuration + animSleep);
                    double f = ((1 - Math.Cos(t / animDuration * 2 * Math.PI)) / 2);

                    Random rnd = new Random();
                    pbxIcon.Left = (int)Math.Round(this.defaultPos.X + f * maxDislocation * 2 * (rnd.NextDouble() - 0.5));
                    pbxIcon.Top = (int)Math.Round(this.defaultPos.X + f * maxDislocation * 2 * (rnd.NextDouble() - 0.5));
                }
                else
                {
                    pbxIcon.Location = this.defaultPos;
                }
            }
            else if (this.funState == FunStates.Active)
            {
                const double maxCountdown = 1000;
                const double minCountdown = 200;
                const double minCountdownAfter = 120000;

                double tMax = minCountdown;
                if (tState < minCountdownAfter)
                {
                    tMax = maxCountdown - (maxCountdown - minCountdown) * Math.Sqrt(tState / minCountdownAfter);
                }

                TimeSpan span = DateTime.Now - this.lastClicked;
                if (span.TotalMilliseconds > tMax)
                {
                    this.funState = FunStates.GameOver;
                    this.funStateBegin = DateTime.Now;
                    pbxIcon.Visible = false;
                    pbxIcon.Location = this.defaultPos;
                }
            }
            else if (this.funState == FunStates.GameOver)
            {
                const double restartGameAfter = 500;

                TimeSpan span = DateTime.Now - this.funStateBegin;
                if (span.TotalMilliseconds > restartGameAfter)
                {
                    this.funState = FunStates.Inactive;
                    this.funStateBegin = DateTime.Now;
                    pbxIcon.Visible = true;
                    btnClose.Enabled = true;
                    btnLicenses.Enabled = true;
                    linkContact.Enabled = true;
                    linkWebsite.Enabled = true;
                }
            }
        }

        private void btnLicenses_Click(object sender, EventArgs e)
        {
            LicensesDialog dialog = new LicensesDialog();
            dialog.ShowDialog();
        }

        private void AboutDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrFun.Stop();
            Properties.Settings.Default.FunHighscore = this.bestPoints;
            Properties.Settings.Default.Save();
        }
    }
}
