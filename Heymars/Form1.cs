﻿using CliWrap;
using Heymars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiLaunch
{
    public partial class Form1 : Form
    {
        private readonly EventProcessor eventproc;
        private readonly GuiLaunchEngine eng;

        private readonly Lazy<OutputViewForm> outputViewForm = new Lazy<OutputViewForm>(() =>  new OutputViewForm());
        public Form1(GuiLaunchEngine eng)
        {
            InitializeComponent();
            this.eventproc = new EventProcessor(commandGrid);
            this.eng = eng;
            this.eng.Listener = this.eventproc;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int index = 0; index < eng.Commands.Count(); index++)
            {
                var c = eng.Commands[index];
                int idx = commandGrid.Rows.Add(new[] { (object) index, (string) c.title ?? c.c, "" });
                commandGrid.Rows[idx].Cells[0].ToolTipText = c.ToString();

            }
        }



        private async void commandGrid_KeyDown_1(object sender, KeyEventArgs e)
        {
            int index = commandGrid.CurrentCell.RowIndex;
            var supress = await eng.KeyPress(e.KeyCode, index);
            e.Handled = supress;
            e.SuppressKeyPress = supress;
            //e.SuppressKeyPress

        }

        private void commandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void commandGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (eng.LogStream == null)
            {
                eng.StartLog();
                btnLog.Text = "Stop log";
            } else
            {
                eng.StopLog();
                btnLog.Text = "Log";
            }

        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            eng.CollectOutput = true;
            outputViewForm.Value.Show();
            outputViewForm.Value.BringToFront();
            ShowOutput(commandGrid.CurrentCell.RowIndex);
           
            

        }
        private void ShowOutput(int index)
        {
            if (!eng.CollectOutput)
                return;
            var command = eng.Commands[index];
            var title = command.title ?? command.c;
    
            var o = eng.GetOutput(index);
            var scintilla = outputViewForm.Value.Scintilla;
            outputViewForm.Value.Text = "Heymars: " + title;
            if (o == null)
            {
                SciUtil.SetAllText(scintilla, "No output");
                return;
            }

            SciUtil.SetAllText(scintilla, o);
            scintilla.ExecuteCmd(ScintillaNET.Command.DocumentEnd);

        }
        private void commandGrid_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void commandGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
          
            ShowOutput(e.RowIndex);

        }
    }
}
