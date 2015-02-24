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

namespace NWManagerTest
{
    /// <summary>
    /// Summary description for VictoryConditionTest
    /// </summary>
    [TestClass]
    public class VictoryConditionTest
    {
        public VictoryConditionTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void TestUnitsInAreaCheck()
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
            BaseUnit unit1 = GameManager.Instance.GameData.CreateUnit(player, group1, "arleighburke", "DDG Valiant", pos1, true);
            Position pos2 = new Position(60, 3.2, 500, 120);
            BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player, group2, "f22", "F22-AA", pos2, true);
            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unit1.Position.Coordinate,
                45, 1000);
            //unit2.SetActualSpeed(100);
            unit2.UserDefinedSpeed = TTG.NavalWar.NWComms.GameConstants.UnitSpeedType.Slow;
            //unit2.ActualSpeedKph = unit2.UnitClass.MinSpeedKph;
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true);
            BaseUnit unitEnemyF22 = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, null, "f22", "Raptor", pos1.Offset(130,1200), true);
            
            BaseUnit unitEnemyFisher = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, null, "fishingboat", "Fish", pos1.Offset(40,400), true);
            unitEnemyF22.Position.HeightOverSeaLevelM = 1000;
            unitEnemyFisher.Position.HeightOverSeaLevelM = 0;
            foreach (var weapons in unit1.Weapons)
            {
                weapons.AmmunitionRemaining = 0; //prevent units from shooting down each other
            }
            foreach (var weapons in unit2.Weapons)
            {
                weapons.AmmunitionRemaining = 0; 
            }

            foreach (var weapons in unitEnemy.Weapons)
            {
                weapons.AmmunitionRemaining = 0; 
            }
            foreach (var weapons in unitEnemyF22.Weapons)
            {
                weapons.AmmunitionRemaining = 0; 
            }

            game.IsNetworkEnabled = false;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();
            //game.Tick(10);

            List<BaseUnit> ListUnitsClassWide = (List<BaseUnit>)player.GetAllUnitsInArea(pos1.Coordinate, 10000000);
            List<BaseUnit> ListUnitsClassClose = (List<BaseUnit>)player.GetAllUnitsInArea(pos1.Coordinate, 1000);
            List<BaseUnit> ListUnitsClass = (List<BaseUnit>)player.GetUnitsInAreaByClassId("arleighburke", pos1.Coordinate, 1000);
            List<BaseUnit> ListUnitsRole = (List<BaseUnit>)player.GetUnitsInAreaByRole(
                TTG.NavalWar.NWComms.GameConstants.Role.InterceptAircraft, pos2.Coordinate, 10000);
            
            List<BaseUnit> ListEnemyUnitsAll = (List<BaseUnit>)player.GetAllEnemyUnits();
            List<BaseUnit> ListEnemyUnitsArea = (List<BaseUnit>)player.GetEnemyUnitsInArea(NextDoorPos.Coordinate, 1000);
            List<BaseUnit> ListEnemyUnitsRoleArea = (List<BaseUnit>)player.GetEnemyUnitsInAreaByRole(
                TTG.NavalWar.NWComms.GameConstants.Role.IsSurfaceCombattant,NextDoorPos.Coordinate, 1000);
            List<BaseUnit> ListEnemyUnitsClassArea = (List<BaseUnit>)player.GetEnemyUnitsInAreaByUnitClass("arleighburke", 
                NextDoorPos.Coordinate, 1000);

            Assert.IsTrue(ListUnitsClassWide.Count == 2, "Two units should be present in wide area count.");
            Assert.IsTrue(ListUnitsClassClose.Count == 1, "Only one unit should be present in close area count.");
            Assert.IsTrue(ListUnitsClass.Count == 1, "One arleigh burke should be found in area 1.");
            Assert.IsTrue(ListUnitsRole.Count == 1, "One interceptor (f22) should be found in area 2.");

            Assert.IsTrue(ListEnemyUnitsAll.Count == 3, "Enemy should have 3 units totally.");
            Assert.IsTrue(ListEnemyUnitsArea.Count == 2, "Enemy should have 2 units in area.");
            Assert.IsTrue(ListEnemyUnitsRoleArea.Count == 1, "Enemy should have one unit with surface combattant role in area.");
            Assert.IsTrue(ListEnemyUnitsClassArea.Count == 1, "Enemy should have one arleigh burke in area.");

            GameManager.Instance.Game.Terminate();
        }

        [TestMethod]
        public void TestVictoryConditions()
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
            BaseUnit unit1 = GameManager.Instance.GameData.CreateUnit(player, group1, "arleighburke", "DDG Valiant", pos1, true);
            unit1.Tag = "burke";

            Position pos2 = new Position(60, 3.2, 500, 120);
            BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player, group2, "f22", "F22-AA", pos2, true);
            Coordinate NextDoor = MapHelper.CalculateNewPosition2(unit1.Position.Coordinate,
                45, 1000);
            unit2.UserDefinedSpeed = TTG.NavalWar.NWComms.GameConstants.UnitSpeedType.Slow;
            Position NextDoorPos = new Position(NextDoor);
            NextDoorPos.HeightOverSeaLevelM = 0;
            NextDoorPos.SetNewBearing(130);
            Group grpEnemy = new Group();
            BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, grpEnemy, "arleighburke", "DDG Vicious", NextDoorPos, true);
            BaseUnit unitEnemyF22 = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, null, "f22", "Raptor", pos1.Offset(130, 1200), true);

            BaseUnit unitEnemyFisher = GameManager.Instance.GameData.CreateUnit(
                playerEnemy, null, "fishingboat", "Fish", pos1.Offset(40, 400), true);
            unitEnemyFisher.Tag = "fishingboat";
            unitEnemyF22.Position.HeightOverSeaLevelM = 1000;
            unitEnemyFisher.Position.HeightOverSeaLevelM = 0;
            foreach (var weapons in unit1.Weapons)
            {
                weapons.AmmunitionRemaining = 0; //prevent units from shooting down each other
            }
            foreach (var weapons in unit2.Weapons)
            {
                weapons.AmmunitionRemaining = 0;
            }

            foreach (var weapons in unitEnemy.Weapons)
            {
                weapons.AmmunitionRemaining = 0;
            }
            foreach (var weapons in unitEnemyF22.Weapons)
            {
                weapons.AmmunitionRemaining = 0;
            }

            player.EventTriggers = new List<TTG.NavalWar.NWComms.Entities.EventTrigger>()
            {
                new PlayerObjective()
                {
                    EventTriggerType = TTG.NavalWar.NWComms.GameConstants.EventTriggerType.EnemyUnitTagIsDestroyed,
                    Tag="fishingboat",
                    IsVictoryCondition = true,
                }
            };

            game.IsNetworkEnabled = false;
            GameManager.Instance.Game.RunGameInSec = 1;
            GameManager.Instance.Game.StartGamePlay();

            player.ExecuteEventTriggers();
            player.CheckForVictoryConditions();

            Assert.IsTrue(player.HasWonGame == false, "Player should not have won game.");
            unitEnemyFisher.IsMarkedForDeletion = true;

            player.ExecuteEventTriggers();
            player.CheckForVictoryConditions();

            Assert.IsTrue(player.HasWonGame, "Player should now have won game.");

        }

        
    //    [TestMethod]
    //    public void TestSimpleVictoryConditions()
    //    {
    //        //TODO: Test more complex rules.
    //        Player player = new Player();
    //        player.Name = "GoodGuy";
    //        player.IsComputerPlayer = true;
    //        Player playerEnemy = new Player();
    //        playerEnemy.Name = "BadGuy";
    //        playerEnemy.IsComputerPlayer = true;

    //        player.DefeatConditionSets = new List<DefeatConditionSet>()
    //        {
    //            new DefeatConditionSet()
    //            {
    //                OwnerPlayer = player,
    //                DefeatConditions = new List<DefeatCondition>()
    //                {
    //                    new DefeatCondition()
    //                    {
    //                        DefeatConditionType = TTG.NavalWar.NWComms.GameConstants.DefeatConditionType.CountUnitClassIdLowerThan,
    //                        ConditionParameter = "arleighburke",
    //                        ConditionValue = 1,
    //                        OwnerPlayer = player,
    //                        DescriptionEnemy = "Must have at least one Arleigh Burke class destroyer"
    //                    }
    //                }
    //            }
    //        };


    //        playerEnemy.DefeatConditionSets = new List<DefeatConditionSet>()
    //        {
    //            new DefeatConditionSet()
    //            {
    //                OwnerPlayer = playerEnemy,
    //                DefeatConditions = new List<DefeatCondition>()
    //                {
    //                    new DefeatCondition()
    //                    {
    //                        DefeatConditionType = TTG.NavalWar.NWComms.GameConstants.DefeatConditionType.CountUnitClassIdLowerThan,
    //                        ConditionParameter = "arleighburke",
    //                        ConditionValue = 2,
    //                        OwnerPlayer = playerEnemy,
    //                        DescriptionEnemy = "Must have at least two Arleigh Burke class destroyers"
    //                    }
    //                }
    //            }
    //        };
    //        Group group1 = new Group();
    //        Group group2 = new Group();
    //        GameManager.Instance.GameData.InitAllClassData();
    //        Game game = GameManager.Instance.CreateGame(player, "test game");
    //        game.Players.Add(player);
    //        game.Players.Add(playerEnemy);
    //        GameManager.Instance.Game.SetAllPlayersEnemies();


    //        Position pos1 = new Position(60, 3, 0, 120);
    //        BaseUnit unit1 = GameManager.Instance.GameData.CreateUnit(player, group1, "arleighburke", "DDG Valiant", pos1, true);
    //        Position pos2 = new Position(60, 3.2, 500, 120);
    //        BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player, group2, "f22", "F22-AA", pos2, true);
    //        Coordinate NextDoor = MapHelper.CalculateNewPosition2(unit1.Position.Coordinate,
    //            45.0, 1000);
    //        unit2.ActualSpeedKph = unit2.UnitClass.MinSpeedKph;
    //        Position NextDoorPos = new Position(NextDoor);
    //        NextDoorPos.HeightOverSeaLevelM = 0;
    //        NextDoorPos.SetNewBearing(130);
    //        Group grpEnemy = new Group();
    //        BaseUnit unitEnemy = GameManager.Instance.GameData.CreateUnit(playerEnemy, group1, "arleighburke", "DDG Vicious", NextDoorPos, true);

    //        game.IsNetworkEnabled = false;
    //        GameManager.Instance.Game.RunGameInSec = 1;
    //        GameManager.Instance.Game.StartGamePlay();
    //        //game.Tick(10);

    //        bool IsPlayerDefeated = player.HasBeenDefeated;
    //        bool IsPlayerEnemyDefeated = playerEnemy.HasBeenDefeated;

    //        Assert.IsFalse(IsPlayerDefeated, "Player has one Arleigh Burke and should not be defeated.");
    //        Assert.IsTrue(IsPlayerEnemyDefeated, "PlayerEnemy has < 2 Arleigh Burke and should be defeated.");
    //        GameManager.Instance.Game.Terminate();
    //    }
    }
}
