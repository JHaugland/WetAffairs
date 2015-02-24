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
using System.Drawing;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for MapVisulizer.xaml
    /// </summary>
    public partial class HeightTest : Window
    {
        private Bitmap img;
        public HeightTest()
        {
            InitializeComponent();
            img = new Bitmap(@"D:\Prosjekter\TurboTapeGames\Svn\Naval War\NWmanager\NWmanager\imgGfx\hoyde2.jpg");
        }

        private void bgImage1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //  Bitmap hm = new Bitmap(@"D:\Prosjekter\TurboTapeGames\Svn\Naval War\NWmanager\NWmanager\imgGfx\hoyde.jpeg");
            //System.Drawing.Color c = hm.GetPixel(23, 24);
            // System.Windows.Point point = e.GetPosition(bgImage);
            // System.Drawing.Color cc = hm.GetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
            // double height = cc.B * 3.90625;
            // if (cc.R != cc.B)
            // {
            //     height *= -1;
            // }
            // MessageBox.Show("Height : " + height + " R:"+cc.R + " G:"+cc.G+" B:"+cc.B);

        }



        private BitmapImage GetImageFromFile(string FilePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(FilePath);
            bitmap.EndInit();
            return bitmap;
        }

        private void bgImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                System.Windows.Point point = e.GetPosition(bgImage);
                System.Drawing.Color cc = img.GetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));

                //byte Higest; 
                //Higest =
                
                int x = Math.Abs(cc.B - 255);
                double height = x * 3.90625;

                if (cc.R != cc.B)
                {
                    height *= -1;
                }

                lblHight.Content = "Høyde: " + height.ToString() + "M";
                lblRGB.Content = "R:" + cc.R + " G:" + cc.G + " B:" + cc.B + "   X = :" + x;
            }
            catch (Exception)
            {


            }

        }


    }
}
