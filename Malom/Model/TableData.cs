using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Model
{ 

    internal class TableData
    {
        private readonly Tile[] _tiles;

        #region Constructor
        public TableData()
        {   
            _tiles = new Tile[24];
            for (int i = 0; i < 24; i++) { _tiles[i] = new Tile(); }

            Connect(_tiles[0], _tiles[1], Direction.Right); Connect(_tiles[0], _tiles[9], Direction.Down);
            Connect(_tiles[1], _tiles[2], Direction.Right); Connect(_tiles[1], _tiles[4], Direction.Down);
            Connect(_tiles[2], _tiles[14], Direction.Down);

            Connect(_tiles[3], _tiles[4], Direction.Right); Connect(_tiles[3], _tiles[10], Direction.Down);
            Connect(_tiles[4], _tiles[5], Direction.Right); Connect(_tiles[4], _tiles[7], Direction.Down);
            Connect(_tiles[5], _tiles[13], Direction.Down);

            Connect(_tiles[6], _tiles[7], Direction.Right); Connect(_tiles[6], _tiles[11], Direction.Down);
            Connect(_tiles[7], _tiles[8], Direction.Right);
            Connect(_tiles[8], _tiles[12], Direction.Down);

            Connect(_tiles[9], _tiles[10], Direction.Right); Connect(_tiles[9], _tiles[21], Direction.Down);
            Connect(_tiles[10], _tiles[11], Direction.Right); Connect(_tiles[10], _tiles[18], Direction.Down);

            Connect(_tiles[12], _tiles[13], Direction.Right); Connect(_tiles[12], _tiles[17], Direction.Down);
            Connect(_tiles[13], _tiles[20], Direction.Down);

            Connect(_tiles[14], _tiles[23], Direction.Down); Connect(_tiles[14], _tiles[15], Direction.Right);

            Connect(_tiles[15], _tiles[16], Direction.Right); 
            Connect(_tiles[16], _tiles[17], Direction.Right); Connect(_tiles[16], _tiles[19], Direction.Down);

            Connect(_tiles[18], _tiles[19], Direction.Right);
            Connect(_tiles[19], _tiles[20], Direction.Right); Connect(_tiles[19], _tiles[22], Direction.Down);
            Connect(_tiles[21], _tiles[22], Direction.Right);
            Connect(_tiles[22], _tiles[23], Direction.Right);

        }

        #endregion

        #region Private Methods
        private static void Connect(Tile from, Tile to, Direction dirFromA)
        {
            from.Neighbours[(int)dirFromA] = to;
            to.Neighbours[((int)dirFromA + 2) % 4] = from;
        }

        #endregion

        #region Public Methods

        public Tile GetTile(int i)
        {
            if (i < 0 || i >= 24) throw new ArgumentException();
            return _tiles[i];
        }

        public bool SetTile(int i, Player p)
        {
            if (_tiles[i].Occupier == Player.Empty)
            {
                _tiles[i].Occupier = p;
                return true;
            }
            return false;
        }

        public bool ClearTile(int i, Player p)
        {
            if (_tiles[i].Occupier == Player.Empty || _tiles[i].Occupier == p) return false;

            _tiles[i].Occupier = Player.Empty;
            return true;
        }

        private bool ClearTile(int i)
        {
            _tiles[i].Occupier = Player.Empty;

            return true;
        }

        public bool Move(int from, int to, Player p)
        {
            if (from < 0 || from >= 24 || to < 0 || to >= 24
                || !(_tiles[from].Neighbours.Contains(_tiles[to]))
                || _tiles[from].Occupier != p
                || _tiles[to].Occupier != Player.Empty) return false;

            SetTile(to, p);
            ClearTile(from);
            return true;
        }

        //TODO
        public bool HasMill(int i)
        {
            Tile t = _tiles[i];

            //Check cases where tile is on side of mill
            for (int j = 0; j < 4; j++)
            {
                if (t.Neighbours[j]?.Occupier == t.Occupier &&
                    t.Neighbours[j]?.Neighbours[j]?.Occupier == t.Occupier)
                {
                    return true;
                }
            }

            //Check cases of tile being center of mill
            if (t.Neighbours[(int)Direction.Up]?.Occupier == t.Occupier &&
                t.Neighbours[(int)Direction.Down]?.Occupier == t.Occupier)
            {
                return true;
            }
            if (t.Neighbours[(int)Direction.Left]?.Occupier == t.Occupier &&
                t.Neighbours[(int)Direction.Right]?.Occupier == t.Occupier)
            {
                return true;
            }
            return false;
        }

        //TODO - iterate through all mills, if there are tiles not in mill yet, tile in mill can't be removed.
        public bool IsRemovable(int i, Player p)
        {
            Tile t = _tiles[i];
            if (t.Occupier == Player.Empty || t.Occupier == p) return false;
            if (!HasMill(i)) return true;

            for (int j = 0; j < 24; j++)
            {
                if (_tiles[j].Occupier != Player.Empty && !HasMill(j)) return false;
            }

            return true;
        }
        #endregion
    }
}
