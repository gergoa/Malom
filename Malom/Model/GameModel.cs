using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Malom.Persistence;

namespace Malom.Model
{
    public class GameModel
    {

        #region Fields

        private TableData _tableData;
        private bool _removing;
        private int? _selectedPiece;
        private Player _playerOnTurn;
        private int _steps;
        private (int, int) _removedPieces;
        private IFileHandler _fileHandler;



        private enum GamePhase { Placing, Moving, Removing }
        private GamePhase Phase =>
            _removing ? GamePhase.Removing :
            _steps < 18 ? GamePhase.Placing :
            GamePhase.Moving;

        #endregion

        #region Properties
        public int Steps { get { return _steps; } }
        public int RemovedRedPieces { get { return _removedPieces.Item1; } }
        public int RemovedBluePieces { get { return _removedPieces.Item2; } }
        public string PlayerOnTurn { get { return _playerOnTurn == Player.Red ? "Red" : "Blue"; } }

        public TableData TableData { get {
                TableData clone = new();
                for (int i = 0; i < 24; i++)
                {
                    clone.SetTile(i, _tableData.GetTile(i).Occupier);
                }
                return clone;
            } }

        #endregion

        #region Events
        public event EventHandler<MillsTileEventArgs>? TilePlaced;
        public event EventHandler<MillsTileEventArgs>? TileMoved;
        public event EventHandler<MillsTileEventArgs>? TileDeleted;

        public event EventHandler<MillsEventArgs>? RoundProgressed;
        public event EventHandler<MillsEventArgs>? GameOver;

        public event EventHandler<MillsEventArgs>? GameLoaded;

        #endregion


        #region Constructor

        public GameModel(string startingPlayer, IFileHandler fileHandler)
        {
            //TODO - Implement persistence in constructor
            _tableData = new TableData();
            _removing = false;
            _fileHandler = fileHandler;
            _playerOnTurn = startingPlayer == "Red" ? Player.Red : Player.Blue;
            _steps = 0;
            _removedPieces = (0, 0);
            _selectedPiece = null;
        }

        #endregion

        private bool IsGameOver()
        { 
            Player opponent = _playerOnTurn == Player.Red ? Player.Blue : Player.Red;

            int opponentPieces = 0;
            for (int i = 0; i < 24; i++)
            {
                if (_tableData.GetTile(i).Occupier == opponent) opponentPieces++;
            }

            return opponentPieces < 3 && _steps > 18;
        }

        #region Methods
        public void NewGame()
        {
            GameOver?.Invoke(this, new MillsEventArgs("New Game"));
        }

        public bool Update(int to)
        {
            bool success = false;
            //Handle placing-moving-deleting phases
            switch (Phase)
            {
                case GamePhase.Placing:
                    success = _tableData.SetTile(to, _playerOnTurn);
                    if (success) TilePlaced?.Invoke(this, new MillsTileEventArgs(null, to));
                    break;

                case GamePhase.Moving:
                    if (_selectedPiece != null)
                    {
                        success = _tableData.Move((int)_selectedPiece, to, _playerOnTurn);
                        if (success) TileMoved?.Invoke(this, new MillsTileEventArgs(_selectedPiece, to));
                        _selectedPiece = null;
                    }
                    else
                    {
                        success = false;
                        _selectedPiece = _tableData.GetTile(to).Occupier == _playerOnTurn ? to : null;
                    }
                    break;

                case GamePhase.Removing:
                    success = _tableData.IsRemovable(to, _playerOnTurn)
                        ? _tableData.ClearTile(to, _playerOnTurn)
                        : false;

                    if (success)
                    {
                        if (_playerOnTurn == Player.Blue)
                        {
                            _removedPieces.Item1++;
                        }
                        else if (_playerOnTurn == Player.Red)
                        {
                            _removedPieces.Item2++;
                        }
                        TileDeleted?.Invoke(this, new MillsTileEventArgs(null, to));
                    }
                    if (IsGameOver()) GameOver?.Invoke(this, new MillsEventArgs(Phase.ToString()));

                    break;
            }
            if (!success) return false;

            //end of round conditions, change player
            _removing = _removing == false ? _tableData.HasMill(to) : false;

            if (!_removing)
            {
                _playerOnTurn = _playerOnTurn == Player.Red ? Player.Blue : Player.Red;
                _steps++;
            }
            RoundProgressed?.Invoke(this, new MillsEventArgs(Phase.ToString()));

            return true;
        }

        public bool SaveGame(string path)
        {
            return _fileHandler.SaveFile(new TableState(_tableData, _steps, _playerOnTurn, _removedPieces), path);
        }

        public bool LoadGame(string path)
        {
            TableState? newState = _fileHandler.OpenFile(path);
            if (newState?.TableData == null) return false;
            TableData newData = newState.Value.TableData;

            _tableData = newData;
            _removing = false;
            _selectedPiece = null;
            _steps = newState.Value.Steps;
            _removedPieces = newState.Value.RemovedPieces;
            _playerOnTurn = newState.Value.PlayerOnTurn;
            GameLoaded?.Invoke(this, new(Phase.ToString()));
            return true;
        }
        #endregion
    }
}
