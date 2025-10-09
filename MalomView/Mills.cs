using Malom.Model;

namespace MalomView
{
    public partial class Mills : Form
    {
        private GameModel model;
        Button[] buttons = new Button[24];
        private const int initialWidth = 1000;
        private const float winRatio = 1.8f;
        public Mills()
        {
            model = new GameModel();
            InitializeComponent();

            model.TilePlaced += TilePlaced;
            model.TileMoved += TileMoved;
            model.TileDeleted += TileDeleted;
            model.RoundProgressed += GameProgressed;
            //this.Resize += UISizeChanged;

            InitializeControls();
            InitializeGraphics();

        }

        /*
        private void UISizeChanged(object? sender, EventArgs e)
        {
            float scaleFactor = (float)this.Width / (float)initialWidth;
            this.Size = new Size(this.Width, (int)(this.Width / winRatio));
            panel1.Size = new Size((int)(panel1.Width * scaleFactor), (int)(panel1.Height * scaleFactor));
        }*/

        #region Methods

        private void InitializeControls()
        {
            foreach (Control control in panel1.Controls)
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
                }
            }
        }
        private void InitializeGraphics()
        {
            this.Text = "Red to Placing; Round 1";
            this.Size = new Size(initialWidth, (int)(initialWidth / winRatio));
            this.MinimumSize = this.Size;
            panel1.MinimumSize = panel1.Size;

            textBoxRedPieces.Text = "";
            textBoxRedPieces.ForeColor = Color.Red;
            textBoxBluePieces.Text = "";
            textBoxBluePieces.ForeColor = Color.Blue;

            foreach (Control control in panel1.Controls)
            {
                if (control is Button b)
                {
                    b.Text = "";
                    b.BackColor = Color.Transparent;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 255, 255, 255);
                }

            }
        }
        #endregion

        #region Event Handling

        private void GameProgressed(object? sender, MillsEventArgs e)
        {
            this.Text = e.PlayerOnTurn + " to " + e.NextAction + "; Round " + (e.Round + 1);
            if (e.PlayerOnTurn == "Red")
            {
                textBoxRed.BackColor = Color.LightYellow;
                textBoxBlue.BackColor = Color.White;
            }
            else
            {
                textBoxBlue.BackColor = Color.LightYellow;
                textBoxRed.BackColor = Color.White;
            }
        }

        private void TileDeleted(object? sender, MillsTileEventArgs e)
        {
            buttons[e.Position].ForeColor = Color.Transparent;
            buttons[e.Position].Text = "";
            buttons[e.Position].FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 255, 255, 255);

            if (e.PlayerOnTurn == "Blue")
            {
                textBoxRedPieces.Text = textBoxRedPieces.Text + "\u274C";
            }
            else
            {
                textBoxBluePieces.Text = textBoxBluePieces.Text + "\u274C";
            }

        }

        private void TileMoved(object? sender, MillsTileEventArgs e)
        {
            if (e.SelectedPosition == null) throw new Exception("SelectedPosition was null");
            buttons[(int)e.SelectedPosition].ForeColor = Color.Transparent;
            buttons[(int)e.SelectedPosition].FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 255, 255, 255);
            buttons[(int)e.SelectedPosition].Text = "";
            buttons[e.Position].ForeColor = e.PlayerOnTurn == "Red" ? Color.Red : Color.Blue;
            buttons[e.Position].FlatAppearance.MouseOverBackColor = e.PlayerOnTurn == "Red" ? Color.DarkRed : Color.DarkBlue;
            buttons[e.Position].Text = "\u25C9";
        }

        private void TilePlaced(object? sender, MillsTileEventArgs e)
        {
            buttons[e.Position].ForeColor = e.PlayerOnTurn == "Red" ? Color.Red : Color.Blue;
            buttons[e.Position].FlatAppearance.MouseOverBackColor = e.PlayerOnTurn == "Red" ? Color.DarkRed : Color.DarkBlue;
            buttons[e.Position].Text = "\u25C9";
        }

        private void B_Click(object? sender, EventArgs e)
        {
            int buttonID = (int)((Button)sender).Tag - 1;
            model.Update(buttonID);
        }

        #endregion

        private void Mills_Load(object sender, EventArgs e)
        {

        }
    }
}
