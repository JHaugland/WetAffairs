using UnityEngine;
using System.Collections;

public class DrawLineHelper : MonoBehaviour {

    public static Color LastColor;
    public static Texture2D LineTexture;

    public static void DrawLine(Rect rect)
    {
        DrawLine(rect, GUI.contentColor, 1.0f);
    }
    public static void DrawLine(Rect rect, Color color)
    {
        DrawLine(rect, color, 1.0f);
    }
    public static void DrawLine(Rect rect, float width)
    {
        DrawLine(rect, GUI.contentColor, width);
    }    
    public static void DrawLine(Rect rect, Color color, float width)
    {
        DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.xMax, rect.yMax), color, width);
    }
    public static void DrawLine(Vector2 start, Vector2 end)
    {
        DrawLine(start, end, GUI.contentColor, 1.0f);
    }
    public static void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        DrawLine(start, end, color, 1.0f);
    }
    public static void DrawLine(Vector2 start, Vector2 end, float width)
    {
        DrawLine(start, end, GUI.contentColor, width);   
    }
    public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
         Matrix4x4 matrix = GUI.matrix;
    
        // Generate a single pixel texture with the designated color. This will be the color of the line.
        // This looks more complex then it needs to be for optimization.
        //  Instead of regenerating the texture every time, we only do so when the color has changed.

        if (LineTexture == null) 
        { 
            LineTexture = new Texture2D(1, 1); 
        }
        if (color != LastColor) 
        {
            LineTexture.SetPixel(0, 0, color);
            LineTexture.Apply();
            LastColor = color;
        }

        // Determine the angle of the line.
        float angle = Vector3.Angle(end-start, Vector2.right);
        
        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (start.y > end.y) 
        { 
            angle = -angle; 
        }
        
        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((end-start).magnitude, width), new Vector2(start.x, GameManager.Instance.CameraManager.SateliteCamera.pixelHeight - start.y));
        
        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, start);
        
        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(start.x, start.y, 1, 1), LineTexture);
        
        // We're done.  Restore the GUI matrix to whatever it was before.
        GUI.matrix = matrix;
    }





  
}
