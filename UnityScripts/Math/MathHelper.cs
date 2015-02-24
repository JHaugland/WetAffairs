using UnityEngine;
using System;

public class MathHelper : MonoBehaviour
{

    public static float METERS_TO_NAUTICAL_MILES = 1852;
    public static float METERS_TO_KILOMETERS = 1000;
    public static float METERS_TO_FEET = 3.28083f;

    public static Vector2 Vector2Lerp(Vector2 v1, Vector2 v2, float pos)
    {
        return new Vector2(Mathf.Lerp(v1.x, v2.x, pos),
                    Mathf.Lerp(v1.y, v2.y, pos));
    }

    public static Rect ViewportRectToScreenRect(Rect viewportRect)
    {
        return new Rect(viewportRect.x * Screen.width, viewportRect.y * Screen.height,
            viewportRect.width * Screen.width, viewportRect.height * Screen.height);
    }

    public static void SetLayerRecursivly(Transform t, int layer)
    {
        foreach ( Transform child in t )
        {
            SetLayerRecursivly(child, layer);
            child.gameObject.layer = layer;
        }
        t.gameObject.layer = layer;
    }

    public static float MetersToUnitOfLength(float meters, GameManager.UnitOfLength unitlength, out string shortString)
    {
        switch (unitlength)
        {
            case GameManager.UnitOfLength.Kilometer:
                shortString = "Km";
                return meters / METERS_TO_KILOMETERS;
                break;
            case GameManager.UnitOfLength.NauticalMiles:
                shortString = "Nm";
                return meters / METERS_TO_NAUTICAL_MILES;
                break;
            case GameManager.UnitOfLength.Feet:
                shortString = "ft";
                return meters / METERS_TO_FEET;
                break;
            default:
                shortString = "";
                return 0;
                break;
        }
    }


    public static Rect ScreenRectToViewportRect(Rect screenRect)
    {

        return new Rect(screenRect.xMin / Screen.width, screenRect.yMin / Screen.height,
            screenRect.width / Screen.width, screenRect.height / Screen.height);
    }

    public static Rect GetMouseGUIRect(Camera cam, float width, float height, float xOffset, float yOffset)
    {
        return new Rect(Input.mousePosition.x + xOffset, cam.pixelHeight - Input.mousePosition.y + yOffset, width, height);
    }

    public static float SuperLerp(float a, float b, float c, float d, float t)
    {
        return Mathf.Lerp(a, b, Mathf.InverseLerp(c, d, t));
    }

    public static Color SuperLerp(Color a, Color b, float c, float d, float t)
    {
        return Color.Lerp(a, b, Mathf.InverseLerp(c, d, t));
    }

    public static Color Gray(float t)
    {
        return new Color(t, t, t);
    }

    public static Vector3 ClampVector(Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
    }

    public static float Clamp360(float value)
    {
        if (value > 360)
        {
            return value - 360;
        }
        if (value < 0)
        {
            return value + 360;
        }
        return value;
    }

    public static Vector2 MousepositionToGUICoord(Vector3 mousePosition)
    {
        mousePosition.y = Screen.height - mousePosition.y;

        return mousePosition;
    }

    public static Vector2 ActualFormationPositionToGUIPosition(Vector2 formationPos)
    {
        //float xFac = (float)FormationPosition.PositionOffset.RightM * GameManager.Instance.GUIManager.FormationFactorX;
        //float yFac = (float)FormationPosition.PositionOffset.ForwardM * GameManager.Instance.GUIManager.FormationFactorY;
        ////Debug.Log(string.Format("x : {0} - - yFac: {1}", xFac, yFac));
        //xFac += GameManager.Instance.GUIManager.FormationRectSize.x / 2;
        //yFac += GameManager.Instance.GUIManager.FormationRectSize.x / 2;


        float xFac = formationPos.x * GameManager.Instance.GUIManager.FormationFactorX;
        float yFac = formationPos.y * GameManager.Instance.GUIManager.FormationFactorY;
        //Debug.Log(string.Format("x : {0} - - yFac: {1}", xFac, yFac));
        xFac += GameManager.Instance.GUIManager.FormationRectSize.x / 2;
        yFac += GameManager.Instance.GUIManager.FormationRectSize.x / 2;


        return new Vector2(xFac, yFac);
    }

    public static Vector2 GUIPositionToActualFormationPosition(Vector2 guiPos)
    {
        guiPos.x -= GameManager.Instance.GUIManager.FormationRectSize.x / 2;
        guiPos.y -= GameManager.Instance.GUIManager.FormationRectSize.y / 2;
        guiPos.x /= GameManager.Instance.GUIManager.FormationFactorX;
        guiPos.y /= GameManager.Instance.GUIManager.FormationFactorY;

        return guiPos;
    }


    public static int GetNumericId(string id)
    {
        char[] chars = id.ToCharArray();

        int counter = 0;

        int sum = 0; ;
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            sum += (Convert.ToInt32(chars[i]) - 64) *
                            (int)Math.Pow(26, counter);
            counter++;
        }
        return sum;
    }


    public static bool MouseOverGUI(Vector2 mousePos, Rect rect)
    {
        Vector2 guiMousePos = new Vector2(mousePos.x, Screen.height - mousePos.y);
        if (rect.Contains(guiMousePos))
        {
            return true;
        }
        return false;

    }




}
