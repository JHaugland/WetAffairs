using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWData.Ai;
using System.Diagnostics;

namespace NWManagerTest
{
    /// <summary>
    /// Summary description for UtilHelperTest
    /// </summary>
    [TestClass]
    public class TerrainReaderTest
    {
        public TerrainReaderTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestTerrainData()
        {
            TerrainReader.TestSomeCoordinates();

        }

        [TestMethod]
        public void CreateTraverableList()
        {
            TerrainReader.LoadMemoryMap();
            TerrainReader.CreateTraversableNodeMap();

        }

        [TestMethod]
        public void UpdateElevationWithTerrain()
        {
            TerrainReader.LoadMemoryMap();
            var landPos = new Position(60, 9, 100, 90);
            var seaPos = new Position(60, 3, 100, 90);
            var heightLandM = TerrainReader.GetHeightM(landPos.Coordinate);
            var heightSeaM = TerrainReader.GetHeightM(seaPos.Coordinate);
            Assert.IsTrue(heightLandM > 0, "Land coord height should be over 0");
            Assert.IsTrue(heightSeaM < 0, "Sea coord height should be under 0");

            Player player1 = new Player();
            player1.Name = "Player1";
            Player player2 = new Player();
            player2.Name = "Player2";

            Group group1 = new Group();
            Group group2 = new Group();

            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player1, "test game");
            GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
            GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
            game.AddPlayer(player2);

            var unitF22 = GameManager.Instance.GameData.CreateUnit(player1, group1, "f22", "", landPos, true);
            unitF22.UserDefinedElevation = GameConstants.HeightDepthPoints.Low;
            unitF22.MovementOrder = new MovementOrder(new Waypoint(new Position(new Coordinate(60, 0))));
            unitF22.MoveToNewCoordinate(10);

            Assert.IsTrue(unitF22.Position.HeightOverSeaLevelM > heightLandM, "F22 should be above land terrain at position.");
            unitF22.Position = seaPos.Clone();

            unitF22.Position.HeightOverSeaLevelM = 161;

            unitF22.TerrainHeight10SecForwardM = -500;
            unitF22.TerrainHeight30SecForwardM = -500;
            unitF22.TerrainHeightAtPosM = -500;

            unitF22.MoveToNewCoordinate(10);

            Assert.IsTrue(unitF22.DesiredHeightOverSeaLevelM == 100, "The unit's desired height should be 100.");



            TerrainReader.CloseMemoryMap();

        }
        [TestMethod]
        public void TerrainLineTest()
        {
            var startCoord = new Coordinate(60, 4);
            var endCoord = new Coordinate(62, 6);
            var noOfPoints = 10;
            double bearingDeg;
            double distanceM;
            var distM = MapHelper.CalculateDistanceApproxM(startCoord, endCoord);
            var coordList = TerrainReader.GetCoordinateLine(startCoord, endCoord, noOfPoints, out bearingDeg, out distanceM);
            //TerrainReader.CloseMemoryMap();
            TerrainReader.LoadMemoryMap();
            var testHeightM = TerrainReader.GetHeightM(new Coordinate(60, 5));
            Assert.IsTrue(testHeightM != 0, "Test height should not be 0. Memory map not loaded.");
            var sw = Stopwatch.StartNew();
            var heights = TerrainReader.GetHeightArrayM(coordList);
            sw.Stop();
            var time10coordsMs = sw.ElapsedMilliseconds;
            GameManager.Instance.Log.LogDebug("TerrainLineTest: Get 10 coords: " + time10coordsMs + " ms");
            sw.Start();
            endCoord = new Coordinate(55, 13);
            var summary10 = TerrainReader.GetTerrainHeightSummary(startCoord, endCoord, 10);
            sw.Stop();
            var time10coordsSummaryMs = sw.ElapsedMilliseconds;
            GameManager.Instance.Log.LogDebug("TerrainLineTest: Get 10 coords summary: " + time10coordsSummaryMs + " ms");
            sw.Start();
            var terrainSummary = TerrainReader.GetTerrainHeightSummary(startCoord, endCoord, 1000);
            sw.Stop();
            var time1000coordsSummaryMs = sw.ElapsedMilliseconds;
            GameManager.Instance.Log.LogDebug("TerrainLineTest: Get 1000 coords summary: " + time1000coordsSummaryMs + " ms");
            TerrainReader.CloseMemoryMap();
        }
        //[TestMethod]
        //public void CrateIt()
        //{
        //    TerrainReader.CreateMap();

        //}
    }
}
