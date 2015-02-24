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

namespace NWManagerTest
{
	/// <summary>
	/// Summary description for ScenarioTest
	/// </summary>
	[TestClass]
	public class ScenarioTest
	{
		public ScenarioTest()
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
		public void LoadTestScenario()
		{
			Player player = new Player();
			player.Name = "GoodGuy";
			player.IsComputerPlayer = true;
			GameManager.Instance.GameData.InitAllData();
			Game game = GameManager.Instance.CreateGame(player, "test game");
			GameManager.Instance.GameData.LoadGameScenario("test", game);
			Player otherPlayer = player.Enemies[0];
			Assert.IsTrue(game.Players.Count == 2, "There should be two players in game.");
			//Assert.IsTrue(otherPlayer.Units.Count == 15, "Player 1 should have 15 units.");
			BaseUnit unit = player.GetUnitById("tag:main");
			Assert.IsNotNull(unit, "Unit with tag main should exist."); 
			var wp = unit.MovementOrder.GetActiveWaypoint();
			Assert.IsNotNull(wp, "Main unit waypoint should not be null.");
			BaseUnitInfo info = unit.GetBaseUnitInfo();
			Assert.IsTrue(info.IsGroupMainUnit, "Main unit should be main unit");
		}

        [TestMethod]
        public void EventTriggerTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true;
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            GameManager.Instance.GameData.LoadGameScenario("test", game);
            Player otherPlayer = player.Enemies[0];
            Assert.IsTrue(game.Players.Count == 2, "There should be two players in game.");

            var eventTrigger = new EventTrigger() { EventTriggerType = GameConstants.EventTriggerType.TimeHasElapsed, TimeElapsedSec = 10, };
            eventTrigger.InnerTriggers = new List<EventTrigger>() 
            { 
                new EventTrigger()
                {
                    EventTriggerType = GameConstants.EventTriggerType.TimeHasElapsed,
                    TimeElapsedSec = 10,
                    Tag = "new",
                },
            };
            player.EventTriggers.Add(eventTrigger);
            bool triggerResult = player.IsEventTriggerConditionMet(eventTrigger);
            Assert.IsFalse(triggerResult, "Event trigger should not be triggered now.");

            game.GameWorldTimeSec = 10.1;

            triggerResult = player.IsEventTriggerConditionMet(eventTrigger);
            
            Assert.IsTrue(triggerResult, "Event trigger should now be triggered.");
            
            Assert.IsTrue(player.EventTriggers.Count > 0, "Player should have event triggers");

            game.IsGameLoopStarted = true;
            game.IsGamePlayStarted = true;

            player.ExecuteEventTriggers();
            var nEventTriggerTime = player.EventTriggers.FirstOrDefault<EventTrigger>(t => t.EventTriggerType == GameConstants.EventTriggerType.TimeHasElapsed && t.Tag == "new");
            Assert.IsNotNull(nEventTriggerTime, "New EventTrigger should be found");


        }


		[TestMethod]
		public void AircraftLaunchAndMovementTest()
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
			game.IsGamePlayStarted = true; //a lie
			var airfield = player.Units.FirstOrDefault<BaseUnit>(u => u.UnitClass.IsLandbased);
			Assert.IsNotNull(airfield, "Airfield should be found.");
			List<AircraftUnit> airList = new List<AircraftUnit>();
			int maxCount = 2;
			int count = 0;
			foreach (var craft in airfield.AircraftHangar.Aircraft)
			{
				if (craft.UnitClass.Id == "f22")
				{
					airList.Add(craft);
				}
				count++;
				if (count>=maxCount)
				{
					break;
				}
			}
			var coord = new Coordinate(80, -3);
			var orders = new List<BaseOrder>();
			MovementOrder moveOrder = new MovementOrder(new Waypoint(coord));
			orders.Add(moveOrder);
			var result = airfield.LaunchAircraft(airList, string.Empty, string.Empty, orders, string.Empty);
			var group = player.Groups.FirstOrDefault<Group>(g => g.MainUnit.UnitClass.Id == "f22");
			Assert.IsNotNull(group, "F22 group should not be null");
			airfield.AircraftHangar.ReadyInSec = 0; //cheat;
			airfield.ExecuteOrders();
			game.RealTimeCompressionFactor = 1.0;
			var unitMain = group.MainUnit;
			var unitFollowing = group.Units[1];
			Assert.IsNotNull(unitFollowing, "Following F22 should not be null");
			int MaxIterations = 1000000;
			int Count = 0;
			//double DistanceToTargetM = 0;
			double[] listDistancesFollow = new double[MaxIterations];
			double[] listDistancesMain = new double[MaxIterations];
			double[] listDistancesBetween = new double[MaxIterations];
			var previousPositionFollow = unitFollowing.Position.Clone();
			var previousPositionMain = unitMain.Position.Clone();

