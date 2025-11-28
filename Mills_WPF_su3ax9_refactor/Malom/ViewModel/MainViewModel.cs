using System.Collections.ObjectModel;
using System.IO;
using Malom.Model;
using Malom.Persistence;

namespace Malom.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        //Fields
        readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private GameModel model;
        private ObservableCollection<TileViewModel> _boardTiles = new ObservableCollection<TileViewModel>();
        private readonly string starterPlayer;
        private IDialogService _dialogService;
        private string currentAction = "Placing";

        List<(int, int)> layout = new();
        //Properties
        public string RoundTracker
        {
            get { return $"Round: {model.Steps + 1}" + $"\nCurrently {PlayerOnTurn} is {currentAction}"; }
        }
        public string PlayerOnTurn
        {
            get { return model.PlayerOnTurn; }
        }
        public int RedRemovedPieces
        {
            get { return model.RemovedRedPieces; }
        }
        public int BlueRemovedPieces
        {
            get { return model.RemovedBluePieces; }
        }
        public ObservableCollection<TileViewModel> BoardTiles
        {
            get { return _boardTiles; }
        }
        //Commands
        public DelegateCommand NewGameCommand => new DelegateCommand(_ => NewGame());
        public DelegateCommand LoadGameCommand => new DelegateCommand(_ => LoadGame());
        public DelegateCommand SaveGameCommand => new DelegateCommand(_ => SaveGame());
        public DelegateCommand QuitGameCommand => new DelegateCommand(_ => QuitGame(true));
        public MainViewModel(IDialogService dialogService)
        {

            starterPlayer = Random.Shared.Next(0, 2) == 0 ? "Red" : "Blue";
            model = new GameModel(starterPlayer, new MillsFileHandler());

            _dialogService = dialogService;
            InitializeBoardLayout();
            InitializeHandlers();
            InitializeControls();
        }

        public MainViewModel(string starterPlyer, IDialogService dialogService)
        {
            starterPlayer = starterPlyer;
            model = new GameModel(starterPlayer, new MillsFileHandler());

            _dialogService = dialogService;
            InitializeBoardLayout();
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
            if (e.NextAction == "New Game")
            {
                NewGame();
                return;
            }


            bool res = _dialogService.DisplayYesNoMessageBox(
                                    $"{PlayerOnTurn} player has won the game! Would you like to start a new game?",
                                    "Game Over");

            if (res)
            {
                NewGame();
            }
            else
            {
                QuitGame(false);
            }
            //TODO
        }

        private void OnGameLoaded(object? sender, MillsEventArgs e)
        {
            _boardTiles = [];
            //Update view from model
            for (int i = 0; i < 24; i++)
            {
                Tile m_tile = model.TableData.GetTile(i);
                _boardTiles.Add(new(i, m_tile.Occupier, OnButtonClicked, (_) => true, layout[i]));
            }
            OnPropertyChanged(nameof(RoundTracker));
            OnPropertyChanged(nameof(PlayerOnTurn));
            OnPropertyChanged(nameof(RedRemovedPieces));
            OnPropertyChanged(nameof(BlueRemovedPieces));
            OnPropertyChanged(nameof(BoardTiles));
        }

        private void InitializeControls()
        {
            _boardTiles.Clear();
            for (int i = 0; i < 24; i++)
            {
                _boardTiles.Add(new(i, Player.Empty, OnButtonClicked, (_) => true, layout[i]));
            }

            OnPropertyChanged(nameof(RoundTracker));
            OnPropertyChanged(nameof(PlayerOnTurn));
            OnPropertyChanged(nameof(RedRemovedPieces));
            OnPropertyChanged(nameof(BlueRemovedPieces));
            OnPropertyChanged(nameof(BoardTiles));
        }
        #endregion

        #region Event Handling

        private void OnGameProgressed(object? sender, MillsEventArgs e)
        {
            currentAction = e.NextAction;
            OnPropertyChanged(nameof(RoundTracker));
            OnPropertyChanged(nameof(PlayerOnTurn));
        }

        private void OnTileDeleted(object? sender, MillsTileEventArgs e)
        {
            int from = e.Position;
            _boardTiles[from].Occupier = "Empty";
            OnPropertyChanged(nameof(RedRemovedPieces));
            OnPropertyChanged(nameof(BlueRemovedPieces));
        }

        private void OnTileMoved(object? sender, MillsTileEventArgs e)
        {
            int to = e.Position;
            int from = e.SelectedPosition ?? throw new Exception();
            _boardTiles[from].Occupier = "Empty";
            _boardTiles[to].Occupier = PlayerOnTurn;
        }

        private void OnTilePlaced(object? sender, MillsTileEventArgs e)
        {
            if (PlayerOnTurn == "Red")
            {
                _boardTiles[e.Position].Occupier = "Red";
            }
            else
            {
                _boardTiles[e.Position].Occupier = "Blue";
            }
        }

        private void OnButtonClicked(TileViewModel button)
        {
            model.Update(button.Index);
        }


        private void SaveGame()
        {
            Directory.CreateDirectory(Path.Combine(baseDirectory, "Saves"));
            string path = _dialogService.DisplaySaveFileDialog(Path.Combine(baseDirectory, "Saves"), "Nine Men's Morris Files (*.nmm)|*.nmm");
            if (!string.IsNullOrEmpty(path))
            {
                model.SaveGame(path);
            }
        }

        private void NewGame()
        {
            model = new GameModel(starterPlayer, new MillsFileHandler());
            InitializeHandlers();
            InitializeControls();
        }

        private void LoadGame()
        {
            string path = _dialogService.DisplayOpenFileDialog(Path.Combine(baseDirectory, "Saves"), "Nine Men's Morris Files (*.nmm)|*.nmm");
            if (!string.IsNullOrEmpty(path))
            {
                model.LoadGame(path);
            }
        }

        private void QuitGame(bool want_save)
        {
            if (want_save)
            {
                if (_dialogService.DisplayYesNoMessageBox(
                "Would you like to save your progress?",
                "Quit Game")) SaveGame();
            }
            _dialogService.CloseApplication();
        }
        private void InitializeBoardLayout()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    bool isMiddle = (i == 3 || j == 3);
                    bool isDiagonal = (i == j || (i + j) == 6);
                    bool isCenter = (i == 3 && j == 3);

                    if ((isMiddle || isDiagonal) && !isCenter)
                    {
                        layout.Add((i, j));
                    }
                }
            }
        }
    }
    #endregion
}
