using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using Malom.Model;
using Malom.Persistence;
using System.Collections.ObjectModel;

namespace MalomWPF.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        //Fields
        private GameModel model;
        TileViewModel[] buttons = new TileViewModel[24];
        private readonly string starterPlayer = "Red";

        //Properties
        public int RoundTracker
        {
            get { return model.Steps + 1; }
            set
            {
                RoundTracker = value;
                OnPropertyChanged(nameof(RoundTracker));
            }
        }
        public string PlayerOnTurn
        {
            get { return model.PlayerOnTurn; }
            set
                {
                    PlayerOnTurn = value;
                    OnPropertyChanged(nameof(PlayerOnTurn));
                }
            }
        public int RedRemovedPieces
        {
            get { return model.RemovedRedPieces; }
            set
            {
                RedRemovedPieces = value;
                OnPropertyChanged(nameof(RedRemovedPieces));
            }
        }
        public int BlueRemovedPieces
        {
            get { return model.RemovedBluePieces; }
            set
            {
                BlueRemovedPieces = value;
                OnPropertyChanged(nameof(BlueRemovedPieces));
            }
        }

        public MainViewModel()
        {
            model = new GameModel(starterPlayer, new MillsFileHandler());

            InitializeHandlers();
            InitializeControls();

        }

        #region Methods

        private void InitializeHandlers()
        {
            model.TilePlaced += OnTilePlaced;
            model.TileMoved += OnTileMoved;
            model.TileDeleted += OnTileDeleted;
            model.RoundProgressed += OnGameProgressed;
            model.GameLoaded += OnGameLoaded;
            model.GameOver += OnGameOver;
        }

        private void OnGameOver(object? sender, MillsEventArgs e)
        {
            //TODO
        }

        private void OnGameLoaded(object? sender, MillsEventArgs e)
        {
            //Update view from model.
            RoundTracker = model.Steps + 1;
            PlayerOnTurn = model.PlayerOnTurn;
            RedRemovedPieces = model.RemovedRedPieces;
            BlueRemovedPieces = model.RemovedBluePieces;

            for (int i = 0; i < 24; i++)
            {
                string occupier = model.TableData.GetTile(i).Occupier.ToString();
                buttons[i].Occupier = occupier;
            }
        }

        private void InitializeControls()
        {
            for (int i = 0; i < 24; i++)
            {
                buttons[i] = new TileViewModel(i, "Empty", OnButtonClicked, (_) => true); //TODO
            }
        }
        #endregion

        #region Event Handling

        private void OnGameProgressed(object? sender, MillsEventArgs e)
        {
            RoundTracker = model.Steps + 1;
            PlayerOnTurn = model.PlayerOnTurn;
        }

            private void OnTileDeleted(object? sender, MillsTileEventArgs e)
            {
                int from = e.Position;
                buttons[from].Occupier = "Empty";
                OnPropertyChanged(nameof(RedRemovedPieces));
                OnPropertyChanged(nameof(BlueRemovedPieces));
            }

            private void OnTileMoved(object? sender, MillsTileEventArgs e)
            {
                int from = e.Position;
                int to = e!.SelectedPosition ?? -1;
                buttons[from].Occupier = "Empty";
                buttons[to].Occupier = model.TableData.GetTile(to).Occupier.ToString();
            }

        private void OnTilePlaced(object? sender, MillsTileEventArgs e)
        {
            if (PlayerOnTurn == "Red")
            {
                buttons[e.Position].Occupier = "Red";
            }
            else
            {
                buttons[e.Position].Occupier = "Blue";
            }
        }

        private void OnButtonClicked(TileViewModel button)
        {
            model.Update(button.Index);
        }


        private void OnSaveGame(object sender, EventArgs e)
        {
            /*
            //model.SaveGame("C:\\Users\\gergo\\Documents\\prog\\eva\\test.txt");
            using (SaveFileDialog FileDialog = new SaveFileDialog())
            {
                FileDialog.InitialDirectory = "C:\\";
                FileDialog.Filter = "Nine Men's Morris Files|*.nmm";
                FileDialog.RestoreDirectory = true;

                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!model.SaveGame(FileDialog.FileName))
                    {
                        MessageBox.Show("Saving game was unsuccessful!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }*/
        }

        private void OnNewGame(object sender, EventArgs e)
        {
            model = new(starterPlayer, new MillsFileHandler());
            InitializeHandlers();
            InitializeControls();
        }

        private void OnLoadGame(object sender, EventArgs e)
        {
            /*
            using (OpenFileDialog FileDialog = new OpenFileDialog())
            {
                FileDialog.InitialDirectory = "C:\\";
                FileDialog.Filter = "Nine Men's Morris Files (*nmm)|*.nmm";
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
            */
        }

        private void OnQuitGame(object sender, EventArgs e)
        {
            OnSaveGame(sender, e);
        }
    }
    #endregion
}
