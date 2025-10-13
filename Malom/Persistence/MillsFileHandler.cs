using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malom.Model;

namespace Malom.Persistence
{
    public class MillsFileHandler : IFileHandler
    {
        public GameModel? OpenFile(string path)
        {
            return null;
        }

        public bool SaveFile(GameModel gameState, string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(gameState.Steps.ToString() + " " + gameState.PlayerOnTurn);
                    for (int i = 0; i < 24; i++)
                    {
                        string[] neighbours = gameState.TableData.GetTile(i).Neighbours
                            .Select(x => x == null ? "Empty" : x.Occupier.ToString()).ToArray();
                        sw.Write(gameState.TableData.GetTile(i).Occupier.ToString() +" ");
                        foreach (var str in neighbours) { sw.Write(str + " "); }
                        sw.Write("\n");
                    }
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
