using Malom.Model;
using Malom.Persistence;
using Moq;

namespace MillsTest
{
    [TestClass]
    public sealed class MillsModelTests
    {
        private Mock<IFileHandler> _mockFileHandler;
        private GameModel _model;

        public MillsModelTests()
        {
            _mockFileHandler = new Mock<IFileHandler>();
            _model = new GameModel("Red", _mockFileHandler.Object);
        }
        [TestMethod]
        public void NewGameTest()
        {
            _model.NewGame();
            Assert.AreEqual(0, _model.Steps);
            Assert.AreEqual(0, _model.RemovedRedPieces);
            Assert.AreEqual(0, _model.RemovedBluePieces);
            Assert.AreEqual("Red", _model.PlayerOnTurn);
            for (int i = 0; i < 24; i++)
            {
                Assert.AreEqual(Player.Empty, _model.TableData.GetTile(i).Occupier);
            }
        }

        [TestMethod]
        public void PlayerStepTest()
        {
            bool roundProgressedEventFired = false;
            string nextAction = "";
            _model.RoundProgressed += (sender, args) => { roundProgressedEventFired = true; nextAction = args.NextAction; };

            _model.Update(0); // Red places
            Assert.AreEqual(Player.Red, _model.TableData.GetTile(0).Occupier);
            Assert.AreEqual(1, _model.Steps);
            Assert.AreEqual("Blue", _model.PlayerOnTurn);
            Assert.IsTrue(roundProgressedEventFired);
            roundProgressedEventFired = false;

            _model.Update(1); // Blue places
            Assert.AreEqual(Player.Blue, _model.TableData.GetTile(1).Occupier);
            Assert.AreEqual(2, _model.Steps);
            Assert.AreEqual("Red", _model.PlayerOnTurn);
            Assert.IsTrue(roundProgressedEventFired);

            for (int i = 0; i < 16; i++)
            {
                _model.Update(2 + i); // Fill up the board
            }
            Assert.AreEqual(18, _model.Steps);
            Assert.AreEqual("Moving", nextAction);
        }

        [TestMethod]
        public void GameMillTest()
        {
            string nextAction = "";
            bool pieceDeletedEventFired = false;
            _model.RoundProgressed += (s, e) => { nextAction = e.NextAction; };
            _model.TileDeleted += (s, e) => { pieceDeletedEventFired = true; };

            _model.NewGame();
            _model.Update(0); // Red
            _model.Update(3); // Blue
            _model.Update(1); // Red
            _model.Update(4); // Blue
            _model.Update(2); // Red

            Assert.AreEqual("Removing", nextAction);
            Assert.AreEqual("Red", _model.PlayerOnTurn);

            _model.Update(3);

            Assert.AreEqual("Empty", _model.TableData.GetTile(3).Occupier.ToString());
            Assert.AreEqual(1, _model.RemovedBluePieces);
            Assert.IsTrue(pieceDeletedEventFired);
        }

        [TestMethod]
        public void GameLoadTest()
        {
            _model.NewGame();
            _model.Update(0);
            _model.Update(1);
            TableState savedState = new(_model.TableData, 6, Player.Red, (1, 2));
            _mockFileHandler.Setup(fh => fh.OpenFile("test.nmm")).Returns(savedState);

            bool gameLoadedEventFired = false;
            _model.GameLoaded += (s, e) => { gameLoadedEventFired = true; };
            _model.NewGame();
            _model.LoadGame("test.nmm");

            Assert.IsTrue(gameLoadedEventFired);
            Assert.AreEqual(6, _model.Steps);
            Assert.AreEqual("Red", _model.PlayerOnTurn);
            Assert.AreEqual(1, _model.RemovedRedPieces);
            Assert.AreEqual(2, _model.RemovedBluePieces);
            Assert.AreEqual(Player.Red, _model.TableData.GetTile(0).Occupier);
            Assert.AreEqual(Player.Blue, _model.TableData.GetTile(1).Occupier);
        }

        [TestMethod]
        public void GameSaveTest()
        { 
            _mockFileHandler.Setup(fh => fh.SaveFile(It.IsAny<TableState>(), "save.nmm")).Returns(true);

            _model.NewGame();
            _model.Update(0);
            _model.Update(1);

            bool saveResult = _model.SaveGame("save.nmm");
            Assert.IsTrue(saveResult);
            _mockFileHandler.Verify(fh => fh.SaveFile(It.Is<TableState>(ts =>
                ts.Steps == 2 &&
                ts.PlayerOnTurn == Player.Red &&
                ts.TableData.GetTile(0).Occupier == Player.Red &&
                ts.TableData.GetTile(1).Occupier == Player.Blue
            ), "save.nmm"), Times.Once);
        }
    }
}
