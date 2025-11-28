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
        private TileViewModel[] buttons = new TileViewModel[24];
        private readonly string starterPlayer;
        private IDialogService _dialogService;
        private string currentAction = "Placing";
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
        public TileViewModel[] Buttons
        {
            get { return buttons; }
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
            InitializeHandlers();
            InitializeControls();
        }

        public MainViewModel(string starterPlyer, IDialogService dialogService)
        {
            starterPlayer = starterPlyer;
            model = new GameModel(starterPlayer, new MillsFileHandler());

            _dialogService = dialogService;
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
            //Update view from model.

            for (int i = 0; i < 24; i++)
            {
                string occupier = model.TableData.GetTile(i).Occupier.ToString();
                buttons[i].Occupier = occupier;
            }
            OnPropertyChanged(nameof(RoundTracker));
            OnPropertyChanged(nameof(PlayerOnTurn));
            OnPropertyChanged(nameof(RedRemovedPieces));
            OnPropertyChanged(nameof(BlueRemovedPieces));
        }

        private void InitializeControls()
        {
            for (int i = 0; i < 24; i++)
            {
                buttons[i] = new TileViewModel(i, Player.Empty, OnButtonClicked, (_) => true); //TODO
            }

            OnPropertyChanged(nameof(RoundTracker));
            OnPropertyChanged(nameof(PlayerOnTurn));
            OnPropertyChanged(nameof(RedRemovedPieces));
            OnPropertyChanged(nameof(BlueRemovedPieces));
            OnPropertyChanged(nameof(Buttons));
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
            buttons[from].Occupier = "Empty";
            OnPropertyChanged(nameof(RedRemovedPieces));
            OnPropertyChanged(nameof(BlueRemovedPieces));
        }

        private void OnTileMoved(object? sender, MillsTileEventArgs e)
        {
            int to = e.Position;
            int from = e.SelectedPosition ?? throw new Exception();
            buttons[from].Occupier = "Empty";
            buttons[to].Occupier = PlayerOnTurn;
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
    }
    #endregion
}
