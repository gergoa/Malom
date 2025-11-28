namespace Malom.Persistence
{
    public enum Player { Red, Blue, Empty }

    // Tile neighbours are stored in order: Up, Left, Down, Right
    public class Tile
    {
        private Tile[] _neighbours = new Tile[4];
        private Player _occupier = Player.Empty;

        public Player Occupier
        {
            get { return _occupier; }
            set { _occupier = value; }
        }

        public Tile[] Neighbours
        {
            get
            {
                Tile[] copy = new Tile[4];
                Array.Copy(_neighbours, copy, 4);
                return copy;
            }
        }

        public Tile()
        {
        }
        public void SetNeighbour(int index, Tile neighbour)
        {
            _neighbours[index] = neighbour;
        }

    }
}
