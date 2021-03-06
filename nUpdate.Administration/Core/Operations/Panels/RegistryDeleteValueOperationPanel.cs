﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class RegistryDeleteValueOperationPanel : UserControl, IOperationPanel
    {
        private BindingList<string> _itemList = new BindingList<string>();

        public RegistryDeleteValueOperationPanel()
        {
            InitializeComponent();
        }

        public bool IsValid
        {
            get { return !String.IsNullOrEmpty(subKeyTextBox.Text) && ItemList.Any(); }
        }

        public string KeyPath
        {
            get
            {
                return String.Format("{0}\\{1}", mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedItem),
                    subKeyTextBox.Text);
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
                        subKeyTextBox.Text += String.Format("\\{0}", pathPart);
                    }
                }
            }
        }

        public BindingList<string> ItemList
        {
            get { return _itemList; }
            set { _itemList = value; }
        }

        public Operation Operation
        {
            get { return new Operation(OperationArea.Registry, OperationMethod.Delete, KeyPath, ItemList.ToList()); }
        }

        private void RegistryEntryDeleteValueOperationPanel_Load(object sender, EventArgs e)
        {
            nameValuePairsToDeleteListBox.DataSource = _itemList;
            mainKeyComboBox.SelectedIndex = 0;
        }

        private void valueNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(valueNameTextBox.Text))
                return;
            _itemList.Add(valueNameTextBox.Text);
            valueNameTextBox.Clear();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            _itemList.RemoveAt(nameValuePairsToDeleteListBox.SelectedIndex);
        }

        private void InputChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (!textBox.Text.Contains("/"))
                return;
            textBox.Text = textBox.Text.Replace('/', '\\');
            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }
    }
}