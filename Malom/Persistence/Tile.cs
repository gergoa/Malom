using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Persistence
{
    public enum Player { Red, Blue, Empty }

    // Tile neighbours are stored in order: Up, Left, Down, Right
    public enum Direction { Up, Left, Down, Right }
    public class Tile
    {
        private Tile[] neighbours = new Tile[4];
        private Player occupier = Player.Empty;

        public Player Occupier
        {
            get { return occupier; }
            set { occupier = value; }
        }

        public Tile[] Neighbours
        {
            get
            {
                Tile[] copy = new Tile[4];
                Array.Copy(neighbours, copy, 4);
                return copy;
            }
        }

        public void SetNeighbour(int index, Tile neighbour)
        {
            neighbours[index] = neighbour;
        }

    }
}
