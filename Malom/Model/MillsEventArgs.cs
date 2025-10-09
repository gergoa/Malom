using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Model
{
    public class MillsEventArgs : EventArgs
    {
        private String _playerOnTurn;
        private int? _selectedTile;
        private int _steps;
        private int
        private String _action;

        #region Properties
        
        public String PlayerOnTurn {  get { return _playerOnTurn; }}
        public int? SelectedTile { get  { return _selectedTile; }}
        #endregion
        public int Steps { get { return _steps; }}
        public String Action { get { return _action; }}
        public MillsEventArgs(String playerOnTurn, int? selectedTile, int steps, string action)
        {
            _playerOnTurn = playerOnTurn;
            _selectedTile = selectedTile;
            _steps = steps;
            _action = action;
        }
    }
}
