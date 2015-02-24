using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms.Entities;

[AddComponentMenu("Rendering/Fog Layer")]
[RequireComponent(typeof(Camera))]
public class FogLayer : MonoBehaviour
{
    private bool _RevertFogState = false;

    public bool FogEnabled = false;

    void OnPreRender()
    {
        _RevertFogState = RenderSettings.fog;
        RenderSettings.fog = FogEnabled;
    }

    void OnPostRender()
    {
        RenderSettings.fog = _RevertFogState;
    }
}
