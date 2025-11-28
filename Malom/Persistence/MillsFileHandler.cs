namespace Malom.Persistence
{
    public class MillsFileHandler : IFileHandler
    {

        public TableState? OpenFile(string path)
        {
            TableData? newData;
            TableState? tableState;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string[]? args = sr.ReadLine()?.Split(" ") ?? throw new Exception("Invalid file content!");
                    string playerOnTurn = args[1];
                    int steps = int.Parse(args[0]);
                    int removedRedPieces = int.Parse(args[2]);
                    int removedBluePieces = int.Parse(args[3]);
                    newData = new TableData();
                    for (int i = 0; i < 24; i++)
                    {
                        string? occupier = sr.ReadLine()?.Split(" ")[0];
                        newData.SetTile(i, occupier == "Red" ? Player.Red :
                                           occupier == "Blue" ? Player.Blue :
                                           occupier == "Empty" ? Player.Empty :
                                           throw new Exception("Invalid file content!"));
                    }
                    tableState = new TableState(newData, steps, playerOnTurn == "Red" ? Player.Red : Player.Blue, (removedRedPieces, removedBluePieces));
                }
            }
            catch (Exception e)
            {
                throw new IOException("Could not open file!", e);
            }
            return tableState;
        }

        public bool SaveFile(TableState gameState, string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(gameState.Steps.ToString() + " " + gameState.PlayerOnTurn + " " + gameState.RemovedPieces.Item1.ToString() + " " + gameState.RemovedPieces.Item2.ToString());
                    for (int i = 0; i < 24; i++)
                    {
                        sw.WriteLine(gameState.TableData.GetTile(i).Occupier.ToString());
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
