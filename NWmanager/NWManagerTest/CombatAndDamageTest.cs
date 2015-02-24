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
using TTG.NavalWar.NWComms.Entities;

namespace NWManagerTest
{
    /// <summary>
    /// Summary description for CombatAndDamageTest
    /// </summary>
    [TestClass]
    public class CombatAndDamageTest
    {
        public CombatAndDamageTest()
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
        public void AirfieldEngageTargets()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 0, 120);
            BaseUnit unitAirport = GameManager.Instance.GameData.CreateUnit(player, group1, "ukairportlarge", "Airport", pos1, true, true);

            Position pos2 = new Position(60, 3.2, 500, 120);
            BaseUnit unitMig = GameManager.Instance.GameData.CreateUnit(playerEnemy, group2, "mig29k", "MiG", pos2, true);
            //Coordinate NextDoor = MapHelper.CalculateNewPosition2(unitAirport.Position.Coordinate,
            //    45, 1000);
            Assert.IsNotNull(unitAirport, "Airport should exist.");
            Assert.IsNotNull(unitMig, "Mig should exist.");

            unitMig.UserDefinedSpeed = TTG.NavalWar.NWComms.GameConstants.UnitSpeedType.Slow;
            unitMig.ActualBearingDeg = 85;
            unitAirport.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);

            game.IsNetworkEnabled = false;
            var UnitList = player.GetSortedUnitsInAreaByRole(GameConstants.Role.AttackAir,
                unitMig.Position.Coordinate, GameConstants.DEFAULT_AA_DEFENSE_RANGE_M, true);
            Assert.IsTrue(UnitList.Any(), "At leat one unit (airport) should be available to fire on detected target.");
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();
            Assert.IsTrue(player.DetectedUnits.Count > 0 || playerEnemy.Units.Count < 1, "Player should have detected aircraft if it is not shot down.");


        }

        [TestMethod]
        public void BearingOnlyAttackAndSoftKillTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 0, 120);
            BaseUnit unitBurke = GameManager.Instance.GameData.CreateUnit(player, group1, "arleighburke", "DDG Valiant", pos1, true);
            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unitBurke.Position.Coordinate,
                45, 10000);
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemyBurke = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true);

            game.IsNetworkEnabled = false;

            //GameManager.Instance.Game.RunGameInSec = 1;
            //GameManager.Instance.Game.StartGamePlay();
            //var detectedEnemy = player.DetectedUnits[0];
            //Assert.IsNotNull(detectedEnemy, "Enemy should have been detected.");
            //adding two attack orders on same target; second should be ignored.
            var unitEngOrder = OrderFactory.CreateEngagePositionOrder(unitBurke.Id, 
                unitEnemyBurke.Position.GetPositionInfo(), "rgm84harpoon", 
                GameConstants.EngagementOrderType.BearingOnlyAttack, 
                GameConstants.EngagementStrength.MinimalAttack, 
                1);
            //var baseEngOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(unitEngOrder);
            //unitBurke.Orders.Enqueue(baseEngOrder);
            player.HandleMessageFromClient(unitEngOrder);
            Assert.IsTrue(unitBurke.Orders.Count > 0, "There should be an order in the queue on the Burke.");
            unitBurke.ExecuteOrders();
            var missiles = from u in player.Units
                           where u.UnitClass.IsMissileOrTorpedo
                           select u;
            int missileCount = missiles.Count<BaseUnit>();
            Assert.IsTrue(missileCount > 0, "At least one missile should have been launched on bearing only launch.");
            //var missileSearchOrder = from o in 
            var missile = missiles.FirstOrDefault<BaseUnit>() as MissileUnit;
            Assert.IsNotNull(missile.MovementOrder, "Missile has movement order");
            bool hasMissileSearchOrder = false;
            foreach (var wp in missile.MovementOrder.GetWaypoints())
            {
                if (wp.Orders.Count > 0)
                {
                    foreach (var o in wp.Orders)
                    {
                        if (o.OrderType == GameConstants.OrderType.MissileSearchForTarget)
                        {
                            hasMissileSearchOrder = true;
                        }
                    }
                }
            }
            Assert.IsTrue(hasMissileSearchOrder, "Missile should have a MissileSearchForTarget order");
            Assert.IsTrue(missile.TargetDetectedUnit == null, "Missile should not have a TargetDetectedUnit now");
            unitEnemyBurke.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
            unitBurke.SensorSweep();
            Assert.IsTrue(player.DetectedUnits.Count > 0, "At least one enemy unit should be detected.");

            missile.SearchForTarget();

            Assert.IsTrue(missile.TargetDetectedUnit != null, "Missile should now have a TargetDetectedUnit");
            bool isSoftKillPossible = false;
            BaseWeapon softKillWeapon = null;
            foreach (var wpn in unitEnemyBurke.Weapons)
	        {
                if (wpn.WeaponClass.IsNotWeapon && wpn.WeaponClass.EwCounterMeasures != GameConstants.EwCounterMeasuresType.None)
                {
                    if (missile.CanBeSoftKilled(wpn))
                    {
                        isSoftKillPossible = true;
                        softKillWeapon = wpn;
                    }
                }
	        }
            
            Assert.IsTrue(isSoftKillPossible, "Soft kill should be possible");
            Assert.IsNotNull(softKillWeapon, "Soft kill weapon should be specified");
            bool isItSoftKilled = unitEnemyBurke.InflictMissileSoftkill(missile); //this is random

        }


        [TestMethod]
        public void EngagementOrderTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 0, 120);
            BaseUnit unitBurke = GameManager.Instance.GameData.CreateUnit(player, group1, "arleighburke", "DDG Valiant", pos1, true);
            Position pos2 = new Position(60, 3.2, 500, 120);
            BaseUnit unitF22 = GameManager.Instance.GameData.CreateUnit(player, group2, "f22", "F22-AA", pos2, true);
            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unitBurke.Position.Coordinate,
                45, 1000);
            unitF22.UserDefinedSpeed = TTG.NavalWar.NWComms.GameConstants.UnitSpeedType.Slow;
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemyBurke = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true);

            game.IsNetworkEnabled = false;

            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();
            var detectedEnemy = player.DetectedUnits[0];
            Assert.IsNotNull(detectedEnemy,"Enemy should have been detected.");
            //adding two attack orders on same target; second should be ignored.
            unitBurke.EngageDetectedUnit(detectedEnemy, 
                TTG.NavalWar.NWComms.GameConstants.EngagementOrderType.EngageNotClose, false);
            unitBurke.EngageDetectedUnit(detectedEnemy, 
                TTG.NavalWar.NWComms.GameConstants.EngagementOrderType.EngageNotClose, false);
            var attackOrders = from o in unitBurke.Orders
                               where o is EngagementOrder && (o as EngagementOrder).TargetDetectedUnit.Id == detectedEnemy.Id
                               select o;
            int countAttackOnBurke = attackOrders.Count<BaseOrder>();
            Assert.IsTrue(countAttackOnBurke == 1, "Exactly one attack order on enemy Burke should be in orders queue.");
        }

        [TestMethod]
        public void TestWeaponsInSectorRange()
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

            Position pos = new Position(60, 3, 0, 75);
            var nextDoorPos = pos.Offset(75, 10000);
            nextDoorPos.HeightOverSeaLevelM = 0;

            var nextDoorPos2 = pos.Offset(255, 10050);
            nextDoorPos2.HeightOverSeaLevelM = 0;
            Group enemyGroup = new Group();
            var unitKirov = GameManager.Instance.GameData.CreateUnit(player, null, "kirov", "Kirov", pos, true);
            unitKirov.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);

            var unitF22 = GameManager.Instance.GameData.CreateUnit(player, null, "f22", "F22", pos, true);
            unitF22.Position.HeightOverSeaLevelM = 1000;
            unitF22.Position.BearingDeg = 255;

            var amraamWpn = unitF22.Weapons.FirstOrDefault<BaseWeapon>(w => w.WeaponClass.Id.Contains("amraam"));
            Assert.IsNotNull(amraamWpn, "AMRAAM should be found on F-22,");

            var unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, enemyGroup, "pamela", "Pamela 1", nextDoorPos2, true);
            unitKirov.SensorSweep();
            var detUnit = player.DetectedUnits[0];

            var bearingToTarget = MapHelper.CalculateBearingDegrees(unitKirov.Position.Coordinate, detUnit.Position.Coordinate);

            foreach (var wpn in unitKirov.Weapons)
            {
                var wpmBearingDeg = wpn.GetCurrentWeaponBearingDeg();
                var wpnBearingRange = wpn.WeaponClass.WeaponBearingRangeDeg;
                var isInBearing = wpn.IsCoordinateInSectorRange(unitEnemy.Position.Coordinate);
                if (wpn.WeaponClass.Id == "gsh30")
                {
                    Assert.IsFalse(isInBearing, "Gsh-30 should not be in bearing");
                }
            }
            Assert.IsTrue(amraamWpn.IsCoordinateInSectorRange(unitEnemy.Position.Coordinate),"AMRAAM should be in sector range.");
            unitF22.Position.BearingDeg = 75;
            Assert.IsFalse(amraamWpn.IsCoordinateInSectorRange(unitEnemy.Position.Coordinate), "AMRAAM should NOT be in sector range.");


        }

        [TestMethod]
        public void MissileTrackingTargetAltitudeTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 100, 70);
            BaseUnit unitF22 = GameManager.Instance.GameData.CreateUnit(player, group1, "f22", "F22", pos1, true);
            unitF22.SetWeaponLoad("Air superiority");
            unitF22.UserDefinedElevation = GameConstants.HeightDepthPoints.Low;
            var wpnAmraam = unitF22.Weapons.FirstOrDefault<BaseWeapon>(w => w.WeaponClass.WeaponClassName.Contains("RAAM"));
            Assert.IsNotNull(wpnAmraam, "Amraam weapon should be found on F22.");

            var pos2 = pos1.Offset(70, wpnAmraam.WeaponClass.EffectiveWeaponRangeM - 10000.0);
            pos2.HeightOverSeaLevelM = 10000;
            var unitEnemySentry = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "e3sentry", "Sentry", pos2, true);
            unitEnemySentry.UserDefinedElevation = GameConstants.HeightDepthPoints.High;
            unitF22.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
            unitF22.SensorSweep();

            var detSentry = player.DetectedUnits.FirstOrDefault<DetectedUnit>();
            Assert.IsNotNull(detSentry, "Sentry should be detected.");
            var distanceM = MapHelper.CalculateDistance3DM(unitF22.Position, detSentry.Position);
            var firedCount = wpnAmraam.Fire(detSentry, 1, distanceM);
            Assert.IsTrue(firedCount > 0, "At least one missile should be fired against the Sentry");
            var unitAmraam = player.Units.FirstOrDefault<BaseUnit>(u => u.UnitClass.IsMissileOrTorpedo) as MissileUnit;
            Assert.IsNotNull(unitAmraam, "Amraam missile should exist.");
            game.RunGameInSec = 10;
            game.StartGamePlay();

            //while (!unitAmraam.IsMarkedForDeletion)
            //{
            //    unitAmraam.MoveToNewCoordinate(100);
            //    var heightM = (double)unitAmraam.ActualHeightOverSeaLevelM;

            //}

            Assert.IsTrue(unitAmraam.ActualHeightOverSeaLevelM == unitEnemySentry.ActualHeightOverSeaLevelM, "Amraam and Sentry should now have the same elevation.");

        }

        [TestMethod]
        public void MissileSearchForTargetTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 1000, 70);
            BaseUnit unitP8 = GameManager.Instance.GameData.CreateUnit(player, group1, "p8poseidon", "Poseidon", pos1, true);
            string weaponLoadName = "Naval strike";
            unitP8.SetWeaponLoad(weaponLoadName);
            Assert.IsTrue(unitP8.CurrentWeaponLoadName == weaponLoadName, "P8 should be set to naval strike");
            var wpnHarpoon = unitP8.Weapons.FirstOrDefault<BaseWeapon>(w=> w.WeaponClass.WeaponClassName.Contains("arpoon"));
            var pos2 = pos1.Offset(70, wpnHarpoon.WeaponClass.EffectiveWeaponRangeM - 10000.0);
            var unitPamela = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "pamela", "Pamela", pos2, true);
            pos2.HeightOverSeaLevelM = 1000;
            var unitHelo = GameManager.Instance.GameData.CreateUnit(player, null, "sh60b", "sh60b", pos2, true);
            unitHelo.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
            unitHelo.SensorSweep();

            Assert.IsTrue(player.DetectedUnits.Count > 0, "Player should have detected at least one unit.");
            var detPamela = player.DetectedUnits.First<DetectedUnit>();

            unitP8.EngageDetectedUnit(detPamela, GameConstants.EngagementOrderType.EngageNotClose, false);
            unitP8.ExecuteOrders();

            var missiles = from m in player.Units
                           where m.UnitClass.IsMissileOrTorpedo
                           select m;
            
            Assert.IsTrue(missiles.Count<BaseUnit>() > 0, "There should be missiles that have been fired.");
            var missile = missiles.FirstOrDefault<BaseUnit>() as MissileUnit;

            var distanceToTarget = double.MaxValue;
            while (distanceToTarget > 4000 && !missile.IsMarkedForDeletion)
            {
                distanceToTarget = MapHelper.CalculateDistance3DM(missile.Position, unitPamela.Position);
                missile.MoveToNewPosition3D(1);
                
            }
            Assert.IsTrue(missile.IsMarkedForDeletion == false, "Missile should not have been deleted.");
            Assert.IsTrue(distanceToTarget <= 4000, "Distance should be under 4000m");
            
            missile.SearchForTarget();

            detPamela.IsMarkedForDeletion = true;
            player.DetectedUnits.Clear();

            //Search for target while it actually exists
            unitHelo.SensorSweep();
            missile.SearchForTarget();
            
            Assert.IsTrue(missile.TargetDetectedUnit != null, "Missile should now have a target.");
            
            
            //Search for target while it does not exist!
            missile.TargetDetectedUnit.IsMarkedForDeletion = true;
            unitPamela.IsMarkedForDeletion = true; 
            player.DetectedUnits.Clear();
            missile.SearchForTarget();

            Assert.IsTrue(missile.IsMarkedForDeletion, "Missile should have been deleted.");
        }

        [TestMethod]
        public void MineDeploymentTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 0, 120);
            BaseUnit unitPamela = GameManager.Instance.GameData.CreateUnit(player, group1, "pamela", "Pamela", pos1, true);
            
            Position pos2 = new Position(60, 3.01, 500, 120);

            unitPamela.MovementOrder.AddWaypoint(new Waypoint(pos2.Clone()));
            unitPamela.SetActualSpeed(30);
            BaseUnit unitHelo = GameManager.Instance.GameData.CreateUnit(playerEnemy, group2, "sh60b", "Helo", pos2, true);
            unitHelo.SetWeaponLoad("Deploy mines");
            var wpn = unitHelo.GetSpecialWeapon(GameConstants.SpecialOrders.DropMine);
            Assert.IsNotNull(wpn, "Helo should have mine deployment weapon.");
            var unitOrder = OrderFactory.CreateMineDeploymentOrder(unitHelo.Id, pos2.GetPositionInfo(), true);
            var baseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(unitOrder);
            unitHelo.Orders.Enqueue(baseOrder);
            game.IsNetworkEnabled = false;
            game.RealTimeCompressionFactor = 10;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();

            var mines = unitHelo.OwnerPlayer.Units.Where<BaseUnit>(u => u.UnitClass.UnitType == GameConstants.UnitType.Mine);
            Assert.IsTrue(mines.Count<BaseUnit>() > 0, "There is at least one mine in the game.");
            foreach (var mine in mines)
            {
                mine.ReadyInSec = 0;
            }
            var aMine = mines.First<BaseUnit>();
            Assert.IsNotNull(aMine, "Mine should be found");
            unitPamela.Position = aMine.Position.Clone();
            unitPamela.UserDefinedSpeed = GameConstants.UnitSpeedType.Slow;
            var enemyUnits = unitHelo.OwnerPlayer.GetEnemyUnitsInAreaByUnitType(GameConstants.UnitType.SurfaceShip, aMine.Position.Coordinate, 1000);
            Assert.IsTrue(enemyUnits.Count > 0, "There should be more than one unit in area");
            Assert.IsTrue(aMine.IsMarkedForDeletion == false, "Mine should not be marked for deletion.");
            //GameManager.Instance.Game.IsGameLoopStarted = false;
            //GameManager.Instance.Game.RunGameInSec = 10;
            //GameManager.Instance.Game.StartGamePlay();
            //mines = unitHelo.OwnerPlayer.Units.Where<BaseUnit>(u => u.UnitClass.UnitType == GameConstants.UnitType.Mine);

        }

        [TestMethod]
        public void GroupEngagementTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            GameManager.Instance.GameData.LoadGameScenario("small", game);
            Player otherPlayer = player.Enemies[0];
            Assert.IsTrue(game.Players.Count == 2, "There should be two players in game.");
            //Assert.IsTrue(otherPlayer.Units.Count == 15, "Player 1 should have 15 units.");
            BaseUnit unit = player.GetUnitById("tag:main");
            Assert.IsNotNull(unit, "Unit with tag main should exist.");
            var wp = unit.MovementOrder.GetActiveWaypoint();
            Assert.IsNotNull(wp, "Main unit waypoint should not be null.");
            BaseUnitInfo info = unit.GetBaseUnitInfo();
            Assert.IsTrue(info.IsGroupMainUnit, "Main unit should be main unit");

            //foreach (var u in player.Units)
            //{
            //    if (string.IsNullOrEmpty(u.CurrentWeaponLoadName))
            //    {
            //        GameManager.Instance.Log.LogDebug(
            //            string.Format("Unit {0} has empty CurrentWeaponLoad.", u.ToShortString()));
            //    }
            //}
            game.IsNetworkEnabled = false;
            game.RealTimeCompressionFactor = 10;
            GameManager.Instance.Game.RunGameInSec = 1;
            var sentryPlane = player.GetUnitById("tag:sentry");
            Assert.IsNotNull(sentryPlane,"Sentry plane should exist and not be null.");
            sentryPlane.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);

            GameManager.Instance.Game.StartGamePlay();
            var detectedGroup = player.DetectedGroups.FirstOrDefault<DetectedGroup>();
            Assert.IsNotNull(detectedGroup,"Detected group should be found");
            var result = unit.EngageDetectedGroup(detectedGroup, GameConstants.EngagementOrderType.EngageNotClose, GameConstants.EngagementStrength.DefaultAttack, true);
            Assert.IsTrue(result, "EngageDetectedGroup should respond true");

        }

        [TestMethod]
        public void CloseAndEngageAirTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 1000, 120);

            var unitF22 = GameManager.Instance.GameData.CreateUnit(player, group1, "f22", string.Empty, pos1, true);
            unitF22.UserDefinedElevation = GameConstants.HeightDepthPoints.MediumHeight;
            unitF22.UserDefinedSpeed = GameConstants.UnitSpeedType.Half;
            var maxRangeWpn = unitF22.GetMaxWeaponRangeM(GameConstants.DomainType.Air);
            var pos2 = pos1.Offset(90, maxRangeWpn * 1.5);
            var unitHelo = GameManager.Instance.GameData.CreateUnit(playerEnemy, group2, "ka27", string.Empty, pos2, true);
            unitHelo.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
            unitHelo.UserDefinedElevation = GameConstants.HeightDepthPoints.MediumHeight;
            unitHelo.UserDefinedSpeed = GameConstants.UnitSpeedType.Half;
            
            unitF22.SensorSweep();

            var detHelo = player.DetectedUnits.FirstOrDefault<DetectedUnit>();
            Assert.IsNotNull(detHelo, "Helo should be detected");

            var canShoot = unitF22.CanImmediatelyFireOnTargetType(detHelo);
            Assert.IsFalse(canShoot, "F22 should not be able to fire on helo now");

            var canEngage = unitF22.EngageDetectedUnit(detHelo, GameConstants.EngagementOrderType.CloseAndEngage, false);
            Assert.IsTrue(canEngage, "F22 should be able to engage helo.");

            var engOrder = unitF22.Orders.FirstOrDefault<BaseOrder>();
            Assert.IsNotNull(engOrder, "F22 should have an engagement order");

            unitF22.ExecuteOrders();
            var moveEngageOrder = unitF22.Orders.FirstOrDefault<BaseOrder>();

            Assert.IsNull(moveEngageOrder, "F22 should now not have a movement order");
            Assert.IsTrue(unitF22.MovementOrder.GetActiveWaypoint().TargetDetectedUnit != null, "F22 should have wp with target unit.");








        }

        [TestMethod]
        public void CloseAndEngageTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 3, 0, 120);
            BaseUnit unitBurke = GameManager.Instance.GameData.CreateUnit(player, group1, "arleighburke", "DDG Valiant", pos1, true);
            unitBurke.SetSensorsActivePassive(GameConstants.SensorType.Sonar, true);
            unitBurke.UserDefinedSpeed = GameConstants.UnitSpeedType.Crawl;
            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unitBurke.Position.Coordinate,
                45, 1000);
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemyBurke = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true);


            var subPos = pos1.Offset(4, 1100);
            subPos.HeightOverSeaLevelM = -40;
            var unitEnemySub = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, null, "lada", "Lada sub", subPos, true);

            Position pos2 = NextDoorPos.Offset(90, 120000);
            pos2.BearingDeg = 90;
            BaseUnit unitF22 = GameManager.Instance.GameData.CreateUnit(player, group2, "f22", "F22-AA", pos2, true);
            unitF22.SetWeaponLoad("Naval strike");
            unitF22.UserDefinedSpeed = TTG.NavalWar.NWComms.GameConstants.UnitSpeedType.Slow;
            var weaponHarpoons = unitF22.Weapons.Where<BaseWeapon>(w => w.WeaponClass.Id == "jassmer");
            var weaponHarpoon = weaponHarpoons.First<BaseWeapon>();
            Assert.IsNotNull(weaponHarpoon, "JASSM-ER weapon should be found on F22");
            foreach (var wpn in unitBurke.Weapons)
            {
                wpn.AmmunitionRemaining = 0;
            }
            foreach (var wpn in unitEnemyBurke.Weapons)
            {
                wpn.AmmunitionRemaining = 0;
            }
            pos2 = NextDoorPos.Offset(90, weaponHarpoon.WeaponClass.MaxWeaponRangeM + 5000);
            pos2.BearingDeg = 90;
            unitF22.Position = pos2.Clone();

            game.IsNetworkEnabled = false;

            var gameTimeStartUp = GameManager.Instance.Game.GameCurrentTime;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();

            var gameTimeAfter = GameManager.Instance.Game.GameCurrentTime;
            var gameTimeElapsed = gameTimeAfter.Subtract(gameTimeStartUp);

            //Assert.IsTrue(Math.Abs(gameTimeElapsed.TotalSeconds - 20.0) < 0.01, "10 seconds of gametime should have elapsed."); //different on 32 and 64-bit!
            
            Assert.IsTrue(unitF22.CurrentWeaponLoadName.ToLower() == "naval strike", "F22 should be set for naval strike");

            var detectedEnemyShip = player.DetectedUnits.FirstOrDefault<DetectedUnit>(u=>u.DomainType == GameConstants.DomainType.Surface);
            var detectedEnemySub = player.DetectedUnits.FirstOrDefault<DetectedUnit>(u => u.DomainType == GameConstants.DomainType.Subsea);
            var detectedMyUnit = playerEnemy.DetectedUnits[0];
            Assert.IsNotNull(detectedEnemyShip, "Enemy should have been detected.");
            Assert.IsNotNull(detectedMyUnit, "Enemy should have detected my unit.");
            Assert.IsNotNull(detectedEnemySub, "Enemy sub should have been detected.");

            detectedEnemyShip.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
            detectedEnemySub.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
            foreach (var wpn in unitBurke.Weapons) //rearm again
            {
                wpn.AmmunitionRemaining = wpn.MaxAmmunition;
            }
            
            var allWeaponStatuses = unitBurke.GetAllWeaponEngagementStatuses("", detectedEnemySub);
            
            var bestWeaponSub = unitBurke.GetBestAvailableWeapon("", detectedEnemySub, false);
            Assert.IsNotNull(bestWeaponSub, "A best weapon to target sub should be found.");
            Assert.IsTrue(bestWeaponSub.EngagementStatusResult == GameConstants.EngagementStatusResultType.ReadyToEngage, "ASW weapon should be ready to engage.");

            double distanceBefore = MapHelper.CalculateDistance3DM(unitF22.Position, detectedEnemyShip.Position);
            bool hasAnyEngagementOrders = unitF22.HasAnyEngagementOrders();
            Assert.IsTrue(!hasAnyEngagementOrders, "Should NOT have engagement orders");
            unitF22.UserDefinedSpeed = TTG.NavalWar.NWComms.GameConstants.UnitSpeedType.Cruise;
            EngagementOrder order = new EngagementOrder(
                detectedEnemyShip, TTG.NavalWar.NWComms.GameConstants.EngagementOrderType.CloseAndEngage);
            unitF22.Orders.Enqueue(order);
            unitF22.ExecuteOrders();
            hasAnyEngagementOrders = unitF22.HasAnyEngagementOrders();
            //bool hasSpecificEngOrder = unitF22.HasAnyEngagementOrders(detectedEnemyShip);
            Assert.IsTrue(hasAnyEngagementOrders, "Should have engagement orders");
            //Assert.IsTrue(hasSpecificEngOrder, "Should have engagement orders");
            unitF22.EngageDetectedUnit(
                detectedEnemyShip, 
                TTG.NavalWar.NWComms.GameConstants.EngagementOrderType.CloseAndEngage, 
                true);

            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.IsGameLoopStarted = false;

            GameManager.Instance.Game.StartGamePlay();
            double distanceAfter = MapHelper.CalculateDistance3DM(unitF22.Position, detectedEnemyShip.Position);
            Assert.IsTrue(distanceBefore > distanceAfter, "F22 should have closed the distance.");
            GameManager.Instance.Game.RealTimeCompressionFactor = 1;
            //var engageOrder = unitF22.MovementOrder.GetActiveWaypoint().Orders[0];
            //Assert.IsNotNull(engageOrder, "F22 should have an order in waypoint");
            //var engageOrderAsEngage = engageOrder as EngagementOrder;
            //Assert.IsNotNull(engageOrderAsEngage, "F22 order should be engagementorder");
            //Assert.IsTrue(engageOrderAsEngage.TargetDetectedUnit.Id == detectedEnemy.Id, 
            //    "Enemy target should be in wp order.");
            var bestWpnF22 = unitF22.GetUnitEngagementStatus("", detectedEnemyShip, true);

            foreach (var wpn in unitBurke.Weapons)
            {
                wpn.AmmunitionRemaining = wpn.WeaponClass.MaxAmmunition;
            }
            foreach (var wpn in unitEnemyBurke.Weapons)
            {
                wpn.AmmunitionRemaining = wpn.WeaponClass.MaxAmmunition;
            }

            var bestWpnBurke = unitBurke.GetUnitEngagementStatus("", detectedEnemyShip, false);
            var allWpnStatusesBurke = unitBurke.GetAllWeaponEngagementStatuses("", detectedEnemyShip);
            var allWpnStatusesF22 = unitF22.GetAllWeaponEngagementStatuses("", detectedEnemyShip);
            
            var allWpnStatusesEnemyBurke = unitEnemyBurke.GetAllWeaponEngagementStatuses("", detectedMyUnit);
            Assert.IsTrue(bestWpnF22.Weapon.CanTargetDetectedUnit(detectedEnemyShip, true),
                "Selected weapon should be able to target enemy");
            Assert.IsTrue(bestWpnBurke.Weapon.CanTargetDetectedUnit(detectedEnemyShip, true),
                            "Selected weapon should be able to target enemy");
            // Assert.IsTrue(bestWpnF22.WeaponCanBeUsedAgainstTarget, "Can be used against target");
            Assert.IsTrue(bestWpnBurke.WeaponCanBeUsedAgainstTarget, "Can be used against target");

            //GameManager.Instance.Game.RealTimeCompressionFactor = 100;
            //GameManager.Instance.Game.RunGameInSec = 8;
            //GameManager.Instance.Game.IsGameLoopStarted = false;

            //GameManager.Instance.Game.StartGamePlay();
            //double distanceAfterLast = MapHelper.CalculateDistance3DM(unitF22.Position, detectedEnemy.Position);
            ////Assert.IsTrue(distanceAfterLast < distanceAfter, "F22 should have closed the distance even further.");
            //Assert.IsTrue(detectedEnemy.IsFiredUpon, "Enemy should have been fired upon!");
            //var missiles = player.Units.Where<BaseUnit>(u => u.UnitClass.IsMissileOrTorpedo);
            //Assert.IsTrue(missiles.Count<BaseUnit>() > 0, "At least one missile should exist.");
        }

        [TestMethod]
        public void LandInstallationsFireOnAir()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            Group group1 = new Group();
            Group group2 = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);
            GameManager.Instance.Game.SetAllPlayersEnemies();

            Position pos1 = new Position(60, 6, 0, 70);
            Position pos2 = new Position(65, 6, 0, 70);
            Position pos3 = new Position(70, 6, 0, 70);
            var airport1 = GameManager.Instance.GameData.CreateUnit(player, null, "ukairportlarge", "Airport1", pos1, true);
            var airport2 = GameManager.Instance.GameData.CreateUnit(player, null, "ukairportlarge", "Airport2", pos2, true);
            var airport3 = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "russianairportlarge", "Airport3", pos3, true);
            Assert.IsNotNull(airport1, "Airport1 should not be null");

            for (int i = 0; i < 5; i++)
            {
                var helo = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "sh60b", "Airport", 
                    pos1.Offset(GameManager.Instance.GetRandomNumber(360),GameManager.Instance.GetRandomNumber(10000)), true);
                helo.Position.HeightOverSeaLevelM = 100;
                Assert.IsNotNull(helo, "Helo " + i + " should not be null");
            }
            for (int i = 0; i < 5; i++)
            {
                var helo = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "sh60b", "Airport",
                    pos2.Offset(GameManager.Instance.GetRandomNumber(360), GameManager.Instance.GetRandomNumber(10000)), true);
                helo.Position.HeightOverSeaLevelM = 100;
                Assert.IsNotNull(helo, "Helo " + i + " should not be null");
            }
            player.DefaultWeaponOrders = GameConstants.WeaponOrders.FireOnAllClearedTargets;
            airport1.WeaponOrders = GameConstants.WeaponOrders.FireOnAllClearedTargets;
            airport2.WeaponOrders = GameConstants.WeaponOrders.FireOnAllClearedTargets;

            airport1.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
            airport1.SensorSweep();
            airport2.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
            airport2.SensorSweep();

            Assert.IsTrue(player.DetectedUnits.Count > 0, "At least one unit should be detected");
            if (player.AIHandler == null)
            {
                player.AIHandler = new TTG.NavalWar.NWData.Ai.ComputerAIHandler();
                player.AIHandler.OwnerPlayer = player;
            }
            player.IsComputerPlayer = true;
            player.AIHandler.EngageDetectedAirTargets();

            var wpn = airport1.Weapons[0];
            Assert.IsTrue(airport1.Orders.Count > 0, "Airport1 should now have (engagement) orders.");
            Assert.IsTrue(airport2.Orders.Count > 0, "Airport2 should now have (engagement) orders.");
            airport1.ExecuteOrders();
            airport2.ExecuteOrders();

            Assert.IsNotNull(wpn, "Airport Wpn should not be null");

            var missiles = from u in player.Units where u.UnitClass.UnitType == GameConstants.UnitType.Missile select u;
            Assert.IsTrue(missiles.Count() >= 2, "There should have been at least two missiles in the air");

        }

    }
}
