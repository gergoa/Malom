using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Model
{
    public class MillsTileEventArgs : EventArgs
    {
        public int Position { get; set; }
        public int? SelectedPosition { get; set; }
        public string PlayerOnTurn { get; set; }  // "Red", "Blue"
        public int Round { get; set; }
        public MillsTileEventArgs(string playerOnTurn, int? selectedPosition, int position, int round)
        {
            PlayerOnTurn = playerOnTurn;
            SelectedPosition = selectedPosition;
            Position = position;
            Round = round;
        }
    }
}
