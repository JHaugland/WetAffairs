using UnityEngine;
using System.Collections;

public class CompassWindow : DockableWindow
{

    public override void DockableWindowFunc(int id)
    {
        DoResizing();
        GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);




    }


	
}
