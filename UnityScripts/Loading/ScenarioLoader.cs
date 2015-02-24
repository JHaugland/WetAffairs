using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class ScenarioLoader : MonoBehaviour
{
    private GameScenario _scenario;


    void Start()
    {
        _scenario = GameManager.Instance.ScenarioLoaded;

        if (_scenario != null)
        {
            //Add all units to scene
        }

    }

    

}
