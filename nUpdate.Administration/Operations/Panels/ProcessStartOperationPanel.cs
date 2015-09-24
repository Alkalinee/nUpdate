﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using nUpdate.Operations;

namespace nUpdate.Administration.Operations.Panels
{
    public partial class ProcessStartOperationPanel : UserControl, IOperationPanel
    {
        public ProcessStartOperationPanel()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return pathTextBox.Text; }
            set { pathTextBox.Text = value; }
        }

        public IEnumerable<string> Arguments
        {
            get { return argumentTextBox.Text.Split(','); }
            set { argumentTextBox.Text = string.Join(",", value); }
        }

        public bool IsValid => !string.IsNullOrEmpty(pathTextBox.Text) && pathTextBox.Text.Contains("\\") &&
                               pathTextBox.Text.Split(new[] {"\\"}, StringSplitOptions.RemoveEmptyEntries).Length >= 2;

        public Operation Operation => new Operation(OperationArea.Processes, OperationMethod.Start, pathTextBox.Text,
            Arguments.ToList());

        private void environmentVariablesButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables.",
                "%appdata%: AppData\n%temp%: Temp\n%program%: Program's directory\n%desktop%: Desktop directory",
                PopupButtons.Ok);
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!pathTextBox.Text.Contains("/"))
                return;
            pathTextBox.Text = pathTextBox.Text.Replace('/', '\\');
            pathTextBox.SelectionStart = pathTextBox.Text.Length;
            pathTextBox.SelectionLength = 0;
        }
    }
}