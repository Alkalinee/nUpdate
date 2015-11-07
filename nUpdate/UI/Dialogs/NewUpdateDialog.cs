// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.Updating;
using nUpdate.Win32;

namespace nUpdate.UI.Dialogs
{
    public partial class NewUpdateDialog : BaseDialog
    {
        private bool _allowCancel;
        private LocalizationProperties _lp;

        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        internal static void AddShieldToButton(Button btn)
        {
            const int bcmSetshield = 0x160C;

            btn.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage(btn.Handle, bcmSetshield, new IntPtr(0), new IntPtr(1));
        }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(InteractionUpdater.LanguageCulture);

            headerLabel.Text =
                string.Format(
                    InteractionUpdater.PackageConfigurations.Count() > 1
                        ? _lp.NewUpdateDialogMultipleUpdatesHeader
                        : _lp.NewUpdateDialogSingleUpdateHeader, InteractionUpdater.PackageConfigurations.Count());
            infoLabel.Text = string.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);

            var availableVersions =
                InteractionUpdater.PackageConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            newestVersionLabel.Text = string.Format(_lp.NewUpdateDialogAvailableVersionsText,
                InteractionUpdater.PackageConfigurations.Count() <= 2
                    ? string.Join(", ", availableVersions.Select(item => item.FullText))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).FullText} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).FullText}");
            currentVersionLabel.Text = string.Format(_lp.NewUpdateDialogCurrentVersionText, InteractionUpdater.CurrentVersion.FullText);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;
            updateSizeLabel.Text = string.Format(_lp.NewUpdateDialogSizeText, SizeHelper.ToAdequateSizeString((long)InteractionUpdater.TotalSize));
            
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
            if (Icon != null)
                iconPictureBox.Image = Icon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
            AddShieldToButton(installButton);

            foreach (var updateConfiguration in InteractionUpdater.PackageConfigurations)
            {
                var versionText = new UpdateVersion(updateConfiguration.LiteralVersion).FullText;
                var changelogText = updateConfiguration.Changelog.ContainsKey(InteractionUpdater.LanguageCulture)
                    ? updateConfiguration.Changelog.First(item => Equals(item.Key, InteractionUpdater.LanguageCulture)).Value
                    : updateConfiguration.Changelog.First(item => Equals(item.Key, new CultureInfo("en"))).Value;

                changelogTextBox.Text +=
                    string.Format(string.IsNullOrEmpty(changelogTextBox.Text) ? "{0}:\n{1}" : "\n\n{0}:\n{1}",
                        versionText, changelogText);
            }

            var operationAreas =
                InteractionUpdater.PackageConfigurations.Select(item => item.Operations.Select(op => op.Area)).ToList();
            if (!operationAreas.Any())
            {
                accessLabel.Text = $"{_lp.NewUpdateDialogAccessText} -";
                _allowCancel = true;
                return;
            }

            accessLabel.Text =
                $"{_lp.NewUpdateDialogAccessText} {string.Join(", ", LocalizationHelper.GetLocalizedEnumerationValues(_lp, operationAreas.Cast<object>().GroupBy(item => item).Select(item => item.First()).ToArray()))}";
            _allowCancel = true;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            _allowCancel = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void NewUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_allowCancel;
        }

        private void changelogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}