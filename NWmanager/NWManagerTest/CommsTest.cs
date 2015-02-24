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
    /// Summary description for CommsTest
    /// </summary>
    [TestClass]
    public class CommsTest
    {
        public CommsTest()
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
        public void TestGeneralArrayCopy()
        {
            byte[] fromArray = new byte[2] { 32, 31 };
            byte[] toArray = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Array.Copy(fromArray, 0, toArray, 2, fromArray.Length);
            Assert.IsTrue(toArray[2] == 32, "Array value 0 should be in place.");
            Assert.IsTrue(toArray[3] == 31, "Array value 0 should be in place.");

        }

        [TestMethod]
        public void TestSerializingDeserializing()
        {
            CommsMarshaller marshaller = new CommsMarshaller();
            MessageString str = new MessageString();
            str.Message = "Hello!";
            byte[] bytes = marshaller.SerializeObjectForSending(str);
            marshaller.AddToReceiveBufferEnd(bytes, bytes.Length);
            IMarshallable obj = marshaller.DeSerializeNextObjectInReceiveBuffer();
            Assert.IsNotNull(obj, "Object from send buffer should not be null.");
        }

        [TestMethod]
        public void TestSerializePositionInfo()
        { 
            GameManager.Instance.CreateGame(new Player(), "test game");
            BaseUnit unit = new BaseUnit();
            unit.UnitClass = new UnitClass("Ship", GameConstants.UnitType.SurfaceShip);
            unit.UnitClass.TurnRangeDegrSec = 10;
            unit.UnitClass.MaxAccelerationKphSec = 10;
            unit.UnitClass.MaxSpeedKph = 40;
            unit.Position = new Position(60.0, 1.0, 0, 2);
            unit.Name = "USS Neverdock";
            Position dest = new Position(61.0, 2.0);
            unit.MovementOrder.AddWaypoint(new Waypoint(dest));
            CommsMarshaller marshaller = new CommsMarshaller();
            PositionInfo pos = unit.GetPositionInfo();
            byte[] bytes1 = marshaller.SerializeObjectForSending(pos);
            marshaller.AddToReceiveBufferEnd(bytes1, bytes1.Length);
            IMarshallable obj1 = marshaller.DeSerializeNextObjectInReceiveBuffer();
            Assert.IsNotNull( obj1, "Should be 1 object in buffer." );
            PositionInfo posinfo = (PositionInfo)obj1;
            Assert.AreEqual(unit.Id, posinfo.UnitId, "Id should be the same");
            GameManager.Instance.TerminateGame();
        }

        [TestMethod]
        public void TestWaypointClone()
        { 
            Waypoint wp = new Waypoint(new Coordinate(60.2, 5.3));
            Waypoint wp2 = wp.Clone();
            wp2.Position.Coordinate = new Coordinate(60,5);
            Assert.IsTrue(wp.Position.Coordinate.LatitudeDeg != wp2.Position.Coordinate.LatitudeDeg, 
                "Latitudes should not be the same.");

        }
        //[TestMethod]
        //public void TestSerializingDeserializingMultipleObjects()
        //{
        //    CommsMarshaller marshaller = new CommsMarshaller();
        //    Entity ent1 = new Entity() { Id = 1, Name = "A", X = 2, Y = 3 };
        //    Entity ent2 = new Entity() { Id = 2, Name = "B", X = 5, Y = 5 };
        //    MessageString str = new MessageString() { Message = "Hei på deg!" };
        //    byte[] bytes1 = marshaller.SerializeObjectForSending(ent1);
        //    byte[] bytes2 = marshaller.SerializeObjectForSending(ent2);
        //    byte[] bytes3 = marshaller.SerializeObjectForSending(str);
        //    marshaller.AddToBufferEnd(bytes1, bytes1.Length);
        //    marshaller.AddToBufferEnd(bytes3, bytes3.Length);
        //    marshaller.AddToBufferEnd(bytes2, bytes2.Length);
        //    marshaller.DeSerializeNextObjectInBuffer();
        //    marshaller.DeSerializeNextObjectInBuffer();
        //    marshaller.DeSerializeNextObjectInBuffer();
        //    Assert.IsTrue(marshaller.ReceivedObjectsQueue.Count == 3, "Should be 3 objects in buffer.");
        //    IMarshallable obj1 = marshaller.ReceivedObjectsQueue.Dequeue();
        //    IMarshallable obj2 = marshaller.ReceivedObjectsQueue.Dequeue();
        //    IMarshallable obj3 = marshaller.ReceivedObjectsQueue.Dequeue();


        //}

    }
}
