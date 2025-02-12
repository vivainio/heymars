using System;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace GuiLaunch
{
    public class EventProcessor : IProcessEvents
    {
        private readonly SpeechSynthesizer _synth;
        private readonly GuiLaunchEngine _eng;
        private readonly Form1 _form;
        private readonly DataGridView grid;
        public Func<bool> ShouldSpeak { get; set; }

        public EventProcessor(DataGridView grid, GuiLaunchEngine eng, Form1 form)
        {
            _synth = new SpeechSynthesizer();
            _eng = eng;
            _form = form;
            this.grid = grid;
        }

        public void ProcessStatusChanged(int index, string status)
        {
            grid.Rows[index].Cells[2].Value = status;
        }

        public void SpeakStatus(string message)
        {
            if (ShouldSpeak == null || !ShouldSpeak())
                return;
            _synth.SpeakAsync(new Prompt(message));
        }

        public void RepaintNeeded()
        {
            _eng.DrawGrid(grid);
        }

        public void Log(string message)
        {
            _form.LogMessage(message);
        }
    }
}
