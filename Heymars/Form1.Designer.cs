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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.commandGrid = new System.Windows.Forms.DataGridView();
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
            this.commandGrid.Location = new System.Drawing.Point(-2, 0);
            this.commandGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.commandGrid.Name = "commandGrid";
            this.commandGrid.RowHeadersVisible = false;
            this.commandGrid.RowHeadersWidth = 51;
            this.commandGrid.RowTemplate.Height = 24;
            this.commandGrid.ShowCellErrors = false;
            this.commandGrid.ShowEditingIcon = false;
            this.commandGrid.ShowRowErrors = false;
            this.commandGrid.Size = new System.Drawing.Size(1055, 598);
            this.commandGrid.TabIndex = 2;
            this.commandGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.commandGrid_CellContentClick);
            this.commandGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commandGrid_KeyDown_1);
            // 
            // Index
            // 
            this.Index.Frozen = true;
            this.Index.HeaderText = "#";
            this.Index.MinimumWidth = 6;
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Width = 47;
            // 
            // Command
            // 
            this.Command.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Command.HeaderText = "Command";
            this.Command.MinimumWidth = 6;
            this.Command.Name = "Command";
            this.Command.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.MinimumWidth = 6;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.Width = 78;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 612);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}

