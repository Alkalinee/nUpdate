﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft;
using nUpdate.Localization;
using nUpdate.UI.Dialogs;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    /// <summary>
    ///     The integrated updater that interacts with the user using dialogs.
    /// </summary>
    public class DefaultInteractionUpdater : Updater
    {
        private readonly ManualResetEvent _searchResetEvent = new ManualResetEvent(false);
        private readonly LocalizationProperties _lp = new LocalizationProperties();
        private bool _updatesAvailable;
        private bool _isTaskRunning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultInteractionUpdater"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.</param>
        /// <param name="useHiddenSearch">If set to <c>true</c>, nUpdate will search for updates in the background without showing a search dialog.</param>
        public DefaultInteractionUpdater(Uri updateConfigurationFileUri, string publicKey, SynchronizationContext context, bool useHiddenSearch = false) 
            : base(updateConfigurationFileUri, publicKey)
        {
            Context = context;
            UseHiddenSearch = useHiddenSearch;
        }

        /// <summary>
        ///     Gets or sets the <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.
        /// </summary>
        internal SynchronizationContext Context { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a hidden search should be provided in order to search in the background without informing the user, or not.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        /// <summary>
        ///     Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            if (_isTaskRunning)
                return;

            _isTaskRunning = true;
            var searchDialog = new UpdateSearchDialog { InteractionUpdater = this };
            searchDialog.CancelButtonClicked += UpdateSearchDialogCancelButtonClick;

            var newUpdateDialog = new NewUpdateDialog {InteractionUpdater = this};
            var noUpdateDialog = new NoUpdateFoundDialog { InteractionUpdater = this };

            // ReSharper disable once UnusedVariable
            var progressIndicator = new Progress<UpdateDownloadProgressChangedEventArgs>();
            var downloadDialog = new UpdateDownloadDialog {InteractionUpdater = this};
            downloadDialog.CancelButtonClicked += UpdateDownloadDialogCancelButtonClick;

#if PROVIDE_TAP

            try
            {
                // TAP
                TaskEx.Run(async delegate
                {
                    if (!UseHiddenSearch)
                        _context.Post(searchDialog.ShowModalDialog, null);

                    try
                    {
                        _updatesAvailable = await _updateManager.SearchForUpdatesAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (UseHiddenSearch)
                            _context.Send(
                                o =>
                                    Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                                        PopupButtons.Ok), null);
                        else
                        {
                            searchDialog.Fail(ex);
                            _context.Post(searchDialog.CloseDialog, null);
                        }
                        return;
                    }

                    if (!UseHiddenSearch)
                    {
                        _context.Post(searchDialog.CloseDialog, null);
                        await TaskEx.Delay(100);
                            // Prevents race conditions that cause that the UpdateSearchDialog can't be closed before further actions are done
                    }

                    if (_updatesAvailable)
                    {
                        newUpdateDialog.PackageSize = _updateManager.TotalSize;
                        newUpdateDialog.PackageConfigurations = _updateManager.PackageConfigurations;
                        var newUpdateDialogReference = new DialogResultReference();
                        _context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogReference);
                        if (newUpdateDialogReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else if (!_updatesAvailable && UseHiddenSearch)
                        return;
                    else if (!_updatesAvailable && !UseHiddenSearch)
                    {
                        var noUpdateDialogResultReference = new DialogResultReference();
                        if (!UseHiddenSearch)
                            _context.Send(noUpdateDialog.ShowModalDialog, noUpdateDialogResultReference);
                        return;
                    }

                    downloadDialog.PackagesCount = _updateManager.PackageConfigurations.Count();
                    _context.Post(downloadDialog.ShowModalDialog, null);

                    try
                    {
                        progressIndicator.ProgressChanged += (sender, args) =>
                            downloadDialog.ProgressPercentage = (int) args.Percentage;
                        
                        await _updateManager.DownloadPackagesAsync(progressIndicator);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        downloadDialog.Fail(ex);
                        _context.Send(downloadDialog.CloseDialog, null);
                        return;
                    }
                    _context.Send(downloadDialog.CloseDialog, null);

                    bool isValid = false;
                    try
                    {
                        isValid = _updateManager.ValidatePackages();
                    }
                    catch (FileNotFoundException)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                    }
                    catch (ArgumentException)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    }
                    catch (Exception ex)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                    }

                    if (!isValid)
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                        _updateManager.InstallPackage();
                });
            }
            finally
            {
                _isTaskRunning = false;
            }

#else
            try
            {
                //EAP
                UpdateSearchFinished += SearchFinished;
                UpdateSearchFinished += searchDialog.Finished;
                UpdateSearchFailed += searchDialog.Failed;
                PackagesDownloadProgressChanged += downloadDialog.ProgressChanged;
                PackagesDownloadFinished += downloadDialog.Finished;
                PackagesDownloadFailed += downloadDialog.Failed;

                Task.Factory.StartNew(() =>
                {
                    SearchForUpdatesAsync();
                    if (!UseHiddenSearch)
                    {
                        var searchDialogResultReference = new DialogResultReference();
                        Context.Send(searchDialog.ShowModalDialog, searchDialogResultReference);
                        Context.Send(searchDialog.CloseDialog, null);
                        if (searchDialogResultReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else
                    {
                        _searchResetEvent.WaitOne();
                    }

                    if (_updatesAvailable)
                    {
                        var newUpdateDialogResultReference = new DialogResultReference();
                        Context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogResultReference);
                        if (newUpdateDialogResultReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else if (!_updatesAvailable && UseHiddenSearch)
                        return;
                    else if (!_updatesAvailable && !UseHiddenSearch)
                    {
                        Context.Send(noUpdateDialog.ShowModalDialog, null);
                        Context.Send(noUpdateDialog.CloseDialog, null);
                        return;
                    }

                    DownloadPackagesAsync();

                    var downloadDialogResultReference = new DialogResultReference();
                    Context.Send(downloadDialog.ShowModalDialog, downloadDialogResultReference);
                    Context.Send(downloadDialog.CloseDialog, null);
                    if (downloadDialogResultReference.DialogResult == DialogResult.Cancel)
                        return;

                    bool isValid = false;
                    try
                    {
                        isValid = ValidatePackages();
                    }
                    catch (FileNotFoundException)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                    }
                    catch (ArgumentException)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    }
                    catch (Exception ex)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                    }

                    if (!isValid)
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                        InstallPackage();
                });
            }
            finally
            {
                _isTaskRunning = false;
            }
#endif
        }

        private void SearchFinished(object sender, UpdateSearchFinishedEventArgs e)
        {
            _updatesAvailable = e.UpdatesAvailable;
            _searchResetEvent.Set();
        }

        private void UpdateSearchDialogCancelButtonClick(object sender, EventArgs e)
        {
            CancelSearch();
        }

        private void UpdateDownloadDialogCancelButtonClick(object sender, EventArgs e)
        {
            CancelDownload();
        }
    }
}