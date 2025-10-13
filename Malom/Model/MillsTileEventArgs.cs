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
        public MillsTileEventArgs(int? selectedPosition, int position)
        {
            SelectedPosition = selectedPosition;
            Position = position;
        }
    }
}
