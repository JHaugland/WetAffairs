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
using TTG.NavalWar.NWComms;

namespace NWManagerTest
{
    /// <summary>
    /// Summary description for DetectionTest
    /// </summary>
    [TestClass]
    public class DetectionTest
    {
        public DetectionTest()
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
        public void SonarDetection()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);

            GameManager.Instance.Game.SetAllPlayersEnemies();


            Position pos = new Position(60, 3, -100, 5);
            BaseUnit unitUla = GameManager.Instance.GameData.CreateUnit(player, group, "ula", "KNM Ula", pos, true, true);
            unitUla.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Sonar, false);

            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unitUla.Position.Coordinate,
                45, 2000);
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = -100;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemyUla = GameManager.Instance.GameData.CreateUnit(playerEnemy, grpEnemy, "ula", "KNM Uredd", NextDoorPos, true, true);
            unitEnemyUla.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Sonar, false);
            game.IsNetworkEnabled = false;
            //GameManager.Instance.Game.RunGameInSec = 1;
            //GameManager.Instance.Game.StartGamePlay();
            unitUla.SensorSweep();
            Assert.IsTrue(player.DetectedUnits.Count == 1, "One and only one unit should have been detected.");
            var det = player.DetectedUnits.FirstOrDefault<DetectedUnit>();
            Assert.IsNotNull(det, "Detected sub should not be null.");
            Assert.IsTrue(!det.IsFixed, "Detected sub position should not be fixed.");
            var distErrorM = MapHelper.CalculateDistance3DM(det.Position, unitEnemyUla.Position);

            unitUla.SensorSweep(); //again

            var distError2M = MapHelper.CalculateDistance3DM(det.Position, unitEnemyUla.Position);
            var errorDiff = Math.Abs(distError2M - distErrorM);
            Assert.IsTrue(errorDiff < 0.01, "Error distance should be the same in both cases.");
            GameManager.Instance.Log.LogDebug("DetectionTest->SonarDetection(): " + player.DetectedUnits[0].ToLongString());
            GameManager.Instance.TerminateGame();
        }
        
        [TestMethod] 
        public void VisualDetection()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;
            
            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);

            GameManager.Instance.Game.SetAllPlayersEnemies();
            

            Position pos = new Position(60, 3, 0, 120);
            BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "DDG Valiant", pos, true, true);
            unit.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);

            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unit.Position.Coordinate, 
                45, 12000);
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true, true);
            unitEnemy.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, false);
            var visual = (from s in unit.Sensors where s.SensorClass.SensorType == GameConstants.SensorType.Visual select s).FirstOrDefault<BaseSensor>();
            Assert.IsNotNull(visual, "Visual detector should not be null");
            var distM = MapHelper.CalculateDistance3DM(unit.Position, unitEnemy.Position);
            var canSeeIt = visual.AttemptDetectUnit(unitEnemy, distM);

            game.IsNetworkEnabled = false;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();
            
            Assert.IsTrue(playerEnemy.DetectedUnits.Count == 1, "One and only one unit should have been detected.");
            foreach (var weapon in unit.Weapons)
            {
                bool isInSector = weapon.IsCoordinateInSectorRange(unitEnemy.Position.Coordinate);
                GameManager.Instance.Log.LogDebug(
                    string.Format("Enemy within weapon {0} sector range: {1}", weapon.Name, isInSector));
            }
            GameManager.Instance.Log.LogDebug("DetectionTest->VisualDetection(): " + playerEnemy.DetectedUnits[0].ToLongString());
            GameManager.Instance.TerminateGame();
        }

        [TestMethod]
        public void RadarDetectionStrength()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);

            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos = new Position(60, 3, 1000, 120);
            BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, null, "e3sentry", "Awacs", pos, true, true);
            unitMain.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);
            Position NextDoorPos1 = new Position(MapHelper.CalculateNewPosition2(unitMain.Position.Coordinate,
                45, 100000));
            Position NextDoorPos2 = new Position(MapHelper.CalculateNewPosition2(unitMain.Position.Coordinate,
                60, 100000));
            Position NextDoorPos3 = new Position(MapHelper.CalculateNewPosition2(unitMain.Position.Coordinate,
                90, 100000));
            Position NextDoorPos4 = new Position(MapHelper.CalculateNewPosition2(unitMain.Position.Coordinate,
                120, 100000));

            NextDoorPos1.HeightOverSeaLevelM = 1000;
            NextDoorPos1.SetNewBearing(130);
            NextDoorPos2.HeightOverSeaLevelM = 1000;
            NextDoorPos2.SetNewBearing(130);
            NextDoorPos3.HeightOverSeaLevelM = 1000;
            NextDoorPos3.SetNewBearing(130);
            NextDoorPos4.HeightOverSeaLevelM = 1000;
            NextDoorPos4.SetNewBearing(130);

            BaseUnit unitEnemy1 = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "f22", "", NextDoorPos1, true);
            Assert.IsNotNull(unitEnemy1, "Enemy 1 is not null");
            unitEnemy1.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, false);
            
            BaseUnit unitEnemy2 = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "mig29k", "", NextDoorPos2, true);
            Assert.IsNotNull(unitEnemy2, "Enemy 2 is not null");
            unitEnemy2.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, false);

            BaseUnit unitEnemy3 = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "boeing767", "", NextDoorPos3, true);
            Assert.IsNotNull(unitEnemy3, "Enemy 3 is not null");
            unitEnemy3.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, false);
            
            BaseUnit unitEnemy4 = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "ka27", "", NextDoorPos4, true);
            Assert.IsNotNull(unitEnemy4, "Enemy 4 is not null");
            unitEnemy4.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, false);

            game.IsNetworkEnabled = false;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();
            if (player.DetectedUnits.Count < 3)
            {
                //Stop here to examine the bug!
            }
            Assert.IsTrue(player.DetectedUnits.Count == 3, "Three units should have been detected.");

            GameManager.Instance.Log.LogDebug("DetectionTest->RadarDetectionStrength(): " + player.DetectedUnits[0].ToLongString());
            GameManager.Instance.TerminateGame();
        }

        [TestMethod]
        public void DetectionOverTerrainTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);

            GameManager.Instance.Game.SetAllPlayersEnemies();
            TerrainReader.LoadMemoryMap();
            Position posLand1 = new Position(61.16825, 5.21027, 20, 120);
            var posLand1heightM = TerrainReader.GetHeightM(posLand1.Coordinate);
            Assert.IsTrue(posLand1heightM > 0, "Terrain at pos1land should be land.");
            
            var unitF22 = GameManager.Instance.GameData.CreateUnit(player, group, "f22", "f22", posLand1, true);
            unitF22.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);
            var unitEnemyBurke = GameManager.Instance.GameData.CreateUnit(playerEnemy, group, "arleighburke", "Enemy Burke", posLand1, true);

            unitF22.SensorSweep();

            var detBurke = player.DetectedUnits.FirstOrDefault<DetectedUnit>();
            Assert.IsTrue(detBurke != null, "Enemy burke should have been detected.");

            //61.16825 , 5.21027 land 
            //61.18935 , 5.21027 vann
            //61.19935, , 5.21027 land igjen?
            TerrainReader.CloseMemoryMap();

        }


        [TestMethod]
        public void TestDetectionDistance()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);

            GameManager.Instance.Game.SetAllPlayersEnemies();


            Position pos = new Position(60, 3, 22000, 120);
            BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, group, "e3sentry", "AWACS", pos, true);
            unit.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);

            BaseUnit unitJammer = GameManager.Instance.GameData.CreateUnit(player, group, "p8poseidon", "Jammer", pos, true);
            unitJammer.SetWeaponLoad("Electronic warfare");
            unitJammer.Position = unit.Position.Offset(90, 1000).Clone();
            unitJammer.Position.BearingDeg = 90;
            
            var weaponJammer = unitJammer.GetSpecialWeapon(GameConstants.SpecialOrders.JammerRadarDegradation);
            
            Assert.IsNotNull(weaponJammer, "Jammer weapon should not be null");
            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unit.Position.Coordinate,
                45, 100000);
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            var jammingUnitOrder = OrderFactory.CreateRadarDegradationJammingOrder(unitJammer.Id, NextDoorPos.GetPositionInfo());
            var jammingBaseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(jammingUnitOrder);
            Assert.IsTrue(jammingBaseOrder.SpecialOrders == GameConstants.SpecialOrders.JammerRadarDegradation, "Order should be jamming");
            
            Group grpEnemy = new Group();
            BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true);
            unitEnemy.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, false);
            foreach (var wpn in unitEnemy.Weapons)
            {
                wpn.AmmunitionRemaining = 0; //so enemy can't launch missiles
            }

            game.IsNetworkEnabled = false;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();
            unitJammer.Orders.Enqueue(jammingBaseOrder);
            unitJammer.ExecuteOrders();
            var jammingEffect = GameManager.Instance.Game.GetJammingDegradationPercent(
                unitEnemy.Position.Coordinate, playerEnemy);
            Assert.IsTrue(jammingEffect > 0.0, "Jamming should be in effect!");

            Assert.IsTrue(player.DetectedUnits.Count == 1, "One and only one unit should have been detected.");
            player.AIHandler.SetPriorityScoresOnDetectedUnits();

            double distanceToRadarHorizon0m = MapHelper.CalculateMaxRadarLineOfSightM(0, 20);
            double sizeArcSec0 = unitEnemy.CalculateApparentSizeArcSec(
                GameConstants.DirectionCardinalPoints.E, distanceToRadarHorizon0m);
            double distanceToRadarHorizon1000m = MapHelper.CalculateMaxRadarLineOfSightM(1000, 20);
            double sizeArcSec1000 = unitEnemy.CalculateApparentSizeArcSec(
                GameConstants.DirectionCardinalPoints.E, distanceToRadarHorizon1000m);
            double distanceToRadarHorizon5000m = MapHelper.CalculateMaxRadarLineOfSightM(5000, 20);
            double sizeArcSec5000 = unitEnemy.CalculateApparentSizeArcSec(
                GameConstants.DirectionCardinalPoints.E, distanceToRadarHorizon5000m);
            double distanceToRadarHorizon20000m = MapHelper.CalculateMaxRadarLineOfSightM(20000, 20);
            double sizeArcSec20000 = unitEnemy.CalculateApparentSizeArcSec(
                GameConstants.DirectionCardinalPoints.E, distanceToRadarHorizon20000m);
            var radar = unit.Sensors.First<BaseSensor>(s => s.SensorClass.IsPassiveActiveSensor);
            bool detectResult = radar.AttemptDetectUnit(unitEnemy, 600000);


            var log = GameManager.Instance.Log;
            log.LogDebug("*** TestDetectionDistance:\n");
            log.LogDebug(string.Format("Height: {0}m \t RadarHor: {1:F}m {2:F}nm \t Size: {3:F}ArcSec",
                0,distanceToRadarHorizon0m,distanceToRadarHorizon0m.ToNmilesFromMeters(),sizeArcSec0));
            log.LogDebug(string.Format("Height: {0}m \t RadarHor: {1:F}m {2:F}nm \t Size: {3:F}ArcSec",
                1000, distanceToRadarHorizon1000m, distanceToRadarHorizon1000m.ToNmilesFromMeters(), sizeArcSec1000));
            log.LogDebug(string.Format("Height: {0}m \t RadarHor: {1:F}m {2:F}nm \t Size: {3:F}ArcSec",
                5000, distanceToRadarHorizon5000m, distanceToRadarHorizon5000m.ToNmilesFromMeters(), sizeArcSec5000));
            log.LogDebug(string.Format("Height: {0}m \t RadarHor: {1:F}m {2:F}nm \t Size: {3:F}ArcSec",
                20000, distanceToRadarHorizon20000m, distanceToRadarHorizon20000m.ToNmilesFromMeters(), sizeArcSec20000));

        }

        [TestMethod]
        public void DeploySonobuoy()
        {
            Player player = new Player();
            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            GameManager.Instance.CreateGame(player, "test game");
            GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
            GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
            Position pos = new Position(60, 3, 200, 80);
            BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, group, "sh60b", "Helo", pos, true);
            Assert.IsNotNull(unit, "Helo unit should not be null.");
            var unitOrder = OrderFactory.CreateSonobuoyDeploymentOrder(unit.Id, null, true, true);
            var baseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(unitOrder);
            Assert.IsNotNull(baseOrder, "Order should not be null");
            Assert.IsTrue(baseOrder.SpecialOrders == GameConstants.SpecialOrders.DropSonobuoy, "Should be sonobuoy order");
            unit.Orders.Enqueue(baseOrder);

            unit.ExecuteOrders();

            Sonobuoy buoy = (Sonobuoy)player.Units.First<BaseUnit>(u => u.UnitClass.UnitType == GameConstants.UnitType.Sonobuoy);

            Assert.IsNotNull(buoy, "Sonobuoy should exist");
            Assert.IsTrue(buoy.Position.HeightOverSeaLevelM < 0, "Sonobuoy should be below surface.");
            Assert.IsTrue(buoy.Sensors.Count<BaseSensor>() > 0, "Sonobuoy should have at least one sensor.");

        }

        [TestMethod]
        public void SensorBaffleZones()
        {
            Player player = new Player();
            Player playerEnemy = new Player();
            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            var game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
            GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
            Position pos = new Position(60, -3, -100, 180);
            Position pos2 = new Position(60, -3, 0, 180);
            BaseUnit unitSub = GameManager.Instance.GameData.CreateUnit(player, group, "yasen", "Yasen", pos, true);
            BaseUnit unitSirius = GameManager.Instance.GameData.CreateUnit(playerEnemy, new Group(), "siriusstar", "Ship", pos2.Offset(180,20000), true);
            Assert.IsNotNull(unitSub, "Sub unit should not be null.");
            Assert.IsNotNull(unitSirius, "Ship unit should not be null.");
            var sonarArray = unitSub.Sensors.FirstOrDefault<BaseSensor>(s => s.SensorClass.IsDeployableSensor) as Sonar;
            Assert.IsNotNull(sonarArray, "Sonar array should be found.");
            sonarArray.IsOperational = true;
            var distanceM = MapHelper.CalculateDistanceM(unitSirius.Position.Coordinate, unitSub.Position.Coordinate);
            var isDetected = sonarArray.AttemptDetectUnit(unitSirius, distanceM);

            Assert.IsFalse(isDetected, "Towed Sonar Array should not detect unit directly in front.");


        }

    }
}
