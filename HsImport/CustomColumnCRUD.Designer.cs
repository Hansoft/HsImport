namespace HsImport
{
    partial class CustomColumnCRUD
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.displayNameLabel = new System.Windows.Forms.Label();
            this.codeNameLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.displayNameTextBox = new System.Windows.Forms.TextBox();
            this.codeNameTextBox = new System.Windows.Forms.TextBox();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.optionsListBox = new System.Windows.Forms.ListBox();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayNameLabel
            // 
            this.displayNameLabel.AutoSize = true;
            this.displayNameLabel.Location = new System.Drawing.Point(12, 9);
            this.displayNameLabel.Name = "displayNameLabel";
            this.displayNameLabel.Size = new System.Drawing.Size(75, 13);
            this.displayNameLabel.TabIndex = 0;
            this.displayNameLabel.Text = "Display Name:";
            // 
            // codeNameLabel
            // 
            this.codeNameLabel.AutoSize = true;
            this.codeNameLabel.Location = new System.Drawing.Point(12, 60);
            this.codeNameLabel.Name = "codeNameLabel";
            this.codeNameLabel.Size = new System.Drawing.Size(66, 13);
            this.codeNameLabel.TabIndex = 1;
            this.codeNameLabel.Text = "Code Name:";
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Location = new System.Drawing.Point(9, 110);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(34, 13);
            this.typeLabel.TabIndex = 2;
            this.typeLabel.Text = "Type:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(412, 259);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(493, 259);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // displayNameTextBox
            // 
            this.displayNameTextBox.Location = new System.Drawing.Point(12, 25);
            this.displayNameTextBox.Name = "displayNameTextBox";
            this.displayNameTextBox.Size = new System.Drawing.Size(229, 20);
            this.displayNameTextBox.TabIndex = 0;
            this.displayNameTextBox.TextChanged += new System.EventHandler(this.displayNameTextBox_TextChanged);
            // 
            // codeNameTextBox
            // 
            this.codeNameTextBox.Location = new System.Drawing.Point(12, 76);
            this.codeNameTextBox.Name = "codeNameTextBox";
            this.codeNameTextBox.Size = new System.Drawing.Size(229, 20);
            this.codeNameTextBox.TabIndex = 1;
            // 
            // typeComboBox
            // 
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(12, 126);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(229, 21);
            this.typeComboBox.TabIndex = 2;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.deleteButton);
            this.optionsGroupBox.Controls.Add(this.editButton);
            this.optionsGroupBox.Controls.Add(this.newButton);
            this.optionsGroupBox.Controls.Add(this.optionsListBox);
            this.optionsGroupBox.Controls.Add(this.downButton);
            this.optionsGroupBox.Controls.Add(this.upButton);
            this.optionsGroupBox.Enabled = false;
            this.optionsGroupBox.Location = new System.Drawing.Point(270, 12);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(309, 241);
            this.optionsGroupBox.TabIndex = 12;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // deleteButton
            // 
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(171, 201);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(67, 23);
            this.deleteButton.TabIndex = 6;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // editButton
            // 
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(89, 201);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(63, 23);
            this.editButton.TabIndex = 5;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(7, 201);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(61, 23);
            this.newButton.TabIndex = 4;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // optionsListBox
            // 
            this.optionsListBox.FormattingEnabled = true;
            this.optionsListBox.Location = new System.Drawing.Point(6, 21);
            this.optionsListBox.Name = "optionsListBox";
            this.optionsListBox.Size = new System.Drawing.Size(232, 173);
            this.optionsListBox.TabIndex = 3;
            this.optionsListBox.SelectedIndexChanged += new System.EventHandler(this.optionsListBox_SelectedIndexChanged);
            this.optionsListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.optionsListBox_MouseDoubleClick);
            // 
            // downButton
            // 
            this.downButton.Enabled = false;
            this.downButton.Location = new System.Drawing.Point(244, 114);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(52, 23);
            this.downButton.TabIndex = 8;
            this.downButton.Text = "Down";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // upButton
            // 
            this.upButton.Enabled = false;
            this.upButton.Location = new System.Drawing.Point(244, 88);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(52, 23);
            this.upButton.TabIndex = 7;
            this.upButton.Text = "Up";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // CustomColumnCRUD
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(599, 311);
            this.ControlBox = false;
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.typeComboBox);
            this.Controls.Add(this.codeNameTextBox);
            this.Controls.Add(this.displayNameTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.codeNameLabel);
            this.Controls.Add(this.displayNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CustomColumnCRUD";
            this.Text = "Create/Modify Custom Column";
            this.optionsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label displayNameLabel;
        private System.Windows.Forms.Label codeNameLabel;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox displayNameTextBox;
        private System.Windows.Forms.TextBox codeNameTextBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.ListBox optionsListBox;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button newButton;
    }
}