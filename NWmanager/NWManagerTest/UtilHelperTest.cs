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
    public class UtilHelperTest
    {
        public UtilHelperTest()
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
        public void CompareDistanceCalculations()
        {
            List<Coordinate> coordinateList = new List<Coordinate>();
            Coordinate coordinate = new Coordinate(60,3);
            int noOfValues = 100000;
            
            for (int i = 0; i < noOfValues; i++)
            { 
                double randomBearing = GameManager.Instance.GetRandomNumber(360);
                double randomDistance = GameManager.Instance.GetRandomNumber(1000000);
                Coordinate newCoord = MapHelper.CalculateNewPosition2(coordinate, randomBearing, randomDistance);
                coordinateList.Add(newCoord);
            }
            DateTime timeStartApprox = DateTime.Now;
            double sumDistanceApprox = 0;
            for (int i = 0; i < noOfValues; i++)
            {
                double distanceM = MapHelper.CalculateDistanceRoughM(coordinate, coordinateList[i]);
                sumDistanceApprox += distanceM;
            }
            DateTime timeStartNormal = DateTime.Now;
            double sumDistanceExact = 0;
            for (int i = 0; i < noOfValues; i++)
            {
                double distanceM = MapHelper.CalculateDistanceM(coordinate, coordinateList[i]);
                sumDistanceExact += distanceM;
            }
            DateTime timeEnd = DateTime.Now;
            double differenceM = Math.Abs((sumDistanceExact / noOfValues) - (sumDistanceApprox / noOfValues));
            double distanceRoughM = MapHelper.CalculateDistanceRoughM(coordinate, coordinateList[1]);
            double distanceExactM = MapHelper.CalculateDistanceM(coordinate, coordinateList[1]);
            Assert.IsTrue(differenceM < 2000, "Avg. difference should be less than 2000 m");
            TimeSpan spanApprox = timeStartNormal.Subtract(timeStartApprox);
            TimeSpan spanNormal = timeEnd.Subtract(timeStartNormal);
            Assert.IsTrue(spanApprox.TotalMilliseconds < spanNormal.TotalMilliseconds, "Approx calculation should be faster");
            GameManager.Instance.Log.LogDebug(
                string.Format("Total time approx: {0}ms    Total time exact: {1}ms", 
                spanApprox.TotalMilliseconds, spanNormal.TotalMilliseconds));
        }

        [TestMethod]
        public void TestIsDistanceShorterThan()
        {
            List<Position> coordinateList = new List<Position>();
            Position position = new Position(new Coordinate(60, 3));
            int noOfValues = 100000;
            int maxDistanceM = 1000000;
            for (int i = 0; i < noOfValues; i++)
            {
                double randomBearing = GameManager.Instance.GetRandomNumber(360);
                double randomDistance = GameManager.Instance.GetRandomNumber(maxDistanceM);
                Coordinate newCoord = MapHelper.CalculateNewPosition2(position.Coordinate, randomBearing, randomDistance);
                coordinateList.Add(new Position(newCoord));
            }
            maxDistanceM = maxDistanceM / 100;
            DateTime timeStartShorter = DateTime.Now;
            int countDistancesNewMethod = 0;
            for (int i = 0; i < noOfValues; i++)
            {
                double distanceM = 0;
                if (MapHelper.IsDistanceShorterThan(position, coordinateList[i], maxDistanceM, out distanceM))
                {
                    countDistancesNewMethod++;
                }
            }
            DateTime timeEndShorter = DateTime.Now;
            TimeSpan spanShorter = timeEndShorter.Subtract(timeStartShorter);
            int countDistancesOldMethod = 0;
            for (int i = 0; i < noOfValues; i++)
            {
                if (MapHelper.CalculateDistance3DM(position, coordinateList[i]) < maxDistanceM)
                {
                    countDistancesOldMethod++;
                }
            }
            DateTime timeEndOldMethod = DateTime.Now;
            TimeSpan spanOldMethod = timeEndOldMethod.Subtract(timeEndShorter);
            GameManager.Instance.Log.LogDebug(
                string.Format("Total time new method: {0}ms    Total time old method: {1}ms  Count new: {2}   Count old {3} ",
                spanShorter.TotalMilliseconds, spanOldMethod.TotalMilliseconds, 
                countDistancesNewMethod, countDistancesOldMethod));
            //CONCLUSION: New method is actually slower if half are within range. 
            //However, if 10% are inside range, it is twice as fast! If only 1% are in range, it is three times faster.
        }

        [TestMethod]
        public void TestToRadianAndToDegree()
        {
            double twenty = 20;
            double ninety = 90;
            double twohundredtwentyfive = 225;
            double sixhundredfifty = 650;
            double threesixtyone = 361;

            Assert.IsTrue(Math.Abs(twenty.ToRadian().ToDegreeSignedLongitude() - twenty) < 0.01, "20 deg in rad in deg should be 20");
            Assert.IsTrue(Math.Abs(ninety.ToRadian().ToDegreeSignedLongitude() - ninety) < 0.01, "90 deg in rad in deg should be 90");
            Assert.IsTrue(Math.Abs(twohundredtwentyfive.ToRadian().ToDegreeBearing() - twohundredtwentyfive) < 0.01,
                "225 deg in rad in deg should be 225");
            double sixhundredfiftyCorr = sixhundredfifty.ToRadian().ToDegreeBearing();
            double threesixtyoneCorr = threesixtyone.ToRadian().ToDegreeBearing();

            Assert.IsFalse(Math.Abs(sixhundredfiftyCorr - sixhundredfifty) < 0.01, 
                "650 deg in rad in deg should NOT be 650");
            Assert.IsFalse(Math.Abs(threesixtyoneCorr - threesixtyone) < 0.01, 
                "361 deg in rad in deg should NOT be 361");
        }

        [TestMethod]
        public void CalculateElevationAngleBetweenPositionsDegTest()
        {
            Position pos1 = new Position(60, 5, 0, 0);
            Position pos2 = pos1.Offset(90, 1000);
            pos2.HeightOverSeaLevelM = 1000;
            double distanceM3d = MapHelper.CalculateDistance3DM(pos1, pos2);
            double angleDeg = MapHelper.CalculateElevationAngleDeg(pos1, pos2);
            Assert.IsTrue(Math.Abs(angleDeg - 45) < 5, "Angle should be around 45 degrees.");

            pos1.HeightOverSeaLevelM = 1000;
            pos2.HeightOverSeaLevelM = 0;
            angleDeg = MapHelper.CalculateElevationAngleDeg(pos1, pos2);
            Assert.IsTrue(Math.Abs(angleDeg + 45) < 5, "Angle should be around -45 degrees.");

            pos1.HeightOverSeaLevelM = 0;
            pos2.HeightOverSeaLevelM = -10000;
            angleDeg = MapHelper.CalculateElevationAngleDeg(pos1, pos2);
            Assert.IsTrue(Math.Abs(angleDeg + 88) < 5, "Angle should be around -88 degrees.");

            pos1.HeightOverSeaLevelM = 0;
            pos2.HeightOverSeaLevelM = -10;
            angleDeg = MapHelper.CalculateElevationAngleDeg(pos1, pos2);
            Assert.IsTrue(Math.Abs(angleDeg + 1) < 5, "Angle should be around -0.5 degrees.");

            pos2 = pos1.Offset(90, 100000);
            pos2.HeightOverSeaLevelM = 1000;
            angleDeg = MapHelper.CalculateElevationAngleDeg(pos1, pos2);
            Assert.IsTrue(Math.Abs(angleDeg  - 1) < 5, "Angle should be around +0.5 degrees.");
        }

        [TestMethod]
        public void CalculatePositionFromOffsetTest()
        {
            Position posMain = new Position(60, 5, 0, 0);
            PositionOffset offset = new PositionOffset(1000, 1000, 0); //right, forward, up
            Position newPos;

            posMain.BearingDeg = 0;
            offset = new PositionOffset(-1000, -1000, 0);
            newPos = MapHelper.CalculatePositionFromOffset2(posMain, offset);
            Assert.IsTrue(newPos.Coordinate.LatitudeDeg < 60, "Latitude should be < 60");
            Assert.IsTrue(newPos.Coordinate.LongitudeDeg < 5, "Longitude should be < 5");

            
            offset = new PositionOffset(1000,1000,0); //right, forward, up
            newPos = MapHelper.CalculatePositionFromOffset2(posMain, offset);
            Assert.IsTrue(newPos.Coordinate.LatitudeDeg > 60, "Latitude should be > 60");
            Assert.IsTrue(newPos.Coordinate.LongitudeDeg > 5, "Longitude should be > 5");

            posMain.BearingDeg = 180;

            newPos = MapHelper.CalculatePositionFromOffset2(posMain, offset);
            Assert.IsTrue(newPos.Coordinate.LatitudeDeg < 60, "Latitude should be < 60");
            Assert.IsTrue(newPos.Coordinate.LongitudeDeg < 5, "Longitude should be < 5");

            posMain.BearingDeg = 0;
            offset = new PositionOffset(1000, -1000, 0); //right, forward, up
            newPos = MapHelper.CalculatePositionFromOffset2(posMain, offset);
            Assert.IsTrue(newPos.Coordinate.LatitudeDeg < 60, "Latitude should be < 60");
            Assert.IsTrue(newPos.Coordinate.LongitudeDeg > 5, "Longitude should be > 5");

            posMain.BearingDeg = 0;
            offset = new PositionOffset(-1000, 1000, 0); //right, forward, up
            newPos = MapHelper.CalculatePositionFromOffset2(posMain, offset);
            Assert.IsTrue(newPos.Coordinate.LatitudeDeg > 60, "Latitude should be > 60");
            Assert.IsTrue(newPos.Coordinate.LongitudeDeg < 5, "Longitude should be < 5");




            //TODO: More tests!
        }

        [TestMethod]
        public void IsWithinRegionTest()
        {
            Region region = new Region();
            region.Coordinates.Add(new Coordinate(60, 3));
            region.Coordinates.Add(new Coordinate(60, 4));
            region.Coordinates.Add(new Coordinate(61, 4));
            region.Coordinates.Add(new Coordinate(61, 3));
            //region.Coordinates.Add(new Coordinate(60, 3));

            Coordinate coInside1 = new Coordinate(60.1, 3.1);
            Coordinate coInside2 = new Coordinate(60.9, 3.99);
            Coordinate coOutside1 = new Coordinate(59.9, 2.9);
            Coordinate coOutside2 = new Coordinate(60.1, -3.1);
            Coordinate coOutside3 = new Coordinate(-60.1, 3.1);
            Coordinate coOutside4 = new Coordinate(70.1, 3.1);

            bool resInside1 = region.IsWithinRegion(coInside1);
            bool resInside2 = region.IsWithinRegion(coInside2);
            bool resOutside1 = region.IsWithinRegion(coOutside1);
            bool resOutside2 = region.IsWithinRegion(coOutside2);
            bool resOutside3 = region.IsWithinRegion(coOutside3);
            bool resOutside4 = region.IsWithinRegion(coOutside4);
            Assert.IsTrue(resInside1 == resInside2 == true, "These coordinates should be inside");
            Assert.IsTrue(!resOutside1 && !resOutside2 && !resOutside3 && !resOutside4, "These coordinates should be outside");

            Region region2 = new Region(); //triangle
            region.Coordinates.Add(new Coordinate(60, 3));
            region.Coordinates.Add(new Coordinate(59, 2));
            region.Coordinates.Add(new Coordinate(59, 4));

            coInside1 = new Coordinate(59.5, 3);
            coOutside1 = new Coordinate(60, 2);
            resInside1 = region.IsWithinRegion(coInside1);
            resOutside1 = region.IsWithinRegion(coOutside1);

            Assert.IsTrue(resInside1, "This coordinate should be inside triangle");
            Assert.IsFalse(resOutside1, "This coordinate should be outside triangle");

            Region regionRect = Region.FromRectangle(new Coordinate(60, 3), 10000, 10000);
            Region regionCircle = Region.FromCircle(new Coordinate(60, 3), 10000);

            Coordinate coInside3 = new Position(new Coordinate(60, 3)).Offset(90, 4999).Coordinate;
            Coordinate coOutside5 = new Position(new Coordinate(60, 3)).Offset(90, 5500).Coordinate;
            bool resInside3 = regionRect.IsWithinRegion(coInside3);
            bool resOutside5 = regionRect.IsWithinRegion(coOutside5);
            Assert.IsTrue(resInside3, "This coordinate should be inside rectangle");
            Assert.IsFalse(resOutside5, "This coordinate should be outside rectangle");

            Coordinate coOutside6 = new Position(new Coordinate(60, 3)).Offset(90, 11000).Coordinate;

            bool resInside4 = regionCircle.IsWithinRegion(coInside3);
            bool resOutside6 = regionCircle.IsWithinRegion(coOutside6);
            Assert.IsTrue(resInside4, "This coordinate should be inside circle");
            Assert.IsFalse(resOutside6, "This coordinate should be outside circle");


        }

        [TestMethod]
        public void CalculateWithinSectorAndRange()
        {
            double InitialBearingDeg = 30;
            Coordinate coord1 = new Coordinate(60, 5);
            Coordinate coord2 = MapHelper.CalculateNewPosition2(coord1, InitialBearingDeg, 100000);
            double BearingDeg = MapHelper.CalculateBearingDegrees(coord1, coord2);
            double BearingDegBack = MapHelper.CalculateBearingDegrees(coord2, coord1);
            //Assert.IsTrue(Math.Abs(InitialBearingDeg - BearingDeg) < 0.1, "Bearing Degree between coordinates should be close to 30.");

            
            //double FortyDegInRad = double.Parse("40").ToRadian();
            //double FiftyDegInRad = double.Parse("50").ToRadian();
            bool ShouldBeInRange = MapHelper.IsAngleWithinRangeDeg(40, 30, 50);
            bool ShouldNotBeInRange = MapHelper.IsAngleWithinRangeDeg(50, 30, 40);

            Assert.IsTrue(ShouldBeInRange, "40 deg should be between 30 and 50 deg.");
            Assert.IsFalse(ShouldNotBeInRange, "50 deg should be between 30 and 40 deg.");

            bool ShouldBeInSector = MapHelper.IsWithinSector(coord1, 34, 10, coord2);
            bool ShouldNotBeInSector = MapHelper.IsWithinSector(coord1, 39, 10, coord2);

            Assert.IsTrue(ShouldBeInSector, "34 deg should be in 10 deg sector of 30 deg.");
            Assert.IsFalse(ShouldNotBeInSector, "39 deg should not be in 10 deg sector of 30 deg.");

        }

        [TestMethod]
        public void DistanceSamePoint0()
        {
            var distance = MapHelper.CalculateDistanceM(new Coordinate() { LatitudeDeg = 90, LongitudeDeg = -90 },
                           new Coordinate() { LatitudeDeg = 90, LongitudeDeg = -90 });
 
            Assert.AreEqual(0, distance, "Distance same point should be 0.");
 
        }

        [TestMethod]
        public void TestGetUniqueCode()
        {
             	//NWData.dll!TTG.NavalWar.NWData.GameManager.GenerateCode(long number = 3244785) Line 96 + 0x32 bytes	C#
            string code = GameManager.GenerateCode(32447850); //*10, works. so why stack overflow?

        }

        [TestMethod]
        public void TestCreateCoordinateGrid()
        {
            Position pos = new Position(60, 5);
            int gridCount = 9;
            var coordList = MapHelper.CreateCoordinateGrid(pos.Coordinate, gridCount, 1000);
            Assert.IsTrue(coordList.Count == gridCount, "Count should be " + gridCount);
            double distEw = MapHelper.CalculateDistanceM(coordList[0], coordList[2]);
            double distNs = MapHelper.CalculateDistanceM(coordList[0], coordList[6]);
            Assert.IsTrue(Math.Abs(distEw - 2000) < 50, "Distance EW should be about 2000m");
            Assert.IsTrue(Math.Abs(distNs - 2000) < 50, "Distance NS should be about 2000m");
        }

        [TestMethod]
        public void TestPositionRelationship()
        {
            Player player1 = new Player();
            Game game = GameManager.Instance.CreateGame(new Player(), "test game");
            GameManager.Instance.GameData.InitAllData();
            BaseUnit unit1 = GameManager.Instance.GameData.CreateUnit(player1, null,
                "arleighburke", "My Burke", new Position(60.0, 1.0, 0, 2), true);
            unit1.Position.BearingDeg = 90;
            unit1.SetActualSpeed(30);

            Player player2 = new Player();
            game.AddPlayer(player2);
            BaseUnit unit2 = GameManager.Instance.GameData.CreateUnit(player2, null,
                "arleighburke", "Their Burke", new Position(61.0, 1.0, 0, 2), true);
            unit2.Position.BearingDeg = 180;
            unit2.SetActualSpeed(30);
            var relativePos = MapHelper.CalculatePositionRelationship(
                unit1.Position, unit1.ActualSpeedKph, unit2.Position, unit2.ActualSpeedKph);
            Assert.IsTrue(
                relativePos.RelativeBearing == GameConstants.RelativeBearing.MovingTowards, "Should be moving towards.");
            Assert.IsTrue(relativePos.RelativeSpeed == 0, "Should be same speed");

            unit2.Position.BearingDeg = 95;
            unit2.SetActualSpeed(35);

            relativePos = MapHelper.CalculatePositionRelationship(
                unit1.Position, unit1.ActualSpeedKph, unit2.Position, unit2.ActualSpeedKph);
            Assert.IsTrue(
                relativePos.RelativeBearing == GameConstants.RelativeBearing.MovingTowards, "Should be moving towards.");
            Assert.IsTrue(relativePos.RelativeSpeed < 0, "Should be higher speed");

            unit2.Position.BearingDeg = 20;
            unit2.SetActualSpeed(20);
            relativePos = MapHelper.CalculatePositionRelationship(
                            unit1.Position, unit1.ActualSpeedKph, unit2.Position, unit2.ActualSpeedKph);
            Assert.IsTrue(
                relativePos.RelativeBearing == GameConstants.RelativeBearing.MovingAway, "Should be moving away.");
            Assert.IsTrue(relativePos.RelativeSpeed > 0, "Target should be lower speed");

            unit2.Position.BearingDeg = 320;
            relativePos = MapHelper.CalculatePositionRelationship(
                            unit1.Position, unit1.ActualSpeedKph, unit2.Position, unit2.ActualSpeedKph);
            Assert.IsTrue(
                relativePos.RelativeBearing == GameConstants.RelativeBearing.MovingAway, "Should be moving away.");
            

        }

        [TestMethod]
        public void CoordinateTest()
        {
            double lon = -181;
            double lonRad = lon.ToRadian();
            lon = lonRad.ToDegreeSignedLongitude();
            Coordinate coord1 = new Coordinate(0, lon);
            Assert.IsFalse(coord1.LongitudeDeg == -181);

            double lat = -91;
            double latRad = lat.ToRadian();
            lat = latRad.ToDegreeSignedLatitude();
            coord1 = new Coordinate(lat, 30);
            Assert.IsFalse(coord1.LatitudeDeg == -91);
            //coord1 = new Coordinate(60, 181);
            //Assert.IsFalse(coord1.Longitude == 181);
            //coord1 = new Coordinate(91, 9);
            //Assert.IsFalse(coord1.Latitude == 91);
            //coord1 = new Coordinate(5, -181);
            //Assert.IsFalse(coord1.Longitude == -181);


        }

        [TestMethod]
        public void BitConverterTest()
        {
            byte[] bytes = new byte[2] { 255, 255 };
            UInt16 u16 = BitConverter.ToUInt16(bytes, 0);
            int i32 = u16;

        }

        [TestMethod]
        public void TestToDegreeSigned()
        {
            for (int i = -4; i < 4; i++)
            {
                double rad = i;
                double degSigned = rad.ToDegreeSignedLongitude();
                Assert.IsTrue(degSigned >= -180.0 && degSigned <= 180.0, "Should be between -180 and 180");
            }
            for (int i = -4; i < 4; i++)
            {
                double rad = i;
                double degSigned = rad.ToDegreeSignedLatitude();
                Assert.IsTrue(degSigned >= -90.0 && degSigned <= 90.0, "Should be between -90 and 90");
            }
        }

        [TestMethod]
        public void DistanceSample()
        {
            var distance = MapHelper.CalculateDistanceM(new Coordinate() { LatitudeDeg = 45, LongitudeDeg = 0 },
                           new Coordinate() { LatitudeDeg = 0, LongitudeDeg = 45 });
            double actualdistance = 6671374.6175999995;
            Assert.IsTrue(distance - actualdistance < 0.01, "Distance sample should be very close to a predefined  value."); //6671.3746175999995d km
            //Assert.AreEqual(6671374.6175999995d, distance, "Distance sample should equal a predefined  value."); //6671.3746175999995d km //weird. Failed on Win7 64bit

        }

        [TestMethod]
        public void Distance3dTest()
        {
            Position pos1 = new Position(60, 5, 0);
            Position pos2 = pos1.Offset(45, 1000);
            pos2.HeightOverSeaLevelM = 1000;
            double distance2d = MapHelper.CalculateDistanceM(pos1.Coordinate, pos2.Coordinate);
            double distance3d = MapHelper.CalculateDistance3DM(pos1, pos2);
            Assert.IsTrue(distance2d != distance3d, "2D and 3D distance should not be the same.");
            Assert.IsTrue(Math.Abs(distance3d - 1414) < 10, "3d distance should be around 1414 meters.");

        }

        [TestMethod]
        public void ReverseGetUniqueId()
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
            unit.Position = new Position(60.0, 1.0, 50, 2);
            unit.SetActualSpeed(500);
            unit.Name = "F-Flop/B";
            Position dest = new Position(61.0, 2.0, 5000);
            unit.MovementOrder.AddWaypoint(new Waypoint(dest));
            long UniqueId = GameManager.GetNumericId(unit.Id);
            GameManager.Instance.Log.LogDebug("Numeric id =" + UniqueId);
            UniqueId = GameManager.GetNumericId("");
            UniqueId = GameManager.GetNumericId("qa");
        }

        [TestMethod]
        public void DistanceAndBack()
        {
            Coordinate coord1 = new Coordinate(60, 5);
            double bearingDeg1 = 45;
            double bearingDeg2 = 117;
            double distanceM = 10000;
            Coordinate coord2 = MapHelper.CalculateNewPosition2(coord1, bearingDeg1, distanceM);
            Coordinate coord3 = MapHelper.CalculateNewPosition2(coord1, bearingDeg2, distanceM);

            double distance1to2m = MapHelper.CalculateDistanceM(coord1, coord2);
            double distance1to3m = MapHelper.CalculateDistanceM(coord1, coord3);
            double distance2to1m = MapHelper.CalculateDistanceM(coord2, coord1);
            double distance3to1m = MapHelper.CalculateDistanceM(coord3, coord1);

            Assert.IsTrue(Math.Abs(distanceM - distance1to2m) < (distanceM/100), "Back and forth should be approximately the same.");
        }

        [TestMethod]
        public void TestHeightDepthMark()
        {
            double hi = 10000.0;
            double low = -200.0;
            double vhi = 100000.0;
            GameConstants.HeightDepthPoints HdHi = hi.ToHeightDepthMark();
            GameConstants.HeightDepthPoints HdLo = low.ToHeightDepthMark();
            GameConstants.HeightDepthPoints HdVHi = vhi.ToHeightDepthMark();
            System.Diagnostics.Debug.WriteLine(hi + " meters is " + HdHi, ToString());
            System.Diagnostics.Debug.WriteLine(low + " meters is " + HdLo, ToString());
            System.Diagnostics.Debug.WriteLine(vhi + " meters is " + HdVHi, ToString());

            Assert.AreEqual(HdHi, GameConstants.HeightDepthPoints.High, " Should be High.");
            Assert.AreEqual(HdLo, GameConstants.HeightDepthPoints.Deep, " Should be Deep.");
            Assert.AreEqual(HdVHi, GameConstants.HeightDepthPoints.VeryHigh, " Should be Very high.");

        }

        [TestMethod]
        public void LoadClassDataFromXml()
        {
            GameManager.Instance.GameData.InitAllData();
            Country country = GameManager.Instance.GetCountryById("no");
            UnitClass unitclass = GameManager.Instance.GetUnitClassById("f22");
            UnitClass unitclass2 = GameManager.Instance.GetUnitClassById("notexisting");
            Assert.IsNotNull(country, "Country should not be null.");
            Assert.IsNotNull(unitclass, "UnitClass should not be null.");
            Assert.IsNull(unitclass2, "UnitClass2 should be null.");
            //GameManager.Instance.TerminateGame();
        }

        [TestMethod]
        public void TestDistanceToHorizon()
        {
            double Distance = MapHelper.CalculateMaxLineOfSightM(10, 100);
            double Distance2 = MapHelper.CalculateMaxLineOfSightM(110, 0);
            GameManager.Instance.Log.LogDebug("TestDistanceToHorizon reports: " + Distance + " meters.");
            Assert.IsTrue(Distance > 3000 & Distance < 4000, "Distance should be 3571 meters.");
            Assert.IsTrue(Distance == Distance2, "Distances should be exactly equal.");
        }

        [TestMethod]
        public void TestProjCordinate()
        {
            Coordinate C1 = new Coordinate(61.0, 5.0);
            ProjCoordinate P1 = MapProjection.ToEquiProjectedCoordinate(C1);
            Coordinate C2 = MapProjection.FromEquiCoordinate(P1);

            double d = C1.LatitudeDeg - C2.LatitudeDeg;
            double d2 = C1.LongitudeDeg - C2.LongitudeDeg;

            Assert.IsFalse(d > 0.1 && d < -0.1, "Coordinate C1 != C2");
            Assert.IsFalse(d2 > 0.1 && d2 < -0.1, "Coordinate C1 != C2");
            //Assert.AreEqual(C1.Latitude, C2.Latitude, "Coordinate C1 != C2");
            //Assert.AreEqual(C1.Longitude, C2.Longitude, "Coordinate C1 != C2");
        }

        [TestMethod]
        public void TestOrthographicProjection()
        {
            Coordinate coorOrigo = new Coordinate(89.99999999, 0.000000000001);
            ProjCoordinate pCoordOrigo = coorOrigo.ToOrthoProjectedCoordinate();

            Coordinate coordSouthEast = new Coordinate(0, -90);
            Coordinate coordSouthMeredian = new Coordinate(0, 0);
            ProjCoordinate pCoordSouthEast = coordSouthEast.ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordSouthMeredian = coordSouthMeredian.ToOrthoProjectedCoordinate();
            Coordinate coordBergen = new Coordinate(60, 5);
            Coordinate coordLondon = new Coordinate(51.508056, -0.124722);
            Coordinate coordReykjarvik = new Coordinate(64.133333, -21.933333);
            Coordinate coordBoston = new Coordinate(42.357778, -71.061667);
            Coordinate coordMoscow = new Coordinate(55.751667, 37.617778);
            Coordinate coordNorthCape = new Coordinate(71.1725, 25.794444);

            ProjCoordinate pCoordBergen = coordBergen.ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordLondon = coordLondon.ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordReykjarvik = coordReykjarvik.ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordBoston = coordBoston.ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordMoscow = coordMoscow.ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordNorthcape = coordNorthCape.ToOrthoProjectedCoordinate();

            ProjCoordinate pCoordNorthOfBergen = new Coordinate(62, 5).ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordSouthOfBergen = new Coordinate(58, 5).ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordWestOfBergen = new Coordinate(60, 0).ToOrthoProjectedCoordinate();
            ProjCoordinate pCoordEastOfBergen = new Coordinate(60, 10).ToOrthoProjectedCoordinate();
            GameManager.Instance.Log.LogDebug("*** TestOrthographicProjection()");
            GameManager.Instance.Log.LogDebug("NORTH TO SOUTH ALONG NULL-MERIDIAN, EVERY 10"); 
            for (int i = 90; i > 0; i-=10)
            {
                Coordinate coor = new Coordinate(i, 0);
                ProjCoordinate pcoor = coor.ToOrthoProjectedCoordinate();
                GameManager.Instance.Log.LogDebug(string.Format("Coordinate {0} is projected as {1}", coor, pcoor)); 
            }
            GameManager.Instance.Log.LogDebug("*** WEST TO EAST ALONG 60 DEG N, EVERY 10"); 
            for (int i = -90; i < 90; i+=10)
            {
                Coordinate coor = new Coordinate(60, i);
                ProjCoordinate pcoor = coor.ToOrthoProjectedCoordinate();
                GameManager.Instance.Log.LogDebug(string.Format("Coordinate {0} is projected as {1}", coor, pcoor));
            }

            Coordinate coordNullNull = new ProjCoordinate(0, 0).FromOrthoProjectedCoordinate();

            Coordinate coord2 = pCoordOrigo.FromOrthoProjectedCoordinate();
            double DistanceM = MapHelper.CalculateDistanceM(coorOrigo, coord2);
            Assert.IsTrue(DistanceM < 1, "Coordinate 1 projected and back again should be the same");

        }


        [TestMethod]
        public void RandomTimeTest()
        {
            double timeElapsedSec = 1;
            double probabilityMinute = 50;
            double prob = GameManager.Instance.GetProbabilityTimePercent(timeElapsedSec, probabilityMinute);
            double probShouldBe = (probabilityMinute * (timeElapsedSec / 60));
            Assert.IsTrue(prob == probShouldBe,
                string.Format("GetProbabilityTimePercent returns {0}; should return {1}.", prob, probShouldBe));
        }

        [TestMethod]
        public void RandomTest()
        {
            int count20Percent = 0;
            int countFiftyFifty = 0;
            int countOnePercent = 0;
            int count = 1;
            int NoOfIterations = 1000;
            while (count < NoOfIterations)
            {
                if (GameManager.Instance.ThrowDice(20))
                {
                    count20Percent++;
                }
                if (GameManager.Instance.ThrowDice(50))
                {
                    countFiftyFifty++;
                }
                if (GameManager.Instance.ThrowDice(1))
                {
                    countOnePercent++;
                }

                count++;
            }
            Assert.IsTrue(Math.Abs(count20Percent - (NoOfIterations * 0.2)) < (NoOfIterations/10), "Around 20% of results should be true. Count=" + count20Percent);
        }

        [TestMethod]
        public void JulianDayTest()
        {
            DateTime date = new DateTime(2007, 1, 14);
            double jd = date.DtToJulianDay();
            Assert.IsTrue(jd == 2454115, "Julian day should be 2454115(wikipedia)");
            DateTime greg = jd.JdToDateTime();
            Assert.IsTrue(greg.Year == 2007, "Gregorian date should return same year.");
        }

        [TestMethod]
        public void MoonRiseSetTest()
        {
            //Answers: http://aa.usno.navy.mil/data/docs/RS_OneDay.php

            DateTime date = new DateTime(2009, 10, 12);
            date = date.AddHours(7);
            Coordinate coord = new Coordinate(60, -5.3);
            //MoonRiseResult result = WeatherSystem.CalculateMoonRiseSet(coord, date);
            double jd = date.DtToJulianDay();
            //bool is_rise = true;
            GameConstants.RiseSetType riseSetType = GameConstants.RiseSetType.Lunar;
            GameConstants.TideEvent tideEvent = GameConstants.TideEvent.MoonSet;
            GameConstants.TideEventType tideEventType = GameConstants.TideEventType.NewMoon; //init only

            double jdout;
            AstronomyHelper.findNextRiseOrSet(date, coord, ref riseSetType, ref tideEvent, out jdout);
            DateTime timeMoonEvent = jdout.JdToDateTime(); //30Sept: Moonrise 17:07
            AstronomyHelper.findNextMoonPhase(date, out tideEventType);
            Debug.Write("Tide event type: " + tideEventType);
            Assert.IsTrue(tideEvent == GameConstants.TideEvent.MoonSet, "MoonSet 18:42ish on 30 sept 2030.");
            //Assert.IsTrue(result.IsMoonRise == true, "Moon rises on this date");
            //Assert.IsNotNull(result, "Result should not be null.");
        }

        [TestMethod]
        public void InterpolationTest()
        { 
            double v1 = 20;
            double v2 = 100;
            double weightv1 = 50;
            double result = WeatherSystem.InterpolateWeighedDouble(v1, v2, weightv1);

            weightv1 = 80;
            result = WeatherSystem.InterpolateWeighedDouble(v1, v2, weightv1);


        }

        [TestMethod]
        public void WeatherSystemTest()
        {
            int CloudCover = 0;
            DateTime date = new DateTime(2009, 10, 12);
            date = date.AddHours(7);
            Coordinate coord = new Coordinate(64, 10);
            double sunshine = WeatherSystem.CalculateSunShineWm2(date, 5, coord, CloudCover);
            //Assert.IsTrue(sunshine > 510 && sunshine < 560, 
            //    string.Format("Sunshine Wm2 is {0}; should be between 510 and 560", sunshine));

            double sunheightDeg = WeatherSystem.CalculateSunHeightDeg(date, coord);
            //Assert.IsTrue(sunheightDeg > 32 && sunheightDeg < 34, 
            //    string.Format("Sunheight Deg is {0}; should be around 33.8", sunheightDeg));

            double sunDeclination = WeatherSystem.CalculateSunDeclination(date, coord);
            //Assert.IsTrue(sunDeclination > 4.1 && sunDeclination < 4.4,
            //    string.Format("Sun Declination Deg is {0}; should be 4.3", sunDeclination));


            DateTimeFromTo SunRiseSet = WeatherSystem.CalculateSunRiseSunSet(date, coord);
            if (SunRiseSet.ToTime != null && SunRiseSet.FromTime != null)
            { 
                GameManager.Instance.Log.LogDebug(string.Format(
                    "WeatherSystemTest() : Sunrise at {0}, Sunset at {1}", 
                    SunRiseSet.FromTime, SunRiseSet.ToTime));
            }
            GameManager.Instance.CreateGame(new Player(),"test game");
            GameManager.Instance.Game.GameStartTime = date;
            GameManager.Instance.Game.UpperLeftCorner = new Coordinate(70, -10);
            GameManager.Instance.Game.LowerRightCorner = new Coordinate(40, 10);
            GameManager.Instance.GameData.InitMainWeatherSystems(
                GameConstants.WeatherSystemTypes.Rough,
                GameConstants.WeatherSystemSeasonTypes.Autumn);
            GameManager.Instance.GameData.RecreateWeatherDataFromMain();
            WeatherSystem weather = GameManager.Instance.GameData.GetWeather(new Coordinate(60, 5));
            Assert.IsNotNull(weather, "Weather should not be null.");

            var weather50degN = WeatherSystem.CreateRandomWeatherSystem(GameConstants.WeatherSystemTypes.Fine, GameConstants.WeatherSystemSeasonTypes.Autumn, 50);
            var weather70degN = WeatherSystem.CreateRandomWeatherSystem(GameConstants.WeatherSystemTypes.Fine, GameConstants.WeatherSystemSeasonTypes.Autumn, 70);

            GameManager.Instance.Game.GameCurrentTime = new DateTime(2030, 9, 29, 13, 20, 0);
            GameManager.Instance.GameData.RecreateWeatherDataFromMain();
            var wsystem1 = GameManager.Instance.GameData.GetWeather(new Coordinate(60, 5));
            Assert.IsTrue(wsystem1.TotalLightPercent > 75, "There should be daylight.");

            GameManager.Instance.Game.GameCurrentTime = new DateTime(2030, 9, 29, 22, 20, 0);
            GameManager.Instance.GameData.RecreateWeatherDataFromMain();
            var wsystem2 = GameManager.Instance.GameData.GetWeather(new Coordinate(60, 5));
            Assert.IsTrue(wsystem2.TotalLightPercent < 10, "There should be night.");

        }
    }
}
