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

        public String PlayerOnTurn => _playerOnTurn == Player.Red ? "Red" : "Blue";

        #endregion

        #region Events
        public event EventHandler<MillsEventArgs>? TileClicked;
        public event EventHandler<MillsEventArgs>? GameOver;

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
                    break;

                case GamePhase.Moving:
                    if (_selectedPiece != null)
                    {
                        success = _tableData.Move((int)_selectedPiece, to, _playerOnTurn);
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
                    break;
            }
            //Inform model that tile has been clicked
            TileClicked?.Invoke(this, new MillsEventArgs(_playerOnTurn.ToString(), _selectedPiece, _steps, Phase.ToString() ));
            if (!success) return false;

            //end of round conditions, change player
            _removing = _removing == false ? _tableData.HasMill(to) : false;

            if (!_removing)
            {
                _playerOnTurn = _playerOnTurn == Player.Red ? Player.Blue : Player.Red;
                _steps++;
            }

            return true;
        }
        #endregion
    }
}
