﻿// Author: Dominic Beger (Trade/ProgTrade)

using System.Drawing;
using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UI.Dialogs
{
    public class BaseDialog : Form
    {
        public Updater InteractionUpdater { get; set; }

        public void InitializeComponent()
        {
            SuspendLayout();
            // 
            // BaseForm
            // 
            ClientSize = new Size(284, 262);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);
            ResumeLayout(true);
        }
    }
}