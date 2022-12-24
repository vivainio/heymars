using CliWrap;
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

        public Form1(GuiLaunchEngine eng)
        {
            InitializeComponent();
            this.eventproc = new EventProcessor(commandGrid);
            this.eng = eng;
            this.eng.Listener = this.eventproc;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var c in eng.Commands)
            {
                int idx = commandGrid.Rows.Add(new[] { c.title ?? c.c, "" });
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
    }
}
