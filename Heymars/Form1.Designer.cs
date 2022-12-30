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
            this.commandGrid = new System.Windows.Forms.DataGridView();
            this.btnLog = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.commandGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // commandGrid
            // 
            this.commandGrid.AllowUserToAddRows = false;
            this.commandGrid.AllowUserToDeleteRows = false;
            this.commandGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.commandGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.commandGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.commandGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.Command,
            this.Status});
            this.commandGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.commandGrid.Location = new System.Drawing.Point(12, 13);
            this.commandGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.commandGrid.Name = "commandGrid";
            this.commandGrid.RowHeadersVisible = false;
            this.commandGrid.RowHeadersWidth = 51;
            this.commandGrid.RowTemplate.Height = 24;
            this.commandGrid.ShowCellErrors = false;
            this.commandGrid.ShowEditingIcon = false;
            this.commandGrid.ShowRowErrors = false;
            this.commandGrid.Size = new System.Drawing.Size(1029, 551);
            this.commandGrid.TabIndex = 2;
            this.commandGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.commandGrid_CellContentClick);
            this.commandGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.commandGrid_RowEnter);
            this.commandGrid.SelectionChanged += new System.EventHandler(this.commandGrid_SelectionChanged);
            this.commandGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commandGrid_KeyDown_1);
            // 
            // btnLog
            // 
            this.btnLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLog.Location = new System.Drawing.Point(12, 571);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(94, 29);
            this.btnLog.TabIndex = 3;
            this.btnLog.Text = "Log";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOutput.Location = new System.Drawing.Point(112, 571);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(94, 29);
            this.btnOutput.TabIndex = 4;
            this.btnOutput.Text = "Output";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Index
            // 
            this.Index.Frozen = true;
            this.Index.HeaderText = "#";
            this.Index.MinimumWidth = 6;
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Index.Width = 24;
            // 
            // Command
            // 
            this.Command.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Command.HeaderText = "Command";
            this.Command.MinimumWidth = 6;
            this.Command.Name = "Command";
            this.Command.ReadOnly = true;
            this.Command.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Status.DefaultCellStyle = dataGridViewCellStyle1;
            this.Status.HeaderText = "Status";
            this.Status.MinimumWidth = 200;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Status.Width = 200;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 612);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.commandGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Heymars";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.commandGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView commandGrid;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}

