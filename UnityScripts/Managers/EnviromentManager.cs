using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class EnviromentManager : MonoBehaviour
{

    #region Privates, consts and enums

    private WeatherSystemInfo _CurrentWeatherSystemInfo;

    #endregion

    #region Private Methods



    #endregion

    #region Script variables

    public float _SunAngle = 0;
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

    public Transform Sun;
    

    public ParticleEmitter Clouds;
    public RotateObject CloudRotater;
    public ParticleEmitter Rain;


    #endregion

    #region Public Properties

    public WeatherSystemInfo CurrentWeather
    {
        get
        {
            return _CurrentWeatherSystemInfo;
        }
        set
        {
            _CurrentWeatherSystemInfo = value;

            //Set weather

            //SetCloudAmount();
            SetRain();

        }
    }

    

    public void SetRain()
    {
        switch (_CurrentWeatherSystemInfo.Precipitation)
        {
            case GameConstants.PrecipitationLevel.Drizzle:
                Rain.minSize = 1;
                Rain.maxSize = 2;
                Rain.minEmission = 100;
                Rain.maxEmission = 200;
                break;
            case GameConstants.PrecipitationLevel.Heavy:
                Rain.minSize = 2;
                Rain.maxSize = 3;
                Rain.minEmission = 1000;
                Rain.maxEmission = 2000;
                break;
            case GameConstants.PrecipitationLevel.Intermediate:
                Rain.minSize = 1.5f;
                Rain.maxSize = 2.5f;
                Rain.minEmission = 600;
                Rain.maxEmission = 800;
                break;
            case GameConstants.PrecipitationLevel.Light:
                Rain.minSize = 1;
                Rain.maxSize = 2;
                Rain.minEmission = 300;
                Rain.maxEmission = 400;
                break;
            case GameConstants.PrecipitationLevel.None:
                Rain.minSize = 0;
                Rain.maxSize = 0;
                Rain.minEmission = 0;
                Rain.maxEmission = 0;
                break;
            default:
                break;
        }
    }

    public void SetCloudAmount()
    {
        int cloudCover = _CurrentWeatherSystemInfo.CloudCover8ths;
        Clouds.emit = false;
        if (cloudCover <= 1)
        //clear
        {

            Clouds.minEmission = 0;
            Clouds.maxEmission = 1;

        }
        else if (cloudCover <= 2)
        {
            Clouds.minEmission = 0;
            Clouds.maxEmission = 10;
        }
        else if (cloudCover <= 5)
        {
            Clouds.minEmission = 10;
            Clouds.maxEmission = 20;
        }
        else if (cloudCover <= 7)
        {
            Clouds.minEmission = 100;
            Clouds.maxEmission = 120;
        }
        else if (cloudCover <= 8)
        {
            Clouds.minEmission = 200;
            Clouds.maxEmission = 250;
        }
        else
        {
            Clouds.minEmission = Mathf.Clamp(cloudCover * 35, 0, 350);
            Clouds.minEmission = Mathf.Clamp(cloudCover * 40, 0, 400);
        }
        Clouds.emit = true;
        CloudRotater.XSpeed = (float)_CurrentWeatherSystemInfo.WindSpeedMSec / 10;
        Clouds.ClearParticles();
        Clouds.Emit();
    }

    public float WaveHeight
    {
        get
        {
            if (_CurrentWeatherSystemInfo != null)
            {
                switch (_CurrentWeatherSystemInfo.WindForceBeaufort)
                {
                    case GameConstants.BeaufortScale.Calm:
                        return 0.1f;

                    case GameConstants.BeaufortScale.LightAir:
                        return 0.2f;
                    case GameConstants.BeaufortScale.LightBreeze:
                        return 0.3f;
                    case GameConstants.BeaufortScale.GentleBreeze:
                        return 0.4f;
                    case GameConstants.BeaufortScale.ModerateBreeze:
                        return 0.5f;
                    case GameConstants.BeaufortScale.FreshBreeze:
                        return 0.6f;
                    case GameConstants.BeaufortScale.StrongBreeze:
                        return 0.7f;
                    case GameConstants.BeaufortScale.NearGale:
                        return 0.8f;
                    case GameConstants.BeaufortScale.Gale:
                        return 0.9f;
                    case GameConstants.BeaufortScale.StrongGale:
                        return 1f;
                    case GameConstants.BeaufortScale.Storm:
                        return 1.1f;
                    case GameConstants.BeaufortScale.ViolentStorm:
                        return 1.2f;
                    case GameConstants.BeaufortScale.Hurricane:
                        return 1.4f;
                    default:
                        return 0.1f;
                }
            }
            return 0.1f;
        }
    }

    public int SeaState
    {
        get
        {
            if (GameManager.Instance.UnitManager.SelectedUnit != null)
            {
                return GameManager.Instance.UnitManager.SelectedUnit.Info.WeatherSystem.SeaState;
            }
            return 4;
        }
    }

    #endregion

    #region Public Methods

    #endregion

    #region Editor properties

    //public Ocean Ocean;

    public float OverrideHeight = 0.0f;
    public skydomeScript2 Skydome;
    public Light NightLight;
    #endregion

    void Update()
    {
        //if (OverrideHeight != 0.0f)
        //{
        //    Ocean.scale = OverrideHeight;
        //}

        //Ocean.sunpower = Sun.light.intensity;
        ////Ocean.scale = WaveHeight;

        ////Ocean.choppy_scale = SeaState;
        ////Ocean.uv_speed = 10;

        //_SunAngle = (_SunAngle + SunSpeed * TimeCompression * Time.deltaTime) % 360;
        ////transform.RotateAround(Vector3.zero, Vector3.left, 20 * Time.deltaTime);

        //Sun.light.intensity = _SunAngle < 90 ?
        //    MathHelper.SuperLerp(MinLightIntensity, MaxLightIntensity, 0.0f, FadeRange, _SunAngle) :
        //    MathHelper.SuperLerp(MaxLightIntensity, MinLightIntensity, 180 - FadeRange, 180, _SunAngle);
        //Sun.light.color = _SunAngle < 90 ?
        //    MathHelper.SuperLerp(SunsetColor, DayColor, 0.0f, FadeRange, _SunAngle) :
        //    MathHelper.SuperLerp(DayColor, SunsetColor, 180 - FadeRange, 180, _SunAngle);
        ////RenderSettings.ambientLight = _SunAngle < 90 ?
        ////    MathHelper.Gray(MathHelper.SuperLerp(MinAmbient, MaxAmbient, 0.0f, FadeRange, _SunAngle)) :
        ////    MathHelper.Gray(MathHelper.SuperLerp(MaxAmbient, MinAmbient, 180 - FadeRange, 180, _SunAngle));
        //RenderSettings.skybox.SetColor("_Tint", _SunAngle < 90 ?
        //    MathHelper.Gray(MathHelper.SuperLerp(MinSkyboxBrightness, MaxSkyboxBrightness, 0.0f, FadeRange, _SunAngle)) :
        //    MathHelper.Gray(MathHelper.SuperLerp(MaxSkyboxBrightness, MinSkyboxBrightness, 180 - FadeRange, 180, _SunAngle)));
        ////light.enabled = _SunAngle < 180;

        //Sun.eulerAngles = new Vector3(_SunAngle, 0, 0);

        if (GameManager.Instance.UnitManager.SelectedUnit != null)
        {
            float hours = GameManager.Instance.GameCurrentTime.Hour;
            float minutes = (float)GameManager.Instance.GameCurrentTime.Minute / 60;
            float seconds = (float)GameManager.Instance.GameCurrentTime.Second / 3600;
            //Skydome.TIME = hours + minutes + seconds;

            //NightLight.enabled = GameManager.Instance.UnitManager.SelectedUnit.Info.WeatherSystem.SunsetTime.Hour < hours;
            //Debug.Log(string.Format("DateTime Hour:{0} - Minute{1} - Second{2}", GameManager.Instance.GameCurrentTime.Hour, GameManager.Instance.GameCurrentTime.Minute, GameManager.Instance.GameCurrentTime.Second));
            
        }


        //~ Debug.Log(Ocean.GetWaterHeightAtLocation(0,0));
    }

    /// <summary>
    /// Place summary here
    /// </summary>

}
