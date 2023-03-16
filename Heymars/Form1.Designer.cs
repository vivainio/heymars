namespace GuiLaunch
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            commandGrid = new System.Windows.Forms.DataGridView();
            Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnLog = new System.Windows.Forms.Button();
            btnOutput = new System.Windows.Forms.Button();
            btnCls = new System.Windows.Forms.Button();
            cbSpeak = new System.Windows.Forms.CheckBox();
            linkHelp = new System.Windows.Forms.LinkLabel();
            lblDesc = new System.Windows.Forms.Label();
            btnEdit = new System.Windows.Forms.Button();
            cbCurrentConfig = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)commandGrid).BeginInit();
            SuspendLayout();
            // 
            // commandGrid
            // 
            commandGrid.AllowUserToAddRows = false;
            commandGrid.AllowUserToDeleteRows = false;
            commandGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            commandGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            commandGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            commandGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            commandGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Index, Command, Status });
            commandGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            commandGrid.Location = new System.Drawing.Point(12, 42);
            commandGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            commandGrid.Name = "commandGrid";
            commandGrid.RowHeadersVisible = false;
            commandGrid.RowHeadersWidth = 51;
            commandGrid.RowTemplate.Height = 24;
            commandGrid.ShowCellErrors = false;
            commandGrid.ShowCellToolTips = false;
            commandGrid.ShowEditingIcon = false;
            commandGrid.ShowRowErrors = false;
            commandGrid.Size = new System.Drawing.Size(1029, 495);
            commandGrid.TabIndex = 2;
            commandGrid.CellContentClick += commandGrid_CellContentClick;
            commandGrid.RowEnter += commandGrid_RowEnter;
            commandGrid.SelectionChanged += commandGrid_SelectionChanged;
            commandGrid.KeyDown += commandGrid_KeyDown_1;
            // 
            // Index
            // 
            Index.Frozen = true;
            Index.HeaderText = "#";
            Index.MinimumWidth = 6;
            Index.Name = "Index";
            Index.ReadOnly = true;
            Index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Index.Width = 24;
            // 
            // Command
            // 
            Command.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Command.HeaderText = "Command";
            Command.MinimumWidth = 6;
            Command.Name = "Command";
            Command.ReadOnly = true;
            Command.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            Status.DefaultCellStyle = dataGridViewCellStyle1;
            Status.HeaderText = "Status";
            Status.MinimumWidth = 200;
            Status.Name = "Status";
            Status.ReadOnly = true;
            Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Status.Width = 200;
            // 
            // btnLog
            // 
            btnLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnLog.Location = new System.Drawing.Point(12, 571);
            btnLog.Name = "btnLog";
            btnLog.Size = new System.Drawing.Size(94, 29);
            btnLog.TabIndex = 3;
            btnLog.Text = "Log";
            btnLog.UseVisualStyleBackColor = true;
            btnLog.Click += button1_Click;
            // 
            // btnOutput
            // 
            btnOutput.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnOutput.Location = new System.Drawing.Point(112, 571);
            btnOutput.Name = "btnOutput";
            btnOutput.Size = new System.Drawing.Size(94, 29);
            btnOutput.TabIndex = 4;
            btnOutput.Text = "Output";
            btnOutput.UseVisualStyleBackColor = true;
            btnOutput.Click += button1_Click_1;
            // 
            // btnCls
            // 
            btnCls.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnCls.Location = new System.Drawing.Point(212, 571);
            btnCls.Name = "btnCls";
            btnCls.Size = new System.Drawing.Size(94, 29);
            btnCls.TabIndex = 5;
            btnCls.Text = "Cls";
            btnCls.UseVisualStyleBackColor = true;
            btnCls.Click += btnCls_Click;
            // 
            // cbSpeak
            // 
            cbSpeak.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cbSpeak.AutoSize = true;
            cbSpeak.Location = new System.Drawing.Point(948, 578);
            cbSpeak.Name = "cbSpeak";
            cbSpeak.Size = new System.Drawing.Size(71, 24);
            cbSpeak.TabIndex = 6;
            cbSpeak.Text = "Speak";
            cbSpeak.UseVisualStyleBackColor = true;
            // 
            // linkHelp
            // 
            linkHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            linkHelp.AutoSize = true;
            linkHelp.Location = new System.Drawing.Point(412, 575);
            linkHelp.Name = "linkHelp";
            linkHelp.Size = new System.Drawing.Size(16, 20);
            linkHelp.TabIndex = 7;
            linkHelp.TabStop = true;
            linkHelp.Text = "?";
            linkHelp.LinkClicked += linkHelp_LinkClicked;
            // 
            // lblDesc
            // 
            lblDesc.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblDesc.AutoSize = true;
            lblDesc.Location = new System.Drawing.Point(12, 541);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new System.Drawing.Size(50, 20);
            lblDesc.TabIndex = 8;
            lblDesc.Text = "label1";
            // 
            // btnEdit
            // 
            btnEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnEdit.Location = new System.Drawing.Point(312, 571);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new System.Drawing.Size(94, 29);
            btnEdit.TabIndex = 9;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // cbCurrentConfig
            // 
            cbCurrentConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbCurrentConfig.FormattingEnabled = true;
            cbCurrentConfig.Location = new System.Drawing.Point(12, 7);
            cbCurrentConfig.Name = "cbCurrentConfig";
            cbCurrentConfig.Size = new System.Drawing.Size(1025, 28);
            cbCurrentConfig.TabIndex = 10;
            cbCurrentConfig.SelectedIndexChanged += cbCurrentConfig_SelectedIndexChanged;

            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1053, 612);
            Controls.Add(cbCurrentConfig);
            Controls.Add(btnEdit);
            Controls.Add(lblDesc);
            Controls.Add(linkHelp);
            Controls.Add(cbSpeak);
            Controls.Add(btnCls);
            Controls.Add(btnOutput);
            Controls.Add(btnLog);
            Controls.Add(commandGrid);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Heymars";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)commandGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.DataGridView commandGrid;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.Button btnCls;
        private System.Windows.Forms.CheckBox cbSpeak;
        private System.Windows.Forms.LinkLabel linkHelp;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.ComboBox cbCurrentConfig;
    }
}

