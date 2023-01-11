using CliWrap;
using Heymars;
using Heymars.Properties;
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
        private readonly EventProcessor _eventproc;
        private readonly GuiLaunchEngine _eng;
        private readonly LauncherSettings _settings;
        private readonly Lazy<OutputViewForm> outputViewForm = new Lazy<OutputViewForm>(() =>  new OutputViewForm());
        public Form1(GuiLaunchEngine eng, LauncherSettings settings)
        {
            InitializeComponent();
            this._eventproc = new EventProcessor(commandGrid, eng);
            _eventproc.ShouldSpeak = ShouldSpeak;
            this._eng = eng;
            this._settings = settings;
            this._eng.Listener = this._eventproc;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _eng.DrawGrid(commandGrid);
            _eng.StartPolling();
            if (_settings.RunAll)
            {
                _eng.RunAll().ContinueWith(t =>
                {
                    ;
                });
            }

            if (_settings.Run != null)
            {
                _eng.RunCommands(_settings.Run.Split(",")).ContinueWith(t => { });
            }

        }


        private async void commandGrid_KeyDown_1(object sender, KeyEventArgs e)
        {
            int index = commandGrid.CurrentCell.RowIndex;
            var supress = await _eng.KeyPress(e.KeyCode, index);
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
            if (_eng.LogStream == null)
            {
                _eng.StartLog();
                btnLog.Text = "Stop log";
            } else
            {
                _eng.StopLog();
                btnLog.Text = "Log";
            }

        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            _eng.CollectOutput = true;
            outputViewForm.Value.Show();
            outputViewForm.Value.BringToFront();
            ShowOutput(commandGrid.CurrentCell.RowIndex);
           
            

        }
        private void ShowOutput(int index)
        {
            if (!_eng.CollectOutput)
                return;
            var command = _eng.Commands[index];
            var title = command.title ?? command.c;
    
            var o = _eng.GetOutput(index);
            var scintilla = outputViewForm.Value.Scintilla;
            outputViewForm.Value.Text = "Heymars: " + title;
            if (o == null)
            {
                SciUtil.SetAllText(scintilla, "No output");
                return;
            }

            SciUtil.SetAllText(scintilla, o);
#pragma warning disable CA1416 // Validate platform compatibility
            scintilla.ExecuteCmd(ScintillaNET.Command.DocumentEnd);
#pragma warning restore CA1416 // Validate platform compatibility

        }
        private void commandGrid_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void commandGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
          
            ShowOutput(e.RowIndex);

        }

        private void btnCls_Click(object sender, EventArgs e)
        {

            _eng.ClearStatuses();
        }

        private bool ShouldSpeak()
        {
            if (!cbSpeak.Checked) return false;
            if (Form.ActiveForm!= null) return false;
            return true;

        }
        private void cbSpeak_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
