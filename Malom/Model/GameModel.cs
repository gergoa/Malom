using System;
using System.Collections.Generic;
using System.Linq;
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
        private IFileHandler _fileHandler;


        private enum GamePhase { Placing, Moving, Removing }
        private GamePhase Phase =>
            _removing ? GamePhase.Removing :
            _tableData.Steps < 18 ? GamePhase.Placing :
            GamePhase.Moving;

        #endregion

        #region Properties

        public TableData TableData { 
            get 
            {
                TableData copy = new TableData(_tableData.PlayerOnTurn == Player.Red ? "Red" : "Blue", 
                    _tableData.Steps, (_tableData.RemovedRedPieces, _tableData.RemovedBluePieces));
                for (int i = 0; i < 24; i++)
                {
                    copy.SetTile(i, _tableData.GetTile(i).Occupier);
                }
                return copy;
            }
        }
        public string PlayerOnTurn { get { return _tableData.PlayerOnTurn == Player.Red ? "Red" : "Blue"; } }

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

        public GameModel(string startingPlayer)
        {
            //TODO - Implement persistence in constructor
            _tableData = new TableData(startingPlayer);
            _removing = false;
            _fileHandler = new MillsFileHandler();
        }

        public GameModel(TableData tableData)
        {
            _tableData = tableData;
            _fileHandler = new MillsFileHandler();
            _removing = false;
        }


        #endregion

        private bool IsGameOver()
        { 
            Player opponent = _tableData.PlayerOnTurn == Player.Red ? Player.Blue : Player.Red;

            int opponentPieces = 0;
            for (int i = 0; i < 24; i++)
            {
                if (_tableData.GetTile(i).Occupier == opponent) opponentPieces++;
            }

            return opponentPieces < 3 && _tableData.Steps > 18;
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
                    success = _tableData.SetTile(to, _tableData.PlayerOnTurn);
                    if (success) TilePlaced?.Invoke(this, new MillsTileEventArgs(null, to));
                    break;

                case GamePhase.Moving:
                    if (_selectedPiece != null)
                    {
                        success = _tableData.Move((int)_selectedPiece, to, _tableData.PlayerOnTurn);
                        if (success) TileMoved?.Invoke(this, new MillsTileEventArgs(_selectedPiece, to));
                        _selectedPiece = null;
                    }
                    else
                    {
                        success = false;
                        _selectedPiece = _tableData.GetTile(to).Occupier == _tableData.PlayerOnTurn ? to : null;
                    }
                    break;

                case GamePhase.Removing:
                    success = _tableData.IsRemovable(to, _tableData.PlayerOnTurn)
                        ? _tableData.ClearTile(to, _tableData.PlayerOnTurn)
                        : false;

                    if (success) TileDeleted?.Invoke(this, new MillsTileEventArgs(null, to));
                    if (IsGameOver()) GameOver?.Invoke(this, new MillsEventArgs(Phase.ToString()));

                    break;
            }
            if (!success) return false;

            //end of round conditions, change player
            _removing = _removing == false ? _tableData.HasMill(to) : false;

            if (!_removing)
            {
                _tableData.PlayerOnTurn = _tableData.PlayerOnTurn == Player.Red ? Player.Blue : Player.Red;
                _tableData.Steps++;
            }
            RoundProgressed?.Invoke(this, new MillsEventArgs(Phase.ToString()));

            return true;
        }

        public bool SaveGame(string path)
        {
            return _fileHandler.SaveFile(_tableData,path);
        }

        public bool LoadGame(string path)
        {
            TableData? newData = _fileHandler.OpenFile(path);
            if (newData == null) return false;

            _tableData = newData;
            _removing = false;
            _selectedPiece = null;
            GameLoaded?.Invoke(this, new(Phase.ToString()));
            return true;
        }
        #endregion
    }
}
