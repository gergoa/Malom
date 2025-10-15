using Malom.Model;

namespace MalomView
{
    public partial class Mills : Form
    {
        private GameModel model;
        Button[] buttons = new Button[24];
        private readonly int initialWidth = 1000;
        private readonly float winRatio = 1.8f;
        private readonly string starterPlayer = "Blue";
        public Mills()
        {
            model = new GameModel(starterPlayer);
            InitializeComponent();

            InitializeHandlers();
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

        private void InitializeHandlers()
        {
            model.TilePlaced += TilePlaced;
            model.TileMoved += TileMoved;
            model.TileDeleted += TileDeleted;
            model.RoundProgressed += GameProgressed;
            model.GameLoaded += GameLoaded;
            model.GameOver += GameOver;
        }

        private void GameOver(object? sender, MillsEventArgs e)
        {
            if (MessageBox.Show(model.PlayerOnTurn + " has won the game!\n" + "Would you like to start a new game?",
                        "New Game", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                OnNewGame(sender!, new());
            }
            else
            {
                Application.Exit();
            }
        }

        private void GameLoaded(object? sender, MillsEventArgs e)
        {
            //Update view from model.
            roundTrackerLabel.Text = "Round: " + (model.TableData.Steps + 1).ToString();
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

            playerTurnLabel.Text = "Player on turn: " + model.PlayerOnTurn + "; Currently " + e.NextAction;

            textBoxRedPieces.Text = new('\u274C', model.TableData.RemovedRedPieces);
            textBoxBluePieces.Text = new ('\u274C', model.TableData.RemovedBluePieces);

            for (int i = 0; i < 24; i++)
            {
                switch (model.TableData.GetTile(i).Occupier.ToString())
                {
                    case "Red":
                        buttons[i].ForeColor = Color.Red;
                        buttons[i].Text = "\u25C9";
                        break;
                    case "Blue":
                        buttons[i].ForeColor = Color.Blue;
                        buttons[i].Text = "\u25C9";
                        break;
                    case "Empty":
                        buttons[i].ForeColor = Color.Transparent;
                        buttons[i].Text = "";
                        break;
                }
            }
        }

        private void InitializeControls()
        {
            foreach (Control control in panel1.Controls)
            {
                if (control is Button b)
                {
                    b.Click += OnButtonClicked;
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

            playerTurnLabel.Text = "Player on turn: " + starterPlayer + " ,Currently Placing";
            roundTrackerLabel.Text = "Round: 1";
        }
        #endregion

        #region Event Handling

        private void GameProgressed(object? sender, MillsEventArgs e)
        {
            roundTrackerLabel.Text = "Round: " + (model.TableData.Steps + 1).ToString();
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

            playerTurnLabel.Text = "Player on turn: " + model.PlayerOnTurn + " ,Currently " + e.NextAction;
        }

        private void TileDeleted(object? sender, MillsTileEventArgs e)
        {
            buttons[e.Position].ForeColor = Color.Transparent;
            buttons[e.Position].Text = "";

            if (model.PlayerOnTurn == "Blue")
            {
                textBoxRedPieces.Text = new('\u274C',model.TableData.RemovedRedPieces);
            }
            else
            {
                textBoxBluePieces.Text = new('\u274C', model.TableData.RemovedBluePieces);
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

        private void OnButtonClicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int buttonID = (int)((Button)sender).Tag! - 1;
                model.Update(buttonID);
            }
        }


        private void OnSaveGame(object sender, EventArgs e)
        {
            //model.SaveGame("C:\\Users\\gergo\\Documents\\prog\\eva\\test.txt");
            using (SaveFileDialog FileDialog = new SaveFileDialog())
            {
                FileDialog.InitialDirectory = "C:\\";
                FileDialog.Filter = "Text files (*.txt)|*.txt";
                FileDialog.RestoreDirectory = true;

                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!model.SaveGame(FileDialog.FileName))
                    {
                        MessageBox.Show("Saving game was unsuccessful!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnNewGame(object sender, EventArgs e)
        {
            model = new(starterPlayer);
            InitializeGraphics();
            InitializeHandlers();
        }

        private void OnLoadGame(object sender, EventArgs e)
        {
            using (OpenFileDialog FileDialog = new OpenFileDialog())
            {
                FileDialog.InitialDirectory = "C:\\";
                FileDialog.Filter = "Text files (*.txt)|*.txt";
                FileDialog.RestoreDirectory = true;

                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!model.LoadGame(FileDialog.FileName))
                    {
                        MessageBox.Show("Loading game was unsuccessful!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnQuitGame(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you like to save your game before quitting?",
                        "Quit Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                OnSaveGame(sender, e);
            }
            Application.Exit();
        }
    }
    #endregion
}
