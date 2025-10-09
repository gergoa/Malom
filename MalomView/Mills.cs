using Malom.Model;

namespace MalomView
{
    public partial class Mills : Form
    {
        private GameModel model;
        Button[] buttons = new Button[24];
        public Mills()
        {
            model = new GameModel();
            InitializeComponent();

            model.TilePlaced += TilePlaced;
            model.TileMoved += TileMoved;
            model.TileDeleted += TileDeleted;
            model.RoundProgressed += GameProgressed;

            this.Text = "Red to Placing; Round 1";

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

        private void GameProgressed(object? sender, MillsEventArgs e)
        {
            this.Text = e.PlayerOnTurn + " to " + e.NextAction + "; Round " + (e.Round + 1);
        }

        private void TileDeleted(object? sender, MillsTileEventArgs e)
        {
            buttons[e.Position].Text = "";

        }

        private void TileMoved(object? sender, MillsTileEventArgs e)
        {
            if (e.SelectedPosition == null) throw new Exception("SelectedPosition was null");
            buttons[(int)e.SelectedPosition].Text = "";
            buttons[e.Position].Text = e.PlayerOnTurn.ToString();
        }

        private void TilePlaced(object? sender, MillsTileEventArgs e)
        {
            buttons[e.Position].Text = e.PlayerOnTurn.ToString();
        }

        private void B_Click(object? sender, EventArgs e)
        {
            int buttonID = (int)((Button)sender).Tag - 1;
            model.Update(buttonID);
        }
    }
}
