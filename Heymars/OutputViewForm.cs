using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heymars
{
    public partial class OutputViewForm : Form
    {
        public OutputViewForm()
        {
            InitializeComponent();
            this.Scintilla = SciUtil.CreateScintilla();
            Controls.Add(Scintilla);
            Scintilla.Dock = DockStyle.Fill;
            FormClosing += (o, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                }
            };

        }

        public Scintilla Scintilla { get; }
    }
}
