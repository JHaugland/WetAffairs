using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms.Entities;
using System;

public class CarriedUnit
{

    private CarriedUnitInfo _UnitInfo;
    private TimeSpan _TimeSpan;
    public bool Ready = false;
    public bool Selected;
    public bool LaunchOrderAwaiting;
	
	// Update is called once per frame
	public void Update () {
        if (!Ready)
        {
            _TimeSpan = _TimeSpan.Subtract(TimeSpan.FromSeconds((double)Time.deltaTime * GameManager.Instance.GameInfo.RealTimeCompressionFactor));

            if (_TimeSpan.TotalSeconds <= 1)
            {
                Ready = true; ;
                _TimeSpan = TimeSpan.FromSeconds(0);
            }
        }
	}

    public string ReadyInSec
    {
        get
        {
            return Ready == true ? "Now" : string.Format("{0:00}:{1:00}:{2:00}", _TimeSpan.Hours, _TimeSpan.Minutes, _TimeSpan.Seconds);
        }
    }

    public string LoadOut
    {
        get
        {
            return _UnitInfo.CurrentWeaponLoadName;
        }
    }

    public CarriedUnitInfo UnitInfo
    {
        get
        {
            return _UnitInfo;
        }
        set
        {
            _UnitInfo = value;
            _TimeSpan = TimeSpan.FromSeconds(_UnitInfo.ReadyInSec);
            Ready = _UnitInfo.ReadyInSec == 0;
        }
    }
}
