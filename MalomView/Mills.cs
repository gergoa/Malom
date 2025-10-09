using Malom.Model;

namespace MalomView
{
    public partial class Mills : Form
    {
        private GameModel model;
        Button[] buttons = new Button[24];
        private MillsEventArgs? previous;
        public Mills()
        {
            model = new GameModel();
            InitializeComponent();

            //very debug
            model.TileClicked += Model_TileClicked;

            this.Text = model.PlayerOnTurn;
            //

            foreach (Control control in this.Controls)
            {
                if (control is Button b)
                {
                    b.Click += B_Click;
                    b.Tag = int.Parse(String.Join("",
                        b.Text.Where(x => 
                        Char.IsNumber(x))
                        .ToList())
                        );
                    buttons[(int)b.Tag - 1] = b;
                    b.Text = "";
                }
            }
        }

        //TODO: refactor
        private void Model_TileClicked(object? sender, MillsEventArgs e)
        {
            this.Text = e.Steps.ToString() + " " + e.PlayerOnTurn.ToString() + " " + e.Action;
            previous = e;
        }

        private void B_Click(object? sender, EventArgs e)
        {
            if (model.Update((int)((Button)sender).Tag - 1))
            ((Control)sender).Text = previous.PlayerOnTurn;
        }
    }
}
