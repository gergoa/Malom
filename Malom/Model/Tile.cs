using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Model
{
    enum Player { Red, Blue, Empty }

    // Tile neighbours are stored in order: Up, Left, Down, Right
    enum Direction { Up, Left, Down, Right }
    internal class Tile
    {
        private Tile?[] neighbours = new Tile?[4];
        private Player occupier = Player.Empty;

        public Tile?[] Neighbours
        {
            get { return neighbours; }
            set { neighbours = value; }
        }

        public Player Occupier
        {
            get { return occupier; }
            set { occupier = value; }
        }

    }
}
