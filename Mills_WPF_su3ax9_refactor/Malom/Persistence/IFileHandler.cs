namespace Malom.Persistence
{
    public struct TableState
    {
        private readonly TableData _tableData;
        public readonly int Steps;
        public readonly Player PlayerOnTurn;
        public readonly (int, int) RemovedPieces;

        public TableData TableData
        {
            get
            {
                TableData clone = new();
                for (int i = 0; i < 24; i++)
                {
                    clone.SetTile(i, _tableData.GetTile(i).Occupier);
                }
                return clone;
            }
        }
        public TableState(TableData tableData, int steps, Player playerOnTurn, (int, int) removedPieces)
        {
            _tableData = tableData;
            Steps = steps;
            PlayerOnTurn = playerOnTurn;
            RemovedPieces = removedPieces;
        }
    }
    public interface IFileHandler
    {
        TableState? OpenFile(string path);
        bool SaveFile(TableState gameState, string path);
    }
}
