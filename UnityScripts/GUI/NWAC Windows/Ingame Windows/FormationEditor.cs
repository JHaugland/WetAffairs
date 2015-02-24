using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationEditor : DockableWindow {

	public Texture2D FormationGrid;
    private List<PlayerUnit> _Units;
	
	private Rect _FormationRect;
	private float _HeightMod = 0.0f;
    private float _xOffset;
    private float _yOffset;


    public Vector2 CenterPos
    {
        get
        {
            return new Vector2(_FormationRect.width / 2, _FormationRect.height / 2); 
        }
    }

	void Start()
	{
		_HeightMod = GameManager.Instance.GUIManager.MinimizeButtonRect.height + DraggableArea.height;
        _Units = new List<PlayerUnit>();

        _xOffset = Random.Range(-100, 100);
        _yOffset = Random.Range(-100, 100);
	}
	
	void Update()
	{
		_FormationRect = new Rect(this.PaddingLeft, _HeightMod, WindowRect.width - this.PaddingLeft * 2, WindowRect.height - _HeightMod - ResizerSize.y);
        if (SelectedUnitChanged)
        {
            _Units = GameManager.Instance.UnitManager.FindUnitsByGroupId(SelectedUnit.Info.GroupId);
            _xOffset = Random.Range(-CenterPos.x, CenterPos.x);
            _yOffset = Random.Range(-CenterPos.y, CenterPos.y);
        }
	}
	
	public override void DockableWindowFunc(int id)
	{
		
		DoResizing();
		//~ GUI.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		GUI.DrawTexture(_FormationRect, FormationGrid);

		if(_SelectedUnit != null)
		{
            float x = 0;
            float y = 10;
            foreach (PlayerUnit p in _Units)
            {
                Rect pos = new Rect(CenterPos.x + x + _xOffset, CenterPos.y + y + _yOffset, 50, 50);
                if (GUI.Button(pos, GameManager.Instance.GUIManager.BattleCruiserTex))
                {
                    GameManager.Instance.UnitManager.SelectedUnit = p;
                }
                x += 100;
                y += 100;
                //if(GUI.Button(n
            }
		}
	}
		
	
}
