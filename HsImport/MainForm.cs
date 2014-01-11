using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace HsImport
{
    public partial class MainForm : Form
    {
        TextBox[] textBoxes;
        ComboBox[] comboBoxes;
        Mapping mapping;
        string templateFilePath;
        ExcelReader sourceReader;
        bool processComboBoxChanges;

        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            textBoxes = new TextBox[] {t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24, t25, t26, t27, t28, t29, t30, t31, t32, t33, t34, t35, t36, t37, t38, t39, t40};
            comboBoxes = new ComboBox[] { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15, c16, c17, c18, c19, c20, c21, c21, c22, c23, c24, c25, c26, c27, c28, c29, c30, c31, c32, c33, c34, c35, c36, c37, c38, c39, c40 };

            string startPath = Path.GetDirectoryName(Application.ExecutablePath);
            templateFilePath = Path.Combine(startPath, "MappingTemplate.xml");
            NewMapping();
        }

        void NewMapping()
        {
            LoadMapping(templateFilePath, true);
        }

        void LoadMapping(string fileName, bool isNewFile)
        {
            SuspendLayout2();
            if (mapping != null)
            {
                mapping.ColumnAdded -= ColumnAddedHandler;
                mapping.ColumnDeleted -= ColumnDeletedHandler;
                mapping.ColumnUpdated -= ColumnUpdatedHandler;
                mapping.DirtyChanged -= DirtyChangedHandler;
                mapping.FileNameChanged -= FileNameChangedHandler;
                mapping.SourceChanged -= SourceChangedHandler;
                mapping.TargetChanged -= TargetChangedHandler;
            }
            mapping = new Mapping(fileName, isNewFile);
            mapping.ColumnAdded += ColumnAddedHandler;
            mapping.ColumnDeleted += ColumnDeletedHandler;
            mapping.ColumnUpdated += ColumnUpdatedHandler;
            mapping.DirtyChanged += DirtyChangedHandler;
            mapping.FileNameChanged += FileNameChangedHandler;
            mapping.SourceChanged += SourceChangedHandler;
            mapping.TargetChanged += TargetChangedHandler;
            processComboBoxChanges = false;
            PopulateCombos();
            OnSourceSet(false, false, false);
            OnTargetSet();
            ActivateCombos(true);
            UpdateCaption();
            processComboBoxChanges = true;
            ResumeLayout2();
        }

        void TargetChangedHandler(object sender, EventArgs e)
        {
            OnTargetSet();
        }

        void OnTargetSet()
        {
            this.targetFileLabel.Text = mapping.Target;
        }

        void OnSourceSet(bool askIfFirstRowIsHeader, bool makeMappingDirty, bool resetMappings)
        {
            SuspendLayout2();
            this.sourceFileLabel.Text = mapping.Source;
            if (mapping.Source != string.Empty && mapping.Source != null)
            {

                try
                {
                    sourceReader = new ExcelReader(mapping.Source);
                    if (askIfFirstRowIsHeader)
                    {
                        string dataSample = "";
                        for (int i = 0; i <= Math.Min(sourceReader.ColumnHeaders.Length - 1, 3); i += 1)
                        {
                            dataSample += sourceReader.ColumnHeaders[i];
                            dataSample += ", ";
                        }
                        dataSample += "...";
                        mapping.FirstSourceRowIsHeader = (MessageBox.Show("Is the first row a header (" + dataSample + "), i.e., to be excluded from the data mapped?", "Hansoft Excel Import tool", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("It was not possible to open the file " + mapping.Source + ". Make sure that it is a valid Excel file and not open in another program");
                    sourceReader = null;
                }
            }
            else
                sourceReader = null;
            RefreshExcelValues();
            if (resetMappings)
                ResetMappings(makeMappingDirty);
            ActivateCombos(true);
            ResumeLayout2();
        }

        void SourceChangedHandler(object sender, EventArgs e)
        {
            OnSourceSet(true, true, true);
        }

        void PopulateCombos()
        {
            for (int ind = 0; ind < comboBoxes.Length; ind += 1)
            {
                comboBoxes[ind].Items.Clear();
                comboBoxes[ind].Items.Add("<Not mapped>");
                comboBoxes[ind].Items.AddRange(mapping.BuiltInHansoftColumns.ToArray());
                comboBoxes[ind].Items.AddRange(mapping.CustomHansoftColumns.ToArray());
            }
        }

        void ActivateCombos(bool reset)
        {
            for (int comboInd = 0; comboInd < comboBoxes.Length; comboInd += 1)
            {
                if (sourceReader != null && comboInd < sourceReader.ColumnHeaders.Length)
                {
                    comboBoxes[comboInd].Visible = true;
                    comboBoxes[comboInd].Enabled = true;
                    foreach (Mapping.MappedColumn mappedCol in mapping.MappedColumns)
                    {
                        if (mappedCol.ExcelColumnIndex == comboInd)
                        {
                            if (mappedCol.HansoftColumnDefinition != null)
                                comboBoxes[comboInd].SelectedItem = mappedCol.HansoftColumnDefinition;
                            else
                                comboBoxes[comboInd].SelectedIndex = 0;
                            break;
                        }
                    }
                }
                else
                {
                    comboBoxes[comboInd].Visible = false;
                    comboBoxes[comboInd].Enabled = false;
                }
            }
        }

        void RefreshExcelValues()
        {
            for (int tbInd = 0; tbInd < textBoxes.Length; tbInd += 1)
            {
                if (sourceReader != null && tbInd < sourceReader.ColumnHeaders.Length)
                {
                    textBoxes[tbInd].Text = sourceReader.ColumnHeaders[tbInd];
                    textBoxes[tbInd].Visible = true;
                    textBoxes[tbInd].Enabled = false;
                }
                else
                {
                    textBoxes[tbInd].Text = "";
                    textBoxes[tbInd].Visible = false;
                    textBoxes[tbInd].Enabled = false;
                }
            }
        }

        void FileNameChangedHandler(object sender, EventArgs e)
        {
            UpdateCaption();
        }

        void DirtyChangedHandler(object sender, EventArgs e)
        {
            UpdateCaption();
        }

        void ResetMappings(bool makeMappingDirty)
        {
            mapping.ClearMappedColumns(makeMappingDirty);
            if (sourceReader != null)
            {
                for (int i = 0; i < sourceReader.ColumnHeaders.Length; i += 1)
                    mapping.AddMappedColumn(i, null, makeMappingDirty);
            }
        }

        void ColumnAddedHandler(object sender, EventArgs e)
        {
            SuspendLayout2();
            for (int ind = 0; ind < comboBoxes.Length; ind += 1)
                comboBoxes[ind].Items.Add(sender);
            ResumeLayout2();
        }

        void ColumnDeletedHandler(object sender, EventArgs e)
        {
            SuspendLayout2();
            for (int ind = 0; ind < comboBoxes.Length; ind += 1)
            {
                if (comboBoxes[ind].SelectedItem == sender)
                    comboBoxes[ind].SelectedIndex = 0;
                comboBoxes[ind].Items.Remove(sender);
            }
            ResumeLayout2();
        }

        void ColumnUpdatedHandler(object sender, EventArgs e)
        {
            SuspendLayout2();
            for (int ind = 0; ind < comboBoxes.Length; ind += 1)
            {
                for (int jnd = 0; jnd < comboBoxes[ind].Items.Count; jnd += 1 )
                {
                    if (comboBoxes[ind].Items[jnd] == sender)
                    {
                        comboBoxes[ind].Items[jnd] = sender;
                        break;
                    }
                }
            }
            ResumeLayout2();
        }

        void UpdateCaption()
        {
            string fileName = mapping.FileName == null? "?" : mapping.FileName;
            string starred = mapping.Dirty ? "*" : "";
            this.Text = "Excel to Hansoft Xml [" + fileName + "]" + starred;
        }

        private bool SaveMapping(bool promptAlwaysForFileName)
        {
            if (mapping.FileName == null || promptAlwaysForFileName)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.AddExtension = true;
                sfd.CheckFileExists = false;
                sfd.CheckPathExists = true;
                sfd.FileName = "Mapping";
                sfd.DefaultExt = "xml";
                sfd.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                switch (sfd.ShowDialog())
                {
                    case DialogResult.OK:
                        mapping.FileName = sfd.FileName;
                        break;
                    case DialogResult.Cancel:
                        return false;
                }
            }
            return mapping.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveConditionallyIfDirty())
                this.Close();
        }

        private bool SaveConditionallyIfDirty()
        {
            bool retval = true;
            if (mapping.Dirty)
            {
                switch (MessageBox.Show("Do you want to save your changes?", "Hansoft Import Tool", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        retval = SaveMapping(false);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        retval = false;
                        break;
                }
            }
            return retval;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMapping(false);
        }

        private void setInputButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "Excel worksheets (*.xlsx;*.xlsm;*.xls)|*.xlsx;*.xlsm;*.xls|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                mapping.Source = ofd.FileName;
        }

        private void setOutputFileButton_Click(object sender, EventArgs e)
        {
            setoutputFile();
        }

        private bool setoutputFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.FileName = "HansoftItems";
            sfd.DefaultExt = "xml";
            sfd.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                mapping.Target = sfd.FileName;
                return true;
            }
            else
                return false;

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMapping();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMapping(true);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "Mapping definitions (*.xml)|*.xml|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                LoadMapping(ofd.FileName, false);
        }

        private void savveOutputFileButton_Click(object sender, EventArgs e)
        {
            bool doSave = true;
            if (mapping.Target == null || mapping.Target == string.Empty)
            {
                doSave = false;
                if (MessageBox.Show("You must set the name of the output file first","Hansoft Excel Import tool", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    doSave = setoutputFile();
 
            }
            if (doSave)
            {
                FileInfo fInfo = new FileInfo(mapping.Target);
                if (fInfo.Exists)
                {
                    if (MessageBox.Show("The file " + mapping.Target + " already exists, is it ok to overwrite it?", "Hansoft Excel Import tool", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        fInfo.Delete();
                    else
                        doSave = false;
                }
                if (doSave)
                {
                    HansoftXmlWriter.MapToHansoftXml(sourceReader, mapping);
                }
            }
        }

        private void addColumnButton_Click(object sender, EventArgs e)
        {
            new CustomColumnsList(mapping).ShowDialog();
        }

        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SuspendLayout2();
            if (processComboBoxChanges)
            {
                for (int i = 0; i < comboBoxes.Length; i += 1)
                {
                    if (comboBoxes[i] == sender)
                    {
                        Mapping.ColumnDefinition colDef;
                        if (comboBoxes[i].SelectedItem is string)
                            colDef = null;
                        else
                            colDef = (Mapping.ColumnDefinition)comboBoxes[i].SelectedItem;
                        foreach (Mapping.MappedColumn mappedCol in mapping.MappedColumns)
                        {
                            if (mappedCol.ExcelColumnIndex == i)
                            {
                                mappedCol.UpdateMappedHansoftColumn(colDef);
                                mapping.SetDirty(true);
                                break;
                            }
                        }
                    }
                }
            }
            ResumeLayout2();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SaveConditionallyIfDirty())
                e.Cancel = true;
        }

        public void SuspendLayout2()
        {
            tblLayout.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            base.SuspendLayout();
        }

        public void ResumeLayout2()
        {
            base.ResumeLayout();
            tblLayout.ResumeLayout();
            tableLayoutPanel2.ResumeLayout();
            tableLayoutPanel3.ResumeLayout();
            tableLayoutPanel4.ResumeLayout();
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            SuspendLayout2();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            ResumeLayout2();
        }

        private void viewReadmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string readmePath = System.IO.Path.Combine( Application.StartupPath, "Readme.html");
            System.Diagnostics.Process.Start(readmePath);
        }

        private void aboutHsimportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string aboutText = @"Terms and conditions
hsimport 1.1 by Svante Lidman (Hansoft AB) is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License.

This program is not part of the official Hansoft product or subject to its license agreement.
The program is provided as is and there is no obligation on Hansoft AB to provide support, update or enhance this program.

Questions can be sent to svante.lidman@hansoft.se and will be answered when other obligations so permit.";
            MessageBox.Show(aboutText, "About...");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
