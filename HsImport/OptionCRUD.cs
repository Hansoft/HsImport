using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HsImport
{
    public partial class OptionCRUD : Form
    {
        public OptionCRUD()
        {
            InitializeComponent();
            nameTextBox.Focus();
        }

        public OptionCRUD(string optionText) : this()
        {
            nameTextBox.Text = optionText;
        }

        internal string OptionText
        {
            get { return nameTextBox.Text; }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (nameTextBox.Text != null && nameTextBox.Text != "")
                okButton.Enabled = true;
            else
                okButton.Enabled = false;
        }
    }
}
