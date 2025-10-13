using Malom.Model;

namespace MalomView
{
    public partial class Mills : Form
    {
        private GameModel model;
        Button[] buttons = new Button[24];
        private const int initialWidth = 1000;
        private const float winRatio = 1.8f;
        private const string starterPlayer = "Red";
        public Mills()
        {
            model = new GameModel(starterPlayer);
            InitializeComponent();

            model.TilePlaced += TilePlaced;
            model.TileMoved += TileMoved;
            model.TileDeleted += TileDeleted;
            model.RoundProgressed += GameProgressed;
            model.GameOver += (s, e) =>
            {
                MessageBox.Show(model.PlayerOnTurn + " wins!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            };
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
            if (starterPlayer == "Red")
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
        #endregion

        #region Event Handling

        private void GameProgressed(object? sender, MillsEventArgs e)
        {
            roundCounter.Text = "Round: " + (model.Steps+ 1).ToString();
            if (model.PlayerOnTurn == "Red")
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

            if (model.PlayerOnTurn == "Blue")
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
            buttons[(int)e.SelectedPosition].Text = "";
            buttons[e.Position].ForeColor = model.PlayerOnTurn == "Red" ? Color.Red : Color.Blue;
            buttons[e.Position].Text = "\u25C9";
        }

        private void TilePlaced(object? sender, MillsTileEventArgs e)
        {
            buttons[e.Position].ForeColor = model.PlayerOnTurn == "Red" ? Color.Red : Color.Blue;
            buttons[e.Position].Text = "\u25C9";
        }

        private void B_Click(object? sender, EventArgs e)
        {
            int buttonID = (int)((Button)sender).Tag - 1;
            model.Update(buttonID);
        }

        #endregion

        private void saveGame(object sender, EventArgs e)
        {
            model.SaveGame("C:\\Users\\gergo\\Documents\\prog\\eva\\test.txt");
        }
    }
}
