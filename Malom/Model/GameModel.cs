using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Model
{
    public class GameModel
    {

        #region Fields

        private TableData _tableData;
        private Player _playerOnTurn;
        private int _steps;
        private bool _removing;
        private int? _selectedPiece;

        private enum GamePhase { Placing, Moving, Removing }
        private GamePhase Phase =>
            _removing ? GamePhase.Removing :
            _steps < 18 ? GamePhase.Placing :
            GamePhase.Moving;

        #endregion

        #region Properties

        #endregion

        #region Events
        public event EventHandler<MillsTileEventArgs>? TilePlaced;
        public event EventHandler<MillsTileEventArgs>? TileMoved;
        public event EventHandler<MillsTileEventArgs>? TileDeleted;

        public event EventHandler<MillsEventArgs>? RoundProgressed;
        public event EventHandler<MillsTileEventArgs>? GameOver;

        #endregion


        #region Constructor

        public GameModel()
        {
            //TODO - Implement persistence in constructor
            _tableData = new TableData();
            _playerOnTurn = Player.Red;
            _steps = 0;
            _removing = false;
        }

        #endregion

        #region Methods
        public void NewGame()
        {
            _tableData = new TableData();
            _playerOnTurn = Player.Red;
            _steps = 0;
        }

        public bool Update(int to)
        {
            bool success = false;
            //Handle placing-moving-deleting phases
            switch (Phase)
            {
                case GamePhase.Placing:
                    success = _tableData.SetTile(to, _playerOnTurn);
                    if (success) TilePlaced?.Invoke(this, new MillsTileEventArgs(_playerOnTurn.ToString(), null, to, _steps));
                    break;

                case GamePhase.Moving:
                    if (_selectedPiece != null)
                    {
                        success = _tableData.Move((int)_selectedPiece, to, _playerOnTurn);
                        if (success) TileMoved?.Invoke(this, new MillsTileEventArgs(_playerOnTurn.ToString(), _selectedPiece, to, _steps));
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
                    if (success) TileDeleted?.Invoke(this, new MillsTileEventArgs(_playerOnTurn.ToString(), null, to, _steps));
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
            RoundProgressed?.Invoke(this, new MillsEventArgs(Phase.ToString(), _playerOnTurn.ToString(), _steps));

            return true;
        }
        #endregion
    }
}
