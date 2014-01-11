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
    internal partial class CustomColumnsList : Form
    {
        Mapping mapping;

        internal CustomColumnsList(Mapping mapping)
        {
            InitializeComponent();
            this.mapping = mapping;
            mapping.ColumnAdded += mapping_ColumnAdded;
            mapping.ColumnDeleted += mapping_ColumnDeleted;
            PopulateListBox();
            this.Disposed += CustomColumnsList_Disposed;
        }

        void CustomColumnsList_Disposed(object sender, EventArgs e)
        {
            mapping.ColumnAdded -= mapping_ColumnAdded;
            mapping.ColumnDeleted -= mapping_ColumnDeleted;
        }

        void mapping_ColumnAdded(object sender, EventArgs e)
        {
            columnsListBox.Items.Add((Mapping.ColumnDefinition)sender);
        }

        void mapping_ColumnDeleted(object sender, EventArgs e)
        {
            columnsListBox.Items.Remove((Mapping.ColumnDefinition)sender);
        }

        private void PopulateListBox()
        {
            foreach (Mapping.ColumnDefinition colDef in mapping.CustomHansoftColumns)
                this.columnsListBox.Items.Add(colDef);
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            CustomColumnCRUD dialog = new CustomColumnCRUD();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mapping.AddCustomColumn(dialog.DisplayName, dialog.CodeName, dialog.Type, dialog.Options);
                this.columnsListBox.SelectedIndex = columnsListBox.Items.Count - 1;
            }
        }

        private void editSelectedItem()
        {
            Mapping.ColumnDefinition colDef = (Mapping.ColumnDefinition)columnsListBox.SelectedItem;
            CustomColumnCRUD dialog = new CustomColumnCRUD(colDef.DisplayName, colDef.CodeName, colDef.Type, colDef.Options);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bool displayNameChanged = (dialog.DisplayName != colDef.DisplayName);
                mapping.UpdateCustomColumn(colDef, dialog.DisplayName, dialog.CodeName, dialog.Type, dialog.Options);

                // Hack to refresh the item name
                if (displayNameChanged) columnsListBox.Items[columnsListBox.SelectedIndex] = columnsListBox.SelectedItem;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            editSelectedItem();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the column definition? Any mapped columns will be reset.", "Hansoft Excel Import tool", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                Mapping.ColumnDefinition colDef = (Mapping.ColumnDefinition)columnsListBox.SelectedItem;
                foreach (Mapping.MappedColumn mappedCol in mapping.MappedColumns)
                {
                    if (mappedCol.HansoftColumnDefinition == colDef)
                        mappedCol.UpdateMappedHansoftColumn(null);
                }
                mapping.DeleteCustomColumn(colDef);
            }
        }

        private void columnsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.columnsListBox.SelectedIndex == -1)
            {
                this.editButton.Enabled = false;
                this.deleteButton.Enabled = false;
            }
            else
            {
                this.editButton.Enabled = true;
                this.deleteButton.Enabled = true;
            }
        }

        private void columnsListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (columnsListBox.SelectedItem != null)
                editSelectedItem();
        }
    }
}
