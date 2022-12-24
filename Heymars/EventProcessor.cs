using System.Windows.Forms;

namespace GuiLaunch
{
    public class EventProcessor : IProcessEvents
    {
        private readonly DataGridView grid;

        public EventProcessor(DataGridView grid)
        {
            this.grid = grid;
        }
        public void ProcessStatusChanged(int index, string status)
        {
            grid.Rows[index].Cells[1].Value = status;
        }
    }
}
