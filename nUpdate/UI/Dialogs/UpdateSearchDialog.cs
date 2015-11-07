// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateSearchDialog : BaseDialog
    {
        private LocalizationProperties _lp;

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Occurs when the cancel button is clicked.
        /// </summary>
        public event EventHandler<EventArgs> CancelButtonClicked;

        protected virtual void OnCancelButtonClicked()
        {
            CancelButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            _lp = LocalizationHelper.GetLocalizationProperties(InteractionUpdater.LanguageCulture);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            OnCancelButtonClicked();
            DialogResult = DialogResult.Cancel;
        }

        #region TAP

        public void Fail(Exception ex)
        {
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                PopupButtons.Ok)));
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        #endregion

        #region EAP

        public void Failed(object sender, FailedEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption,
                            e.Exception.InnerException ?? e.Exception,
                            PopupButtons.Ok)));
            DialogResult = DialogResult.Cancel;
        }

        public void Finished(object sender, UpdateSearchFinishedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}