			double distanceToFinalTargetM = double.MaxValue;
			do
			{
				unitFollowing.MoveToNewCoordinate(1000.0);
				unitMain.MoveToNewCoordinate(1000.0);
				//DistanceToTargetM = MapHelper.CalculateDistanceMeters(unit.Position.Coordinate, dest.Coordinate);
				if (Count > 0)
				{
					if (unitFollowing.Position == null || unitMain.Position == null)
					{
						distanceToFinalTargetM = 0;
						continue;
					}
					listDistancesFollow[Count] = MapHelper.CalculateDistanceM(unitFollowing.Position.Coordinate, previousPositionFollow.Coordinate);
					listDistancesMain[Count] = MapHelper.CalculateDistanceM(unitMain.Position.Coordinate, previousPositionMain.Coordinate);
					listDistancesBetween[Count] = MapHelper.CalculateDistanceM(unitMain.Position.Coordinate, unitFollowing.Position.Coordinate);
					var lastDistanceM = listDistancesFollow[Count];
					Assert.IsTrue(listDistancesFollow[Count] < 2000, "Follow Unit should never move faster than 250 m/sec. Moved " 
						+ listDistancesFollow[Count] + "m at count=" + Count); //~235 m/sec
					Assert.IsTrue(listDistancesMain[Count] < 2000, "Main Unit should never move faster than 250 m/sec. Moved " + listDistancesFollow[Count] + "m"); //~235 m/sec

				}
				previousPositionFollow = unitFollowing.Position.Clone();
				previousPositionMain = unitMain.Position.Clone();
				distanceToFinalTargetM = MapHelper.CalculateDistanceM(unitMain.Position.Coordinate, coord);
				Count++;

			} while (distanceToFinalTargetM > 10 && Count < MaxIterations);

		}

		[TestMethod]
		public void CheckWeaponStores()
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
			game.IsGamePlayStarted = true; //cheat
			var nimitz = player.GetUnitById("tag:main");
            string nsmcName = "jsm";
			string navalStrikeName = "Naval strike";
			string airSupName = "Air superiority";
			//new WeaponStoreEntry("agm84harpoon", 30),
			//new WeaponStoreEntry("agm84harpoonslam", 30),
			var entry = nimitz.GetWeaponStoreEntry(nsmcName);
            Assert.IsNotNull(entry, "The Nimitz should have a store entry for jsm");
			var countNsmBefore = entry.Count;
            Assert.IsTrue(countNsmBefore > 0, "The jsm count should be > 0");
			var f35 = nimitz.AircraftHangar.Aircraft.FirstOrDefault(a => a.UnitClass.Id == "f35c" && a.CurrentWeaponLoadName=="Air superiority");
			Assert.IsNotNull(f35, "Should be an F35c on carrier");
			var StrikeWpnLoad = GameManager.Instance.GameData.GetWeaponLoadByName(f35.UnitClass.Id, navalStrikeName);

			bool CanChange = f35.CanChangeToWeaponLoad(StrikeWpnLoad, false);
			Assert.IsTrue(CanChange, "F35 should be able to change to Naval Strike");
			f35.SetWeaponLoad(navalStrikeName);
			var countNsmAfter = entry.Count;
            Assert.IsTrue(countNsmAfter < countNsmBefore, "Count of nsm missiles in store should be lower after WpnLoad change.");
			f35.SetWeaponLoad(airSupName);
			Assert.IsTrue(entry.Count == countNsmBefore, "Count of nsm missiles in store should now be same as before change.");
			entry.Count = 1; //requires 2
			CanChange = f35.CanChangeToWeaponLoad(StrikeWpnLoad, false);
			Assert.IsFalse(CanChange, "F35 should NOT be able to change to Naval Strike after store is reduced");
			f35.SetWeaponLoad(navalStrikeName);
			Assert.IsTrue(f35.CurrentWeaponLoadName == airSupName, "F35 should still be set to air superiority");
			var weaponClassNsm = GameManager.Instance.GetWeaponClassById(nsmcName);
			player.Credits = weaponClassNsm.AcquisitionCostAmmoCredits * 3;
			var acqOrder = OrderFactory.CreateAcquireAmmoOrder(nimitz.Id, weaponClassNsm.Id, 3);
			
			var acqResult = player.AcquireMoreAmmo(acqOrder);
			Assert.IsTrue(entry.Count == 4, "There should be 5 nsm's in store now.");
			Assert.IsTrue(acqResult, "Ammo acquisition should succeed.");
			acqOrder = OrderFactory.CreateAcquireAmmoOrder(nimitz.Id, weaponClassNsm.Id, 3);
			acqResult = player.AcquireMoreAmmo(acqOrder);
			Assert.IsFalse(acqResult, "Ammo acquisition should fail (not enough credits).");


		}

		[TestMethod]
		public void SplitAndJoinGroups()
		{
			Player player = new Player();
			player.Name = "GoodGuy";
			player.IsComputerPlayer = true;
			GameManager.Instance.GameData.InitAllData();
			Game game = GameManager.Instance.CreateGame(player, "test game");
			GameManager.Instance.GameData.LoadGameScenario("test", game);
			Group group = new Group();
			BaseUnit unitMain = player.GetUnitById("tag:main");
			Assert.IsNotNull(unitMain, "Unit should not be null");
			BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "Bulke",
				unitMain.Position.Offset(new PositionOffset(3000, 3000)), true);
			BaseUnit unit3 = GameManager.Instance.GameData.CreateUnit(player, group, "arleighburke", "Brande",
				unitMain.Position.Offset(new PositionOffset(3000, 2800)), true);
			unit2.SetActualSpeed(20);
			unit3.SetActualSpeed(20);
			group.AutoAssignUnitsToFormation();
			
            //TEST: Some units in a group should be joined into an existing group.
			
            Group groupSag = player.GetGroupById(unitMain.GroupId);
			Assert.IsNotNull(groupSag, "groupSag should not be null");
			UnitOrder order = new UnitOrder(TTG.NavalWar.NWComms.GameConstants.UnitOrderType.JoinGroups, unit2.Id);
			order.SecondId = groupSag.Id;
			player.HandleMessageFromClient(order);
			//unit2.JoinNewGroupAllUnits(groupSag.Id);
			Assert.IsTrue(unit2.Orders.Count > 0, "There should be at least 1 unit orders for Unit2.");
			unit2.ExecuteOrders();
			Assert.IsTrue(unit2.GroupId == groupSag.Id, "Unit2 should be member of new group.");

            //TEST: Split some members of a group out into a separate, new group
			var unitList = new List<string>();
			unitList.Add(unit2.Id);
			unitList.Add(unit3.Id);
			groupSag.SplitGroup(unitList);
			Assert.IsTrue(unit2.GroupId != groupSag.Id, "Unit2 should no longer be member of GroupSag.");
            var newGroup = player.GetGroupById(unit2.GroupId);
            Assert.IsNotNull(newGroup,"New group should not be null.");

            var joinUnitOrder = OrderFactory.CreateJoinGroupOrder(groupSag.Id, unit2.Id, unitList);
            var joinBaseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(joinUnitOrder);

            Assert.IsNotNull(joinBaseOrder, "Join BaseOrder should not be null.");
            Assert.IsTrue(joinBaseOrder.SecondId == groupSag.Id, "Group sag should be the group to join");
            unit2.JoinNewGroup(joinBaseOrder);

            Assert.IsTrue(unit2.GroupId == groupSag.Id, "Unit2 should again be a member of GroupSag.");
            Assert.IsTrue(unit3.GroupId == groupSag.Id, "Unit3 should again be a member of GroupSag.");

            //TEST: Units that are not members of any group should join an existing group

            groupSag.RemoveUnit(unit2);
            Assert.IsTrue(string.IsNullOrEmpty(unit2.GroupId), "Unit2 should no longer be member of ANY group.");

            unitList = new List<string>();
            unitList.Add(unit2.Id);
            joinUnitOrder = OrderFactory.CreateJoinGroupOrder(groupSag.Id, unit2.Id, unitList);
            joinBaseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(joinUnitOrder);
            Assert.IsNotNull(joinBaseOrder, "Join BaseOrder should not be null.");
            unit2.JoinNewGroup(joinBaseOrder);
            Assert.IsTrue(unit2.GroupId == groupSag.Id, "Unit2 should again be a member of GroupSag.");

            //TEST: Units that are not members of any group should join a NEW group

            groupSag.RemoveUnit(unit2);
            Assert.IsTrue(string.IsNullOrEmpty(unit2.GroupId), "Unit2 should no longer be member of ANY group.");

            unitList = new List<string>();
            unitList.Add(unit2.Id);

            joinUnitOrder = OrderFactory.CreateJoinGroupOrder(string.Empty, unit2.Id, unitList);
            joinBaseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(joinUnitOrder);
            Assert.IsNotNull(joinBaseOrder, "Join BaseOrder should not be null.");
            unit2.JoinNewGroup(joinBaseOrder);

            Assert.IsTrue(!string.IsNullOrEmpty(unit2.GroupId), "Unit2 should now be a member of NEW group.");
            Assert.IsTrue(unit2.GroupId != groupSag.Id, "Unit2 should no longer be member of GroupSag.");


		}

        [TestMethod]
        public void SkillLevelTest()
        {
            Player player = new Player();
            player.Name = "GoodGuy";
            player.IsComputerPlayer = true; 
            GameManager.Instance.GameData.InitAllData();
            Game game = GameManager.Instance.CreateGame(player, "test game");
            game.SkillLevel = 1;
            player.IsComputerPlayer = false; //eek
            GameManager.Instance.GameData.LoadGameScenario("gamescom2011large", game);
            Player playerRussia = player.Enemies[0];
            BaseUnit Airport = player.GetUnitById("tag:Keflavik");
            Assert.IsNotNull(Airport, "Airport should not be null.");
            var f22units = Airport.AircraftHangar.Aircraft.Where<AircraftUnit>(a => a.UnitClass.Id == "f22");
            int countF22 = f22units.Count<BaseUnit>();
            Assert.IsTrue(countF22 == 4, "There should be 4 F22s on Keflavik on easy.");

            game.SkillLevel = 1;
            playerRussia.IsComputerPlayer = true;

            bool isHumanIncreaseLevel = game.IsUnitIncludedForPlayer(player, GameConstants.SkillLevelInclusion.IncludeToIncreasedLevel);
            bool isAiIncreasedLevel = game.IsUnitIncludedForPlayer(playerRussia, GameConstants.SkillLevelInclusion.IncludeToIncreasedLevel);
            bool isHumanDecreaseLevel = game.IsUnitIncludedForPlayer(player, GameConstants.SkillLevelInclusion.IncludeToReducedLevel);
            bool isAiDecreaseLevel = game.IsUnitIncludedForPlayer(playerRussia, GameConstants.SkillLevelInclusion.IncludeToReducedLevel);
            
            Assert.IsTrue(isHumanIncreaseLevel == false && isAiIncreasedLevel == false, "Level 1, increase level: none included.");
            Assert.IsTrue(isHumanDecreaseLevel, "Level 1, reduce level, human should be included.");
            Assert.IsTrue(isAiDecreaseLevel == false, "Level 1, reduce level, AI should NOT be included.");

            game.SkillLevel = 3;
            isHumanIncreaseLevel = game.IsUnitIncludedForPlayer(player, GameConstants.SkillLevelInclusion.IncludeToIncreasedLevel);
            isAiIncreasedLevel = game.IsUnitIncludedForPlayer(playerRussia, GameConstants.SkillLevelInclusion.IncludeToIncreasedLevel);
            isHumanDecreaseLevel = game.IsUnitIncludedForPlayer(player, GameConstants.SkillLevelInclusion.IncludeToReducedLevel);
            isAiDecreaseLevel = game.IsUnitIncludedForPlayer(playerRussia, GameConstants.SkillLevelInclusion.IncludeToReducedLevel);

            Assert.IsTrue(isHumanIncreaseLevel == false, "Level 3, increase level, human: not included.");
            Assert.IsTrue(isAiIncreasedLevel, "Level 3, increase level, AI: included.");
            Assert.IsTrue(isHumanDecreaseLevel == false, "Level 3, reduce level, human should NOT be included.");
            Assert.IsTrue(isAiDecreaseLevel == false, "Level 3, reduce level, AI should NOT be included.");

            

        }

		[TestMethod]
		public void ChangePlaneLoadoutTest()
		{
			Player player = new Player();
			player.Name = "GoodGuy";
			player.IsComputerPlayer = true;
			GameManager.Instance.GameData.InitAllData();
			Game game = GameManager.Instance.CreateGame(player, "test game");
			GameManager.Instance.GameData.LoadGameScenario("test", game);
			Player otherPlayer = player.Enemies[0];
			BaseUnit Airport = player.Units.Find(u => u.UnitClass.Id == "ukairportlarge");
			Assert.IsNotNull(Airport, "Airport should not be null.");
			AircraftUnit f22unit = (AircraftUnit)Airport.AircraftHangar.Aircraft.First<AircraftUnit>(a => a.UnitClass.Id == "f22");
			game.IsGamePlayStarted = true; //a lie
			string desiredWeaponLoad = "Naval Strike";
			f22unit.SetDirty(TTG.NavalWar.NWComms.GameConstants.DirtyStatus.Clean);
			Airport.SetDirty(TTG.NavalWar.NWComms.GameConstants.DirtyStatus.Clean);
			BaseUnitInfo info = Airport.GetBaseUnitInfo();
			f22unit.SetWeaponLoad(desiredWeaponLoad);
			info = Airport.GetBaseUnitInfo();
			Assert.IsTrue(f22unit.ReadyInSec > 500, "F22 should not be ready after loadout change.");
			Assert.IsTrue(f22unit.CurrentWeaponLoadName == desiredWeaponLoad, "Current loadout name should be 'Naval Strike'");
			bool launchResult = Airport.LaunchAircraft(new List<AircraftUnit> { f22unit }, string.Empty, "No planes", null, "");
			
			Assert.IsFalse(launchResult , "Air group should not be launched.");
			info = Airport.GetBaseUnitInfo();
		}

		[TestMethod]
		public void TestUnitDatabaseIntegrity()
		{
			Player player = new Player();
			player.Name = "GoodGuy";
			player.IsComputerPlayer = true;
			GameManager.Instance.GameData.InitAllData();
			foreach (var uc in GameManager.Instance.GameData.UnitClasses)
			{
				foreach (var carried in uc.CarriedUnitClassses)
				{
					var unitClass = GameManager.Instance.GetUnitClassById(carried.UnitClassId);
					Assert.IsNotNull(unitClass, string.Format("UnitClass {0}, Carried Unitclass '{1}' not found",
						uc.UnitClassShortName, carried.UnitClassId));
				}

				foreach (var sensorClassId in uc.SensorClassIdList)
				{
					var sensor = GameManager.Instance.GetSensorClassById(sensorClassId);
					Assert.IsNotNull(sensor, string.Format("UnitClass {0}, SensorClass '{1}' not found",
						uc.UnitClassShortName, sensorClassId));
				}

				foreach (var wploads in uc.WeaponLoads)
				{
					foreach (var wpload in wploads.WeaponLoads)
					{
						WeaponClass weaponClass = GameManager.Instance.GameData.GetWeaponClassById(wpload.WeaponClassId);
						Assert.IsNotNull(weaponClass, string.Format("UnitClass {0}, Load {1}: WeaponClass '{2}' not found",
							uc.UnitClassShortName, wploads.Name, wpload.WeaponClassId));
						Assert.IsTrue(!string.IsNullOrEmpty(weaponClass.WeaponClassShortName), "WeaponClass should have a ShortName");
						if (weaponClass.SpawnsUnitOnFire)
						{
							var spawnUnitClass = GameManager.Instance.GetUnitClassById(weaponClass.SpawnUnitClassId);
							Assert.IsNotNull(spawnUnitClass, string.Format("UnitClass {0}, Load {1}: SpawnedUnitClass '{2}' not found",
								uc.UnitClassShortName, wploads.Name, weaponClass.SpawnUnitClassId));

						}
					}
				}
			}
            foreach (var sc in GameManager.Instance.GameData.Scenarios)
            {
                foreach (var alliance in sc.Alliences)
                {
                    foreach (var pl in alliance.ScenarioPlayers)
                    {
                        foreach (var gr in pl.Groups)
                        {
                            foreach (var u in gr.Units)
                            {
                                var uc = GameManager.Instance.GameData.GetUnitClassById(u.UnitClassId);
                                Assert.IsNotNull(uc, "Unit class '" + u.UnitClassId + "' not found.");
                            }
                        }
                    }
                }
            }
			foreach (var camp in GameManager.Instance.GameData.Campaigns)
			{
				foreach (var campScen in camp.CampaignScenarios)
				{
					var scen = GameManager.Instance.GameData.GetGameScenarioById(campScen.ScenarioId);
					Assert.IsNotNull(scen,
						string.Format("Campaign {0}, Scenario {1} not found.", camp.Id, campScen.ScenarioId));
                    //foreach (var reqScenId in campScen.RequiresCompletedScenarioIds)
                    //{
                    //    var reqScen = GameManager.Instance.GameData.GetGameScenarioById(reqScenId);
                    //    Assert.IsNotNull(reqScen,
                    //        string.Format("Campaign {0}, Required Scenario {1} not found.", camp.Id, reqScenId));

                    //}
				}
			}

            //Test for duplicates

            var ucSorted = from ucs in GameManager.Instance.GameData.UnitClasses
                           orderby ucs.Id
                           select ucs;
            UnitClass prevUc = null;
            foreach (var uc in ucSorted)
            {
                if (prevUc != null)
                {
                    Assert.IsFalse(prevUc.Id == uc.Id, uc.Id + " is a duplicate UnitClass.");
                }
                prevUc = uc;
            }

            var wcSorted = from wpc in GameManager.Instance.GameData.WeaponClasses
                           orderby wpc.Id
                           select wpc;
            WeaponClass prevWc = null;
            foreach (var wc in wcSorted)
            {
                if (prevWc != null)
                {
                    Assert.IsFalse(prevWc.Id == wc.Id, wc.Id + " is a duplicate WeaponClass.");
                }
            }

            var scSorted = from sc in GameManager.Instance.GameData.SensorClasses
                           orderby sc.Id
                           select sc;
            SensorClass prevSc = null;
            foreach (var sc in scSorted)
            {
                if (prevSc != null)
                {
                    Assert.IsFalse(prevSc.Id == sc.Id, sc.Id + " is a duplicate SensorClass.");
                }
            }



		}
	}
}
