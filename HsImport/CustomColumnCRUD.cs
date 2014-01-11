using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HsImport
{
    public partial class CustomColumnCRUD : Form
    {
        public CustomColumnCRUD()
        {
            InitializeComponent();
            typeComboBox.Items.AddRange(new object[]{
                Mapping.ColumnDefinition.ColumnType.String,
                Mapping.ColumnDefinition.ColumnType.Integer,
                Mapping.ColumnDefinition.ColumnType.Float,
                Mapping.ColumnDefinition.ColumnType.Date,
                Mapping.ColumnDefinition.ColumnType.DateTime,
                Mapping.ColumnDefinition.ColumnType.EnumSingle,
                Mapping.ColumnDefinition.ColumnType.EnumMulti,
                Mapping.ColumnDefinition.ColumnType.Hyperlink,
                Mapping.ColumnDefinition.ColumnType.MultilineText,
                Mapping.ColumnDefinition.ColumnType.People });
        }

        internal CustomColumnCRUD(string displayName, string codeName, Mapping.ColumnDefinition.ColumnType type, string[] options) : this()
        {
            displayNameTextBox.Text = displayName;
            codeNameTextBox.Text = codeName;
            typeComboBox.SelectedItem = type;
            optionsListBox.Items.AddRange(options);
        }

        internal string DisplayName
        {
            get { return displayNameTextBox.Text; }
        }

        internal string CodeName
        {
            get { return codeNameTextBox.Text; } 
        }

        internal Mapping.ColumnDefinition.ColumnType Type
        {
            get { return (Mapping.ColumnDefinition.ColumnType)typeComboBox.SelectedItem; } 
        }

        internal string[] Options
        {
            get
            {
                string[] options = new string[optionsListBox.Items.Count];
                for (int i = 0; i < optionsListBox.Items.Count; i += 1)
                    options[i] = (string)optionsListBox.Items[i];
                return options;
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedItem != null && (typeComboBox.SelectedItem.Equals(Mapping.ColumnDefinition.ColumnType.EnumSingle) || typeComboBox.SelectedItem.Equals(Mapping.ColumnDefinition.ColumnType.EnumMulti)))
                optionsGroupBox.Enabled = true;
            else
                optionsGroupBox.Enabled = false;
            ConditionallyEnableOk();
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            object tmp = optionsListBox.Items[optionsListBox.SelectedIndex];
            optionsListBox.Items[optionsListBox.SelectedIndex] = optionsListBox.Items[optionsListBox.SelectedIndex - 1];
            optionsListBox.Items[optionsListBox.SelectedIndex - 1 ] = tmp;
            optionsListBox.SelectedIndex = optionsListBox.SelectedIndex - 1;
            ConditionallyEnableOk();
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            object tmp = optionsListBox.Items[optionsListBox.SelectedIndex];
            optionsListBox.Items[optionsListBox.SelectedIndex] = optionsListBox.Items[optionsListBox.SelectedIndex + 1];
            optionsListBox.Items[optionsListBox.SelectedIndex + 1] = tmp;
            optionsListBox.SelectedIndex = optionsListBox.SelectedIndex + 1;
            ConditionallyEnableOk();
        }

        private void ConditionallyEnableUpDownButtons()
        {
            if (optionsListBox.SelectedIndex == -1)
            {
                editButton.Enabled = false;
                deleteButton.Enabled = false;
                upButton.Enabled = false;
                downButton.Enabled = false;
            }
            else
            {
                editButton.Enabled = true;
                deleteButton.Enabled = true;
                if (optionsListBox.SelectedIndex == 0)
                    upButton.Enabled = false;
                else
                    upButton.Enabled = true;
                if (optionsListBox.SelectedIndex == optionsListBox.Items.Count - 1)
                    downButton.Enabled = false;
                else
                    downButton.Enabled = true;
            }
        }

        private void optionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConditionallyEnableUpDownButtons();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            OptionCRUD optionDialog = new OptionCRUD();
            if (optionDialog.ShowDialog() == DialogResult.OK)
            {
                bool exists = false;
                foreach (string s in optionsListBox.Items)
                {
                    if (s.Equals(optionDialog.OptionText))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    optionsListBox.Items.Add(optionDialog.OptionText);
                    ConditionallyEnableOk();
                    ConditionallyEnableUpDownButtons();
                    optionsListBox.SelectedIndex = optionsListBox.Items.Count - 1;
                }
                else
                    MessageBox.Show("There already exists an options with this name", "Hansoft Excel Import tool", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void editSelectedItem()
        {
            OptionCRUD optionDialog = new OptionCRUD((string)optionsListBox.SelectedItem);
            if (optionDialog.ShowDialog() == DialogResult.OK)
            {
                bool exists = false;
                foreach (string s in optionsListBox.Items)
                {
                    if (s.Equals(optionDialog.OptionText))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    optionsListBox.Items[optionsListBox.SelectedIndex] = optionDialog.OptionText;
                    ConditionallyEnableOk();
                }
                else
                    MessageBox.Show("There already exists an options with this name", "Hansoft Excel Import tool", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            editSelectedItem();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int ind = optionsListBox.SelectedIndex;
            optionsListBox.Items.RemoveAt(optionsListBox.SelectedIndex);
            if (ind < optionsListBox.Items.Count)
                optionsListBox.SelectedIndex = ind;
            else if (optionsListBox.Items.Count > 0)
                optionsListBox.SelectedIndex = 0;
            ConditionallyEnableOk();
            ConditionallyEnableUpDownButtons();
        }

        private void displayNameTextBox_TextChanged(object sender, EventArgs e)
        {
            GenerateCodeName();
            ConditionallyEnableOk();
        }

        private void ConditionallyEnableOk()
        {
            okButton.Enabled = ShouldOkBeEnabled();
        }

        private bool ShouldOkBeEnabled()
        {
            if (displayNameTextBox.Text.Length <= 2)
                return false;
            if (codeNameTextBox.Text.Length <= 2)
                return false;
            if (codeNameTextBox.Text.Contains(" "))
                return false;
            if (typeComboBox.SelectedIndex == -1)
                return false;
            if ((typeComboBox.SelectedIndex == 5 || typeComboBox.SelectedIndex == 6) && optionsListBox.Items.Count <= 1)
                return false;
            return true;
        }

        private void GenerateCodeName()
        {
            codeNameTextBox.Text = XmlConvert.EncodeName(displayNameTextBox.Text).Replace("_x00", "_");
        }

        private void optionsListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (optionsListBox.SelectedItem != null)
                editSelectedItem();
        }
    }
}
