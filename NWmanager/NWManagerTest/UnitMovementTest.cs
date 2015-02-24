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
using System.Diagnostics;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace NWManagerTest
{
	/// <summary>
	/// Summary description for UnitMovementTest
	/// </summary>
	[TestClass]
	public class UnitMovementTest
	{
		public UnitMovementTest()
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
		public void UpdateActualBearingTest()
		{
			Player player = new Player();
			Group group = new Group();
			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(player,"test game");
			GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
			GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
			Position pos = new Position(60, 3, 0, 80);
			//Position dest = new Position(65, 3, 0, 0);
			BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "Arleigh Burke", pos, true, true);
			double runTime = unit.UnitClass.TurnRangeDegrSec / 10;
			unit.Position.SetNewBearing(0);

			double desiredBearingDeg = 80;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			double actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 9 && actualBearingDeg < 20, "ActualBearingDeg should be between 9 and 20.");

			unit.Position.SetNewBearing(45);
			desiredBearingDeg = 25;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 34 && actualBearingDeg < 40, "ActualBearingDeg should be between 34 and 40.");

			unit.Position.SetNewBearing(350);
			desiredBearingDeg = 180;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 330 && actualBearingDeg < 350, "ActualBearingDeg should be between 330 and 350.");

			unit.Position.SetNewBearing(355);
			desiredBearingDeg = 10;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 1 && actualBearingDeg < 10, "ActualBearingDeg should be between 1 and 10.");

			unit.Position.SetNewBearing(180);
			desiredBearingDeg = 5;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 165 && actualBearingDeg < 175, "ActualBearingDeg should be between 165 and 175.");

			unit.Position.SetNewBearing(170);
			desiredBearingDeg = 360;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 155 && actualBearingDeg < 165, "ActualBearingDeg should be between 155 and 165.");

			unit.Position.SetNewBearing(355);
			desiredBearingDeg = 180;
			unit.DesiredBearingDeg = desiredBearingDeg;
			unit.UpdateActualBearing(runTime);
			actualBearingDeg = (double)unit.ActualBearingDeg;
			Assert.IsTrue(actualBearingDeg > 340 && actualBearingDeg < 350, "ActualBearingDeg should be between 340 and 350.");


		}

		[TestMethod]
		public void AircraftMovement()
		{
			GameManager.Instance.Log.LogDebug("*** Running test AircraftMovement");
			Player player = new Player();
			Group group = new Group();
			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(player, "test game");
			GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
			GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
			Position pos = new Position(60, 3, 0, 80);
			BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "Arleigh Burke", pos, true);
			AircraftUnit helo = unit.AircraftHangar.Aircraft.First<AircraftUnit>();
			unit.LaunchAircraft(new List<AircraftUnit> { helo }, string.Empty, string.Empty, null, "");
			var wp = new Waypoint(new Position(new Coordinate(70, -13)));
			wp.Position.HeightOverSeaLevelM = 500;
			var moveOrder = new MovementOrder(wp);
			helo.MovementOrder = moveOrder;
			helo.SetActualSpeed(300);
			helo.UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
			int countLoop = 100;
			double lastBearingDeg = (double)helo.ActualBearingDeg;
			for (int i = 0; i < countLoop; i++)
			{
				helo.MoveToNewPosition3D(1);
				double newBearingDeg = (double)helo.ActualBearingDeg;
				double bearingDiffRad = Math.Abs(newBearingDeg.ToRadian() - lastBearingDeg.ToRadian());
				if (i > 2 && bearingDiffRad > 0.001)
				{
					GameManager.Instance.Log.LogDebug(
						string.Format("BearingDeg has changed: Iteration {0}  Old {1:F}   New {2:F}", 
						i, lastBearingDeg, newBearingDeg));
					Assert.IsTrue(false, "Bearing should never change abruptly.");
				}
				lastBearingDeg = newBearingDeg;
			}
			double distanceM = MapHelper.CalculateDistance3DM(pos, helo.Position);
			Assert.IsTrue(distanceM > 8000, "Helo should have moved over 8000m."); //in fact, around 8333 m in 100 sec at 300 kph

		}
		[TestMethod]
		public void CreateUnitFromClass()
		{
			Player player = new Player();
			Group group = new Group();
			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(player,"test game");
			GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
			GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
			Position pos = new Position(60, 3, 0, 80);
			BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "Arleigh Burke", pos, true);
			AircraftUnit helo = unit.AircraftHangar.Aircraft.First<AircraftUnit>();
			Assert.IsNotNull(helo, "Helo in hangar should not be null.");
			Assert.IsTrue(helo.ReadyInSec == 0, "Helo in hangar should be ready.");
			unit.LaunchAircraft(new List<AircraftUnit> { helo }, string.Empty, string.Empty, null, "");
			Assert.IsTrue(unit.SupportsRole(GameConstants.Role.AttackSurface));
			Assert.IsNotNull(unit.Weapons.First<BaseWeapon>(), "No weapons!");
			Assert.IsNotNull(unit.AircraftHangar, "Has no aircraft hangar!");

			Assert.IsTrue(helo.Position != null, "Launched helo group should not be null."); //no idea why this sometimes fails after rebuild!
			Assert.IsTrue(unit.AircraftHangar.Aircraft.Count == 1, "Destroyer should have one carried unit left.");
			Assert.IsNotNull(unit.AircraftHangar.Aircraft.First<AircraftUnit>().Weapons.First<BaseWeapon>(),"Carried helo should have weapon.");

			GameManager.Instance.TerminateGame();
		}

		[TestMethod]
		public void MissileLaunchAndMovement()
		{
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

			Position pos1 = new Position(60, 3, 0, 70+180);
			Position pos2 = pos1.Offset(new PositionOffset(1800,1780));
			BaseUnit unitNansen = GameManager.Instance.GameData.CreateUnit(player1, group1, "fridtjofnansen", "Nansen", pos1, true);
			AircraftUnit helo = unitNansen.AircraftHangar.Aircraft.First<AircraftUnit>();
			unitNansen.LaunchAircraft(new List<AircraftUnit> { helo }, string.Empty, "", null, "");
            helo.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);

			BaseUnit unitEnemyBurke = GameManager.Instance.GameData.CreateUnit(player2, group2, "arleighburke", "Enemy Burke", pos2, true);
			unitNansen.SensorSweep();
			helo.SensorSweep();

			Assert.IsTrue(player1.DetectedUnits.Count == 1, "Player1 should have detected one unit.");
			DetectedUnit enemyUnit = player1.DetectedUnits[0];
			enemyUnit.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
			var engagementOrder = new EngagementOrder(enemyUnit, GameConstants.EngagementOrderType.EngageNotClose);
			engagementOrder.WeaponClassId = "nsm";
			unitNansen.Orders.Enqueue(engagementOrder);
			unitNansen.ExecuteOrders();
			MissileUnit missile = (MissileUnit)player1.Units.First<BaseUnit>(u=>u.UnitClass.IsMissileOrTorpedo);
			double OldDistance = MapHelper.CalculateDistanceM(
				unitEnemyBurke.Position.Coordinate, missile.Position.Coordinate);
            double distanceM = 0;
            int iterations = 0;
            do
            {
                iterations++;
                missile.MoveToNewCoordinate(100);
                var heightM = missile.Position.HeightOverSeaLevelM;
                distanceM = MapHelper.CalculateDistanceM(
                    unitEnemyBurke.Position.Coordinate, missile.Position.Coordinate);
                if (distanceM < 5000)
                {
                    GameManager.Instance.Log.LogDebug(string.Format(
                        "MissileLaunchAndMovement test: {0} at pos {1} ** distanceM={2} FuelM={3}", 
                        missile, missile.Position, Math.Round(distanceM,0), Math.Round(missile.FuelDistanceRemainingM,0)));
                }
                
            } while (distanceM > GameConstants.DISTANCE_TO_TARGET_IS_HIT_M * 2 && !missile.IsMarkedForDeletion);
			
			double newDistance = MapHelper.CalculateDistanceM(
				unitEnemyBurke.Position.Coordinate, missile.Position.Coordinate);
			Assert.IsTrue(newDistance < OldDistance, "Missile should have moved closer to target.");
            Assert.IsTrue(newDistance < 200, "Missile should be closer to target than 200m");


		}

		[TestMethod]
		public void SimpleUnitMovement()
		{
			Player player = new Player();
			GameManager.Instance.CreateGame(new Player(), "test game");
			GameManager.Instance.GameData.InitAllData();
			BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, new Group(), 
				"arleighburke", "", new Position(60.0, 1.0, 0, 2), true);
			Position dest = new Position(59.0, 2.0);
			unit.MovementOrder.AddWaypoint((new Waypoint(dest)));
			unit.ActualSpeedKph = 40.0;
			int MaxIterations = 1000000;
			int Count = 0;
			//double DistanceToTargetM = 0;
			do
			{
				unit.MoveToNewCoordinate(1000.0);
				//DistanceToTargetM = MapHelper.CalculateDistanceMeters(unit.Position.Coordinate, dest.Coordinate);
				
				Count++;
			} while (unit.MostRecentDistanceToTargetM > 10 && Count < MaxIterations);

			GameManager.Instance.Log.LogDebug("SimpleUnitMovement test: Unit short = " + unit.ToShortString());
			GameManager.Instance.Log.LogDebug("SimpleUnitMovement test: Unit long = " + unit.ToLongString());
			GameManager.Instance.Log.LogDebug("SimpleUnitMovement test: Position = " + unit.Position.ToString());

			Assert.IsFalse(Count >= MaxIterations, "Count should not go MaxIterations iterations.");
			Assert.AreNotEqual(unit.Position.Coordinate.LatitudeDeg, 60.0, "unit should not still be at latitude 60");
			Assert.AreNotEqual(unit.Position.Coordinate.LongitudeDeg, 1.0, "unit should not still be at longitude 1");
			Assert.IsNotNull(unit.Position.BearingDeg, "Bearing should not be null.");

			GameManager.Instance.TerminateGame();
		}

		[TestMethod]
		public void SimpleUnitMovementDetail()
		{
			Player player = new Player();
			GameManager.Instance.CreateGame(new Player(), "test game");
			GameManager.Instance.GameData.InitAllData();
			BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, new Group(),
				"f22", "", new Position(60.0, 5.0, 0, 2), true);
			Position dest1 = new Position(62.0, -1.0);
			Position dest2 = new Position(63.0, 0.0);
			Position finalDest = new Position(61.0, 0.5);
			Position extraDest = new Position(65.0, 2);
			GameManager.Instance.Game.RealTimeCompressionFactor = 1.0;
			unit.MovementOrder.AddWaypoint(dest1);
			unit.MovementOrder.AddWaypoint(dest2);
			unit.MovementOrder.AddWaypoint(finalDest.Clone());
			unit.UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
			int MaxIterations = 1000000;
			int Count = 0;
			//double DistanceToTargetM = 0;
			double[] listDistances = new double[MaxIterations];
			Position previousPosition = unit.Position.Clone();
			double distanceToFinalTargetM = double.MaxValue;
			double maxDeviationM = 250.0 + 250.0; //double to account for "snap" on arrival
			do
			{
				unit.MoveToNewCoordinate(1000.0);
				//DistanceToTargetM = MapHelper.CalculateDistanceMeters(unit.Position.Coordinate, dest.Coordinate);
				if (Count > 0)
				{

					listDistances[Count] = MapHelper.CalculateDistanceM(unit.Position.Coordinate, previousPosition.Coordinate);
					Assert.IsTrue(listDistances[Count] < maxDeviationM , "Unit should never move faster than MaxDev m/sec. Distance=" + Math.Round(listDistances[Count]),2); //~235 m/sec

				}
				previousPosition = unit.Position.Clone();
				distanceToFinalTargetM = MapHelper.CalculateDistanceM(unit.Position.Coordinate, finalDest.Coordinate);
				Count++;

			} while (distanceToFinalTargetM > 100 && Count < MaxIterations);

			GameManager.Instance.Log.LogDebug("SimpleUnitMovement test: Unit short = " + unit.ToShortString());
			GameManager.Instance.Log.LogDebug("SimpleUnitMovement test: Unit long = " + unit.ToLongString());
			GameManager.Instance.Log.LogDebug("SimpleUnitMovement test: Position = " + unit.Position.ToString());

			Assert.IsFalse(Count >= MaxIterations, "Count should not go MaxIterations iterations.");
			Assert.AreNotEqual(unit.Position.Coordinate.LatitudeDeg, 60.0, "unit should not still be at latitude 60");
			Assert.AreNotEqual(unit.Position.Coordinate.LongitudeDeg, 1.0, "unit should not still be at longitude 1");
			Assert.IsNotNull(unit.Position.BearingDeg, "Bearing should not be null.");

			unit.MovementOrder.AddWaypoint(dest1);
			unit.MovementOrder.AddWaypoint(dest2);
			unit.MovementOrder.AddWaypoint(finalDest.Clone());

			unit.MovementOrder.AddWaypointToTop(extraDest.Clone());
			var wp = unit.MovementOrder.GetActiveWaypoint();
			var dist = MapHelper.CalculateDistance3DM(wp.Position, extraDest);
			Assert.IsTrue(dist < 1, "ActiveWaypoint and extra waypoint should be the same.");
			var unitInfo = unit.GetBaseUnitInfo();
			var infoWp = unitInfo.Waypoints[0];
			dist = MapHelper.CalculateDistance3DM(wp.Position, new Position(infoWp.Position));
			Assert.IsTrue(dist < 1, "ActiveWaypoint and BaseUnitInfo first WP should be the same.");
			GameManager.Instance.TerminateGame();
		}


		[TestMethod]
		public void SimpleMovementPlane()
		{
			Player player = new Player();
			player.Name = "Plane movement test player";
			GameManager.Instance.CreateGame(player, "test game");
			BaseUnit unit = new BaseUnit();
			unit.UnitClass = new UnitClass("Plane", GameConstants.UnitType.FixedwingAircraft);
			unit.OwnerPlayer = player;
			unit.UnitClass.TurnRangeDegrSec = 20;
			unit.UnitClass.MaxAccelerationKphSec = 40;
			unit.UnitClass.MaxSpeedKph = 900;
			unit.UnitClass.MaxClimbrateMSec = 50;
			unit.UnitClass.MaxFallMSec = 100;
			unit.UnitClass.HighestOperatingHeightM = 10000;
			unit.UnitClass.MaxHitpoints = 100;
			unit.Position = new Position(60.0, 1.0, 50, 2);
			unit.UserDefinedElevation = GameConstants.HeightDepthPoints.MediumHeight;
			unit.SetActualSpeed(500);
			unit.Name = "F-Flop/B";
			Position dest = new Position(61.0, 2.0, 5000);
			unit.MovementOrder.AddWaypoint(new Waypoint(dest));
			unit.ReCalculateEta();
			TimeSpan OldEtaAllSec = unit.EtaAllWaypoints;
			unit.MovementOrder.AddWaypoint(new Waypoint(new Position(62.0, 2.0)));
			unit.ReCalculateEta();
			TimeSpan NewEtaAllSec = unit.EtaAllWaypoints;
			Assert.IsTrue(OldEtaAllSec != NewEtaAllSec, "Old and new ETA should not be the same after added waypoint.");
			unit.ActualSpeedKph = 400.0;
			int MaxIterations = 1000000;
			int Count = 0;
			do
			{
				unit.MoveToNewCoordinate(100.0);
				Count++;
			} while (MapHelper.CalculateDistanceM(unit.Position.Coordinate, dest.Coordinate) > 100
				&& Count < MaxIterations); //it has to be 100, since a plane will not stop at destinatation, but continue on minimum speed
			Assert.IsTrue(unit.Position.HeightOverSeaLevelM == 2000, "Height over sea level should be 2000 m.");
			Assert.IsFalse(Count >= MaxIterations, "Count should not go MaxIterations iterations.");
			Assert.AreNotEqual(unit.Position.Coordinate.LatitudeDeg, 60.0, "unit should not still be at latitude 60");
			Assert.AreNotEqual(unit.Position.Coordinate.LongitudeDeg, 1.0, "unit should not still be at longitude 1");
			Assert.IsNotNull(unit.Position.BearingDeg, "Bearing should not be null.");

			var oldWp = unit.GetActiveWaypoint().Clone();
			unit.MovementOrder.AddWaypointToTop(new Position(55, 1));
			var newWp = unit.GetActiveWaypoint().Clone();
			Assert.IsTrue(oldWp.Position.Coordinate.LatitudeDeg != newWp.Position.Coordinate.LatitudeDeg, "New wp should be activated");
			GameManager.Instance.TerminateGame();

		}

		[TestMethod]
		public void BearingCalculation()
		{
			Coordinate coor1 = new Coordinate(60.0, 1.0);
			Coordinate coor2 = new Coordinate(55.0, 5.0);
			double BearingDeg = MapHelper.CalculateBearingDegrees(coor1, coor2);
			Assert.AreNotEqual(BearingDeg, 0, "BearingDeg should not be 0.");
			GameConstants.DirectionCardinalPoints BearingPoints = BearingDeg.ToCardinalMark();
			Debug.WriteLine("** Bearing: " + BearingPoints.ToString());

			Coordinate coordBgo = Coordinate.ParseFromString("60 17 32 05 13 19"); //60°17′32″N05°13′19″E
			Coordinate c1 = Coordinate.ParseFromString("60 0 27 2 59 53");
			Coordinate c2 = Coordinate.ParseFromString("60 0 26 2 59 55");
			Coordinate c3 = Coordinate.ParseFromString("60 0 25 2 59 49"); //060° 00' 25"N, 002° 59' 49"
			
			Coordinate d1 = Coordinate.ParseFromString("70 0 0 3 0 0"); 

			double bearingc1d1 = MapHelper.CalculateBearingDegrees(c1, d1);
			double bearingc2d1 = MapHelper.CalculateBearingDegrees(c2, d1);
			double bearingc3d1 = MapHelper.CalculateBearingDegrees(c3, d1);
			double diff1 = bearingc1d1 - bearingc2d1;
			double diff2 = bearingc1d1 - bearingc3d1;
			double diff3 = bearingc2d1 - bearingc3d1;
			Assert.IsTrue(diff1 < 0.01 && diff2 < 0.01 && diff3 < 0.01, "Differences should be neglible.");
		}

		[TestMethod]
		public void AutomaticFormationOrderTest()
		{
			GameManager.Instance.Log.LogDebug("Running test AutomaticFormationOrderTest().");
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

			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(player, "test game");
			Position pos = new Position(60, 3, 0, 45);
			BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "lead", 
				pos, true);
			BaseUnit unit1 = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "follow1", 
				pos.Offset(30,300), true);
			BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "follow2", 
				pos.Offset(120,400), true);
			unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(61.0, 4.0)));
			unitMain.SetActualSpeed(40.0);
			Assert.IsTrue(unitMain.GetActiveWaypoint() != null, "ActiveWaypoint should not be null.");

			group.AutoAssignUnitsToFormation();

			game.IsNetworkEnabled = false;

			ScheduledOrder sched = new ScheduledOrder(1);
			var innerOrder = new BaseOrder(unit1.OwnerPlayer, GameConstants.OrderType.SetSpeed);
			innerOrder.UnitSpeedType = GameConstants.UnitSpeedType.Slow;
			sched.Orders.Add(innerOrder);
			unit1.Orders.Enqueue(sched);
			GameManager.Instance.Game.RunGameInSec = 1;
			GameManager.Instance.Game.StartGamePlay();
			unitMain.ReCalculateEta();
			unit1.ReCalculateEta();
			unit2.ReCalculateEta();

			Assert.IsTrue(unit1.MovementOrder is MovementFormationOrder, "Unit 1 should have a movement formation order.");
			Assert.IsTrue(unit2.MovementOrder is MovementFormationOrder, "Unit 2 should have a movement formation order.");
			Assert.IsFalse(unitMain.MovementOrder is MovementFormationOrder, " MainUnit should NOT have movement formation order.");


			unitMain.HitPoints = 0;
			unitMain.IsMarkedForDeletion = true;
			group.AutoAssignUnitsToFormation();
			//GameManager.Instance.Game.RunGameInSec = 1;
			//GameManager.Instance.Game.StartGamePlay();

			Assert.IsTrue(group.MainUnit.Id == unit1.Id, "Unit 1 should now be main unit.");
			Assert.IsFalse(unit1.MovementOrder is MovementFormationOrder, "Unit 1 should NOT have a movement formation order.");
			Assert.IsTrue(unit2.MovementOrder is MovementFormationOrder, "Unit 2 should still have a movement formation order.");
			var activeWp = unit1.GetActiveWaypoint();
			Assert.IsTrue(activeWp != null, "Unit 1 should have an ActiveWaypoint.");


			GameManager.Instance.TerminateGame();

		}

        [TestMethod]
        public void PlaneMovementOverTerrainTest()
        {
            GameManager.Instance.Log.LogDebug("Running test PlaneMovementOverTerrainTest().");
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            Player playerEnemy = new Player();
            playerEnemy.Name = "BadGuy";
            playerEnemy.IsComputerPlayer = true;

            //Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.Players.Add(player);
            game.Players.Add(playerEnemy);

            GameManager.Instance.Game.SetAllPlayersEnemies();

            GameManager.Instance.GameData.InitAllData();
            GameManager.Instance.CreateGame(player, "test game");
            Position pos = new Position(61, 5.7, 100, 45);
            BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, null, "f22", "lead",
                pos, true);
            unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(64.2, 9.4)));
            unitMain.SetActualSpeed(850);
            unitMain.UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
            GameManager.Instance.Game.GameWorldTimeSec = 10;
            unitMain.MoveToNewPosition3D(1);
            var maxHeightAheadM = MapHelper.GetHighestValue(unitMain.TerrainHeightAtPosM, unitMain.TerrainHeight10SecForwardM, unitMain.TerrainHeight30SecForwardM);
            Assert.IsTrue(unitMain.TerrainHeightAtPosM != 0, "Terrain height should not be 0.");
            Assert.IsTrue(unitMain.Position.HeightOverSeaLevelM > unitMain.TerrainHeightAtPosM, "Unit should be above terrain.");

            unitMain.UserDefinedElevation = GameConstants.HeightDepthPoints.MediumHeight;
            unitMain.MoveToNewPosition3D(1);
            Assert.IsTrue(unitMain.Position.HeightOverSeaLevelM > unitMain.TerrainHeightAtPosM, "Unit should be above terrain.");
            unitMain.UserDefinedElevation = GameConstants.HeightDepthPoints.Low;

            for (int i = 0; i < 1000; i++)
            {
                unitMain.MoveToNewPosition3D(1);
                Assert.IsTrue(unitMain.Position.HeightOverSeaLevelM > unitMain.TerrainHeightAtPosM, "Unit should be above terrain.");
            }
        }

		[TestMethod]
		public void UnitFormationMovement()
		{
            GameManager.Instance.Log.LogDebug("Running test UnitFormationMovement().");
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

			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(player, "test game");
			Position pos = new Position(60, 3, 0, 45);
			BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "lead",
				pos, true);
			BaseUnit unit1 = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "follow1",
				pos.Offset(30, 300), true);
			BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "follow2",
				pos.Offset(120, 400), true);
			unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(50.0, 3.0)));
			unitMain.SetActualSpeed(unitMain.GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType.Cruise));
			Assert.IsTrue(unitMain.GetActiveWaypoint() != null, "ActiveWaypoint should not be null.");

			group.AutoAssignUnitsToFormation();

			game.IsNetworkEnabled = false;
			long counter = 0;
			long maxInterations = 1000;
			long leaveAtCounter = maxInterations;
			bool leaveLoop = false;
			do 
			{
				unitMain.MoveToNewCoordinate(1000);
				unit1.MoveToNewCoordinate(1000);
				unit2.MoveToNewCoordinate(1000);
				//unitMain.CheckIfGroupIsStaging();
				//unit1.CheckIfGroupIsStaging();
				//unit2.CheckIfGroupIsStaging();
				if (counter==10)
				{
					Assert.IsTrue(unit1.IsAtFormationPosition, "10: Unit 1 should be at formation position");
					Assert.IsTrue(unit2.IsAtFormationPosition, "10: Unit 2 should be at formation position");
				}
				if (counter == 11)
				{
					unitMain.MovementOrder.ClearAllWaypoints();
					unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(70, 3)));
				}
				if (counter == 12)
				{
					Assert.IsNotNull(unitMain.GetActiveWaypoint(), "Main should have ActiveWaypoint");
					Assert.IsTrue(group.IsStaging, "Group should be Staging");
					//Assert.IsFalse(unit1.IsAtFormationPosition, "12: Unit 1 should NOT be at formation position");
					//Assert.IsFalse(unit2.IsAtFormationPosition, "12: Unit 2 should NOT be at formation position");

				}
				if ((counter > 13 && unit1.IsAtFormationPosition && unit2.IsAtFormationPosition && !leaveLoop))
				{
					//unitMain.MoveToNewCoordinate(1000);
					leaveLoop = true;
					leaveAtCounter = counter + 10;
				}
				counter++;
			} while (counter < maxInterations && counter < leaveAtCounter);
			double speed = unitMain.GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType.Cruise);
			group.CheckIfGroupIsStaging();
			unitMain.ResolveUnitAndGroupSpeed();
			unit1.ResolveUnitAndGroupSpeed();
			unit2.ResolveUnitAndGroupSpeed();
			unitMain.UpdateActualSpeed(10);
			unit1.UpdateActualSpeed(10);
			unit2.UpdateActualSpeed(10);
			group.CheckIfGroupIsStaging();
			Assert.IsTrue(unitMain.ActualSpeedKph == speed, "Unit Main Should be at cruise speed");
			Assert.IsTrue(unit1.ActualSpeedKph == speed, "Unit 1 Should be at cruise speed");
			Assert.IsTrue(unit2.ActualSpeedKph == speed, "Unit 2 Should be at cruise speed");
			Assert.IsTrue(counter < maxInterations,"Should not use all iterations to get to formation positions");
			GameManager.Instance.TerminateGame();

		}

		[TestMethod]
		public void TestCreateAircraftLaunchOrderEngage()
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
			var nextDoorPos = pos.Offset(75, 10000);
			BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, null, "nimitz", "Nimitz", pos, true);
			unitMain.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);

			BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "boeing767", "", nextDoorPos, true);
			Assert.IsNotNull(unitEnemy, "Enemy is not null");
			unitEnemy.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);
			game.IsGamePlayStarted = true; //lying!
			unitMain.SensorSweep();
			unitEnemy.SensorSweep();
			Assert.IsTrue(player.DetectedUnits.Count > 0, "Player should have detected an enemy.");
			var targetForAttack = player.DetectedUnits[0];
			var unitToLaunch = unitMain.AircraftHangar.Aircraft.First<AircraftUnit>(u => u.SupportsRole(GameConstants.Role.AttackAir) && u.IsReady);
			Assert.IsNotNull(unitToLaunch, "UnitToLaunch should not be null");
			var airList = new List<string>();
			airList.Add(unitToLaunch.Id);
			var unitOrderClient = OrderFactory.CreateAircraftLaunchOrder(unitMain.Id, airList, targetForAttack.GetDetectedUnitInfo());
			var baseOrder = game.GetBaseOrderFromUnitOrder(unitOrderClient);
			Assert.IsTrue(baseOrder.OrderType == GameConstants.OrderType.LaunchOrder, "BaseOrder type should be LaunchOrder");
			var engageOrder = baseOrder.Orders[0] as EngagementOrder;
			Assert.IsNotNull(engageOrder, "EngagementOrder in Launchorder should not be null");
			Assert.IsTrue(engageOrder.TargetDetectedUnit.Id == targetForAttack.Id, "EngagementOrder should have target set to right enemy");
			player.HandleMessageFromClient(unitOrderClient);
			unitMain.ExecuteOrders();
			Assert.IsNotNull(unitToLaunch.Position, "Unit to launch should have non null position");
			Assert.IsNotNull(unitToLaunch.TargetDetectedUnit, "Launched unit should have target detected unit.");
		}

		[TestMethod]
		public void LaunchMultipleAircraftToEngageTarget()
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
			var nextDoorPos = pos.Offset(75, 10000);
			nextDoorPos.HeightOverSeaLevelM = 0;

			var nextDoorPos2 = pos.Offset(75, 10050);
			nextDoorPos2.HeightOverSeaLevelM = 0;
			Group enemyGroup = new Group();
			BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, null, "nimitz", "Nimitz", pos, true);
			unitMain.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);

			BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, enemyGroup, "pamela", "Pamela 1", nextDoorPos, true);
			BaseUnit unitEnemy2 = GameManager.Instance.GameData.CreateUnit(playerEnemy, enemyGroup, "pamela", "Pamela 2", nextDoorPos2, true);

			Assert.IsNotNull(unitEnemy, "Enemy is not null");
			Assert.IsTrue(enemyGroup.Units.Count == 2, "Enemy group should have two units.");

			unitEnemy.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);
			game.IsGamePlayStarted = true; //lying!
			unitMain.SensorSweep();
			unitEnemy.SensorSweep();
			Assert.IsTrue(player.DetectedUnits.Count > 0, "Player should have detected an enemy.");
			var targetForAttack = player.DetectedUnits[0];
			var unitsToLaunch = unitMain.AircraftHangar.Aircraft.Where<AircraftUnit>(u => u.SupportsRole(GameConstants.Role.AttackSurfaceStandoff) && u.IsReady);

			int launchCount = 2;
			int idx = 0;
			Assert.IsNotNull(unitsToLaunch, "UnitToLaunch should not be null");
			Assert.IsTrue(unitsToLaunch.Count<AircraftUnit>() > 2, "More that two units should be found to launch");
			var airList = new List<string>();

			foreach (var aircraft in unitsToLaunch)
			{
				airList.Add(aircraft.Id);
				idx++;
				if (idx >= launchCount)
				{
					break;
				}
			}
			var airCraftList = new List<BaseUnit>();
			foreach (var air in airList)
			{
				var plane = player.GetUnitById(air);
				airCraftList.Add(plane);
			}
			Assert.IsTrue(airList.Count == launchCount, "The right unit count should be picked for launch");
			var unitOrderClient = OrderFactory.CreateAircraftLaunchOrder(unitMain.Id, airList, targetForAttack.GetDetectedUnitInfo());
			var baseOrder = game.GetBaseOrderFromUnitOrder(unitOrderClient);
			Assert.IsTrue(baseOrder.OrderType == GameConstants.OrderType.LaunchOrder, "BaseOrder type should be LaunchOrder");
			var engageOrder = baseOrder.Orders[0] as EngagementOrder;
			Assert.IsNotNull(engageOrder, "EngagementOrder in LaunchOrder should not be null");
			Assert.IsTrue(engageOrder.TargetDetectedUnit.Id == targetForAttack.Id, "EngagementOrder should have target set to right enemy");
			player.HandleMessageFromClient(unitOrderClient);
			unitMain.ExecuteOrders();
			var firstPlane = airCraftList[0];
			var secondPlane = airCraftList[1];
			Assert.IsNotNull(firstPlane.Position, "First plane should have non null position");
			Assert.IsNotNull(firstPlane.TargetDetectedUnit, "First Plane should have target detected unit.");
			Assert.IsNotNull(secondPlane.Position, "Second Plane should have non null position");
			Assert.IsNotNull(secondPlane.TargetDetectedUnit, "Second Plane should have target detected unit.");



		}


		[TestMethod]
		public void AircraftReturnToBaseTest()
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

			Position pos = new Position(60, 3, 0, 120);
			var nextDoorPos = pos.Offset(75, 10000);
			BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, null, "nimitz", "Nimitz", pos, true);
			unitMain.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);

			//BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, null, "boeing767", "", nextDoorPos, true);
			//Assert.IsNotNull(unitEnemy, "Enemy is not null");
			//unitEnemy.SetSensorsActivePassive(TTG.NavalWar.NWComms.GameConstants.SensorType.Radar, true);
			game.IsGamePlayStarted = true; //lying!
			unitMain.SensorSweep();
			//unitEnemy.SensorSweep();
			var unitToLaunch = unitMain.AircraftHangar.Aircraft.First<AircraftUnit>(u => u.SupportsRole(GameConstants.Role.AttackAir) && u.IsReady);
			Assert.IsNotNull(unitToLaunch, "UnitToLaunch should not be null");
			var airList = new List<string>();
			airList.Add(unitToLaunch.Id);
			var destPos = pos.Offset(75, 10000);
			var moveOrder = new UnitMovementOrder();
			moveOrder.Id = unitToLaunch.Id;
			moveOrder.Position = destPos.GetPositionInfo();
			var unitOrderClient = OrderFactory.CreateAircraftLaunchOrder(unitMain.Id, airList, moveOrder);
			//var unitOrderClient = OrderFactory.CreateAircraftLaunchOrder(unitMain.Id, airList, targetForAttack.GetDetectedUnitInfo());
			var baseOrder = game.GetBaseOrderFromUnitOrder(unitOrderClient);
			Assert.IsTrue(baseOrder.OrderType == GameConstants.OrderType.LaunchOrder, "BaseOrder type should be LaunchOrder");
			player.HandleMessageFromClient(unitOrderClient);
			unitMain.ExecuteOrders();
			Assert.IsNotNull(unitToLaunch.Position, "Unit to launch should have non null position");
			unitMain.Tick(10);
			unitToLaunch.Position = destPos.Clone(); //teleport it
			unitToLaunch.FuelDistanceCoveredSinceRefuelM = unitToLaunch.MaxRangeCruiseM - 10000; // unitToLaunch.FuelDistanceRemainingM;
			unitToLaunch.MoveToNewCoordinate(100);
			Assert.IsTrue(unitToLaunch.IsOrderedToReturnToBase, "Launched unit should have been ordered to return to base.");
			var distM = MapHelper.CalculateDistance3DM(unitMain.Position, unitToLaunch.MovementOrder.GetActiveWaypoint().Position);
			Assert.IsTrue(distM < 1, "Launched unit current waypoint should be carrier position");
			var iterations = 0;
			do
			{
				var distanceToCarrierM = MapHelper.CalculateDistance3DM(unitMain.Position, unitToLaunch.Position);
				unitMain.Tick(1);
				unitToLaunch.Tick(1);
				iterations++;
			} while (unitToLaunch.CarriedByUnit == null && iterations < 100000);

			Assert.IsTrue(iterations < 100000, "No more than 100000 iterations should have passed.");

		}




		[TestMethod]
		public void FormationOrder()
		{
			GameManager.Instance.Log.LogDebug("Running test FormationOrder().");
			Player player = new Player();
			Group group = new Group();
			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(player, "test game");
			Position pos = new Position(60, 3, 0, 45);
			BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "", pos, true);
			unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(61.0, 4.0)));
			unitMain.ActualSpeedKph = 40.0;
			Assert.IsTrue(unitMain.GetActiveWaypoint() != null, "ActiveWaypoint should not be null.");

			unitMain.MoveToNewCoordinate(1.0);

			Assert.IsTrue(unitMain.GetActiveWaypoint() != null, "ActiveWaypoint should not be null after movement.");
			pos = new Position(61, 4, 0, 40);
			BaseUnit unitFollow = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "", pos, true);

			MovementFormationOrder order = new MovementFormationOrder(unitMain, 5000, -1000, 0);
			Waypoint waypoint = order.GetActiveWaypoint();
			unitFollow.Position = waypoint.Position.Clone();
			double distanceM = MapHelper.CalculateDistanceM(unitMain.Position.Coordinate, unitFollow.Position.Coordinate);
			Assert.IsTrue(distanceM > 5000 && distanceM < 5100, "Distance should be between 5000 and 5100 M.");
			Assert.IsTrue(Math.Abs((double)unitFollow.Position.BearingDeg - 45) < 1, "UnitFollow bearing should be 45 deg");
			GameManager.Instance.Log.LogDebug("*** Offset right:" + order.PositionOffset.RightM + "  Offset forward:" + order.PositionOffset.ForwardM);
			GameManager.Instance.Log.LogDebug("Main unit pos: " + unitMain.Position.ToString());
			GameManager.Instance.Log.LogDebug("Calculated pos: " + waypoint.Position.ToString());
			GameManager.Instance.TerminateGame();

		}

		[TestMethod]
		public void ParseCoordinateTest()
		{
			Coordinate coordFlesland = Coordinate.ParseFromString("60 17 32 5 13 19"); //60°17′32″N05°13′19″E is Flesland
			Coordinate coordWikiExample = Coordinate.ParseFromString("60 17 32 -87 43 41"); // W87°43'41" is -87.728056.

			Coordinate coordEx3 = Coordinate.ParseFromString("-30 5 35 -91 3 32");

			Assert.IsTrue(Math.Abs(coordWikiExample.LongitudeDeg + 87.728056) < 0.01, "Coordinate longitude should be as specified.");
		}


		//[TestMethod]
		public void IntersectCalculation()
		{
			//TODO: ADD Conditions for test result. 
			Player p = new Player();
			Group group = new Group();
			GameManager.Instance.GameData.InitAllData();
			GameManager.Instance.CreateGame(p, "TestGame");
			Position tpos = new Position(60,3.47991943359375, 0, 0);
			Position ipos = new Position(60,4,0,0);
			BaseUnit target = GameManager.Instance.GameData.CreateUnit(p, group, "arleighburke", "TARGET", tpos, true);
			target.Position.BearingDeg = 0;
			target.ActualSpeedKph = 20;
			BaseUnit intercepter = GameManager.Instance.GameData.CreateUnit(p, group, "arleighburke", "Rocket", ipos, true);
			intercepter.ActualSpeedKph = 25;
			Intercept intercept = new Intercept();
			double targetX = MapHelper.LongitudeToXvalue(target.Position.Coordinate, 9);
			double targetY = MapHelper.LatitudeToYvalue(target.Position.Coordinate, 9);
			double intercepterX = MapHelper.LongitudeToXvalue(intercepter.Position.Coordinate, 9);
			double intercepterY = MapHelper.LatitudeToYvalue(intercepter.Position.Coordinate, 9);

			intercept.calculate(intercepterX, intercepterY, targetX, targetY, 20, 33, 100, 0);
			Coordinate c = intercept.impactPoint;


		}
	}
}
