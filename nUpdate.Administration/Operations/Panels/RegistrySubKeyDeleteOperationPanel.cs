﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Operations;

namespace nUpdate.Administration.Operations.Panels
{
    public partial class RegistrySubKeyDeleteOperationPanel : UserControl, IOperationPanel
    {
        private BindingList<string> _itemList = new BindingList<string>();

        public RegistrySubKeyDeleteOperationPanel()
        {
            InitializeComponent();
        }

        public string KeyPath
        {
            get
            {
                return $"{mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedItem)}\\{subKeyTextBox.Text}";
            }
            set
            {
                var pathParts = value.Split('\\');
                foreach (var pathPart in pathParts)
                {
                    if (pathPart == pathParts[0])
                    {
                        mainKeyComboBox.SelectedValue = pathParts[0];
                    }
                    else
                    {
                        subKeyTextBox.Text += $"\\{pathPart}";
                    }
                }
            }
        }

        public BindingList<string> ItemList
        {
            get { return _itemList; }
            set { _itemList = value; }
        }

        public bool IsValid => !string.IsNullOrEmpty(subKeyTextBox.Text) && ItemList.Any();

        public Operation Operation => new Operation(OperationArea.Registry, OperationMethod.Delete, KeyPath, ItemList.ToList());

        private void RegistryEntryDeleteOperationPanel_Load(object sender, EventArgs e)
        {
            subKeysToDeleteListBox.DataSource = _itemList;
            mainKeyComboBox.SelectedIndex = 0;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(keyNameTextBox.Text))
                return;
            _itemList.Add(keyNameTextBox.Text);
            keyNameTextBox.Clear();
        }

        private void keyNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            _itemList.RemoveAt(subKeysToDeleteListBox.SelectedIndex);
        }

        private void InputChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox) sender;
            if (!textBox.Text.Contains("/"))
                return;
            textBox.Text = textBox.Text.Replace('/', '\\');
            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }
    }
}