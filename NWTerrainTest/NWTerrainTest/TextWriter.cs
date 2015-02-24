using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NWTerrainTest
{
    class TextWriter
    {
        

     public void WriteText(string str, string FontName, float FontSize, FontStyle _FontStyle, Brush StringColour, float BitmapXPos, float BitmapYPos, ref Texture2D Texture1, ref Vector2 Vector1)
     {
        
         Font NewFont = new Font(FontName, FontSize, _FontStyle);
         // The font that the text will be written in
         Graphics Graphics = System.Drawing.Graphics.FromHwnd();
         // The Graphics object that will
         //write the text onto the bitmap
         SizeF StringSize = Graphics.MeasureString(str, NewFont);
         //The length of the text
         Bitmap Bitmap = new Bitmap((int)StringSize.Width, (int)StringSize.Height);
         // the bitmap that will hold the text
         Graphics = System.Drawing.Graphics.FromImage(Bitmap);
         //tell the graphics object that it will be using the bitmap
         Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit;
         // and how it will display the text
         Graphics.DrawString(str, NewFont, StringColour, 0, 0);
         // Draw the string on the bitmap
         System.IO.MemoryStream MemStream = new System.IO.MemoryStream();
         //set aside a portion of memory to hold the bitmap
         Bitmap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
         // save the bitmap to the portion of memory
         MemStream.Position = 0;
         // dont know what this does, but it is necessary
         Texture1 = Texture2D.FromFile(Device, MemStream);
         // create a texture to be used in the spritebatch from the
         Vector1 = new Vector2(BitmapXPos, BitmapYPos);
         // set the position of the texture
        
     }




    }
}
