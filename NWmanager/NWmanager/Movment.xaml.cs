using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Util;
using System.Windows.Forms;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for MapVisulizer.xaml
    /// </summary>
    public partial class Movment : Window
    {
        private Game _game;
        private Player _player;
        private Timer _timer;
        private MovementFormationOrder order;
        private Waypoint waypoint;
        private Image mb;
        private Image fs;
        public Movment()
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = 500;
            _player = new Player();
            mb = mainShip;
            fs = folowingShip;
            _game = GameManager.Instance.CreateGame(_player, "TestGame");
            _game.IsNetworkEnabled = false;
            _game.StartGameLoop();
            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Position pos = new Position(60.25, 10.25);
            BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(_player, group, "arleighburke", "Arleigh Burke", pos, true);
            unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(-10.25, 0.25)));

            unitMain.ActualSpeedKph = 700.0;
            unitMain.MoveToNewCoordinate(1.0);

            BaseUnit unitFollow = GameManager.Instance.GameData.CreateUnit(_player, group, "arleighburke", "Hood", pos, true);
            order = new MovementFormationOrder(unitMain, 326000, -326000, 0);
            waypoint = order.GetActiveWaypoint();
            unitFollow.Position = waypoint.Position.Clone();

            _timer.Start();

        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _game.Tick(1.0);
            _timer.Stop();
            UpdateMap();
        }

        void UpdateMap()
        {
            foreach (Player p in _game.Players)
            {
                foreach (BaseUnit u in p.Units)
                {
                    if (u.Name == "Arleigh Burke")
                    {
                        u.MoveToNewCoordinate(2000);
                        RotateTransform rt = new RotateTransform();
                        rt.Angle = (double)u.Position.BearingDeg;
                        rt.CenterX = mb.Width / 2;
                        rt.CenterY = mb.Height / 2;

                        double testX = MapHelper.LongitudeToXvalue(u.Position.Coordinate.LongitudeDeg, 1);
                        double testY = MapHelper.LatitudeToYvalue(u.Position.Coordinate.LatitudeDeg, 1);

                        testX = ((512 - 10) / 2) + testX;
                        testY = ((512 - 10) / 2) + testY;

                        mb.SetValue(Canvas.LeftProperty, testX);
                        mb.SetValue(Canvas.TopProperty, testY);
                        mb.RenderTransform = rt;
                    }
                    else if(u.Name == "Hood")
                    {
                        waypoint = order.GetActiveWaypoint();
                        u.Position = waypoint.Position.Clone();
                        u.MoveToNewCoordinate(2000);
                        RotateTransform rt = new RotateTransform();
                        rt.Angle = (double)u.Position.BearingDeg;
                        rt.CenterX = mb.Width / 2;
                        rt.CenterY = mb.Height / 2;

                        double testX = MapHelper.LongitudeToXvalue(u.Position.Coordinate.LongitudeDeg, 1);
                        double testY = MapHelper.LatitudeToYvalue(u.Position.Coordinate.LatitudeDeg, 1);

                        testX = ((512 - 10) / 2) + testX;
                        testY = ((512 - 10) / 2) + testY;

                        fs.SetValue(Canvas.LeftProperty, testX);
                        fs.SetValue(Canvas.TopProperty, testY);
                        fs.RenderTransform = rt;
                    }
                }
            }
            _timer.Start();
        }

        private BitmapImage GetImageFromFile(string FilePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(FilePath);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
