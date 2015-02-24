using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData;
using TTG.NavalWar;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWComms;

namespace NWManagerTest
{
    /// <summary>
    /// Summary description for MessageTest
    /// </summary>
    [TestClass]
    public class MessageTest
    {
        public MessageTest()
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
        public void CreateNewMessage()
        {
            Player p1 = new Player();
            Player p2 = new Player();
            GameManager.Instance.CreateGame(p1, "test game");
            Message msg = GameManager.Instance.CreateNewMessage(p1,p2,"TestMessage",GameConstants.Priority.Normal);

            Assert.IsNotNull(msg, "Test Message was NULL after CreateNewMessage");
            Assert.IsTrue((p1.MessageToPlayer.Count > 0), "MessageToPlayer is not more than 0");
            Assert.IsTrue((p2.MessageFromPlayer.Count > 0), "MessageFromPlayer is not more than 0");
            GameManager.Instance.TerminateGame();
        }

        [TestMethod]
        public void CreateNewMessageFromSystem()
        {
            Player p1 = new Player();
            GameManager.Instance.CreateGame(p1, "test game");
           
            Message msg = GameManager.Instance.CreateNewMessage(p1, "TestMessage", GameConstants.Priority.Normal);

            Assert.IsNotNull(msg, "Test Message was NULL after CreateNewMessage");
            Assert.IsNull(msg.FromPlayer, "FromPlayer was NOT NULL on message from system");
            Assert.IsTrue((p1.MessageToPlayer.Count > 0), "MessageToPlayer is not more than 0");
            GameManager.Instance.TerminateGame();
            
        }
    }
}
