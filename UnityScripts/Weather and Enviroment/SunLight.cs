using UnityEngine;
using System.Collections;

public class SunLight : MonoBehaviour {

    private float _SunAngle = 0;
    public float SunSpeed = 2;
    public float FadeRange = 35;
    public float MaxLightIntensity = 6;
    public float MinLightIntensity = 0;
    public float MaxAmbient = 0.45f;
    public float MinAmbient = 0.15f;
    public float MaxSkyboxBrightness = 1;
    public float MinSkyboxBrightness = 0.1f;
    public Color SunsetColor;
    public Color DayColor;
    public float TimeCompression = 1;
    
	
	// Update is called once per frame
	void Update () {

        _SunAngle = (_SunAngle + SunSpeed * TimeCompression * Time.deltaTime) % 360;
        transform.RotateAround(Vector3.zero, Vector3.left, 20 * Time.deltaTime);

        light.intensity = _SunAngle < 90 ? 
            MathHelper.SuperLerp(MinLightIntensity, MaxLightIntensity, 0.0f, FadeRange, _SunAngle) :
            MathHelper.SuperLerp(MaxLightIntensity, MinLightIntensity, 180 - FadeRange, 180, _SunAngle);
        light.color = _SunAngle < 90 ?
            MathHelper.SuperLerp(SunsetColor, DayColor, 0.0f, FadeRange, _SunAngle) :
            MathHelper.SuperLerp(DayColor, SunsetColor, 180 - FadeRange, 180, _SunAngle);
        RenderSettings.ambientLight = _SunAngle < 90 ?
            MathHelper.Gray(MathHelper.SuperLerp(MinAmbient, MaxAmbient, 0.0f, FadeRange, _SunAngle)) :
            MathHelper.Gray(MathHelper.SuperLerp(MaxAmbient, MinAmbient, 180 - FadeRange, 180, _SunAngle));
        RenderSettings.skybox.SetColor("_Tint", _SunAngle < 90 ?
            MathHelper.Gray(MathHelper.SuperLerp(MinSkyboxBrightness, MaxSkyboxBrightness, 0.0f, FadeRange, _SunAngle)) :
            MathHelper.Gray(MathHelper.SuperLerp(MaxSkyboxBrightness, MinSkyboxBrightness, 180 - FadeRange, 180, _SunAngle)));
        //light.enabled = _SunAngle < 180;

        transform.eulerAngles = new Vector3(_SunAngle, 0, 0);



	}


}
