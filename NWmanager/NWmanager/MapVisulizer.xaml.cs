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
using System.Timers;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for MapVisulizer.xaml
    /// </summary>
    public partial class MapVisulizer : Window
    {
        private Game _game;
        private Player _player;

        public MapVisulizer()
        {
            InitializeComponent();
         
            _player = new Player();


            CenterMarker.SetValue(Canvas.TopProperty, (backgr.Height - CenterMarker.Height) / 2);
            CenterMarker.SetValue(Canvas.LeftProperty, (backgr.Width - CenterMarker.Width) / 2);

            _game = GameManager.Instance.CreateGame(_player, "TestGame");
            _game.IsNetworkEnabled = false;
            _game.StartGameLoop();
            Group group = new Group();
            GameManager.Instance.GameData.InitAllData();
            Position pos = new Position(60.25, 5.25);
            BaseUnit unitMain = GameManager.Instance.GameData.CreateUnit(_player, group, "arleighburke", "Arleigh Burke", pos, true);
            unitMain.MovementOrder.AddWaypoint(new Waypoint(new Position(60.5, 5.5)));

            unitMain.ActualSpeedKph = 40.0;
            unitMain.MoveToNewCoordinate(1.0);
            unitMain.Position.BearingDeg = .0;
            //pos = new Position(61.5, 5.5);
            BaseUnit unitFollow = GameManager.Instance.GameData.CreateUnit(_player, group, "arleighburke", "Hood", pos, true);
            BaseUnit unitFollow2 = GameManager.Instance.GameData.CreateUnit(_player, group, "arleighburke", "Hood", pos, true);

            MovementFormationOrder order = new MovementFormationOrder(unitMain, 326000, -326000, 0);
            MovementFormationOrder order2 = new MovementFormationOrder(unitMain, -326000, 326000, 0);
            Waypoint waypoint = order.GetActiveWaypoint();
            Waypoint waypoint2 = order2.GetActiveWaypoint();

            unitFollow.Position = waypoint.Position.Clone();
            unitFollow2.Position = waypoint2.Position.Clone();
            double distanceM = MapHelper.CalculateDistanceM(unitMain.Position.Coordinate, unitFollow.Position.Coordinate);

            ProjCoordinate min = MapProjection.ToEquiProjectedCoordinate(new Coordinate(61.0, 6.0));
            ProjCoordinate max = MapProjection.ToEquiProjectedCoordinate(new Coordinate(62.0, 5));
            Coordinate CenterCoordinate = MapHelper.CalculateMidpoint(new Coordinate(62.0, 5.0), new Coordinate(62.0, 6.0));

            RotateTransform rt = new RotateTransform();
            rt.Angle = (double)unitMain.Position.BearingDeg;
            rt.CenterX = mainShip.Width / 2;
            rt.CenterY = mainShip.Height / 2;

            double testX = MapHelper.LongitudeToXvalue(unitMain.Position.Coordinate.LongitudeDeg, 1);
            double testY = MapHelper.LatitudeToYvalue(unitMain.Position.Coordinate.LatitudeDeg, 1);

            testX = ((512 - 10) / 2) + testX;
            testY = ((512 - 10) / 2) + testY;

            mainShip.SetValue(Canvas.LeftProperty, testX);
            mainShip.SetValue(Canvas.TopProperty, testY);
            mainShip.RenderTransform = rt;

            rt.Angle = (double)waypoint.Position.BearingDeg;
            folowingShip.SetValue(Canvas.LeftProperty, ((512 - 10) / 2) + MapHelper.LongitudeToXvalue(waypoint.Position.Coordinate, 1));
            folowingShip.SetValue(Canvas.TopProperty, ((512 - 10) / 2) + MapHelper.LatitudeToYvalue(waypoint.Position.Coordinate, 1));
            folowingShip.RenderTransform = rt;

            rt.Angle = (double)waypoint2.Position.BearingDeg;
            folowingShip2.SetValue(Canvas.LeftProperty, ((512 - 10) / 2) + MapHelper.LongitudeToXvalue(waypoint2.Position.Coordinate, 1));
            folowingShip2.SetValue(Canvas.TopProperty, ((512 - 10) / 2) + MapHelper.LatitudeToYvalue(waypoint2.Position.Coordinate, 1));
            folowingShip2.RenderTransform = rt;

        }



        private BitmapImage GetImageFromFile(string FilePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(FilePath);
            bitmap.EndInit();
            return bitmap;
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                Movment m = new Movment();
                m.Show();
            }
            if (e.Key == Key.H)
            {
                HeightTest t = new HeightTest();
                t.Show();
            }
        }
    }
}
