using UnityEngine;
using System.Collections;

public class AnimationLauncher : MonoBehaviour
{

    public enum AnimMode
    {
        FixedWingTakeOff,
        FixedWingLanding,
        HelicopterTakeOff,
        HelicopterLanding
    }

    public AnimMode TakeOffMode = AnimMode.FixedWingTakeOff;


    public Animation HeliAnim;
    public Animation FixedWingAnim;
    public Animation CurrentAnim;
    public string FixedWingTakeOffName;
    public string HelicopterTakeOffName;
    public string FixedWingLandingName;
    public string HelicopterLandingName;
    public string CurrentAnimation;
    public float NormTime = 0;
    public GameObject Aircraft;
    public GameObject Helo;

    public bool PlayAutomatically;
    public bool _IsPlaying;

    public void LaunchAnimation()
    {

        SetObjectVisibility(true);

        switch ( TakeOffMode )
        {
            case AnimMode.FixedWingTakeOff:
                CurrentAnimation = FixedWingTakeOffName;
                CurrentAnim = FixedWingAnim;
                CurrentAnim.Play(FixedWingTakeOffName);
                break;
            case AnimMode.HelicopterTakeOff:
                CurrentAnimation = HelicopterTakeOffName;
                CurrentAnim = HeliAnim;
                CurrentAnim.Play(HelicopterTakeOffName);
                break;
            case AnimMode.FixedWingLanding:
                CurrentAnimation = FixedWingLandingName;
                CurrentAnim = FixedWingAnim;
                CurrentAnim.Play(FixedWingLandingName);
                break;
            case AnimMode.HelicopterLanding:
                CurrentAnimation = HelicopterLandingName;
                CurrentAnim = HeliAnim;
                CurrentAnim.Play(HelicopterLandingName);
                break;
            default:
                break;
        }
        Debug.Log("playing animation " + CurrentAnimation);
        _IsPlaying = true;
    }

    void Start()
    {
        switch ( TakeOffMode )
        {
            case AnimMode.FixedWingTakeOff:
                CurrentAnimation = FixedWingTakeOffName;
                CurrentAnim = FixedWingAnim;
                break;
            case AnimMode.HelicopterTakeOff:
                CurrentAnimation = HelicopterTakeOffName;
                CurrentAnim = HeliAnim;
                break;
            case AnimMode.FixedWingLanding:
                CurrentAnimation = FixedWingLandingName;
                CurrentAnim = FixedWingAnim;
                break;
            case AnimMode.HelicopterLanding:
                CurrentAnimation = HelicopterLandingName;
                CurrentAnim = HeliAnim;
                break;
            default:
                break;
        }
        SetObjectVisibility(false);
        if ( PlayAutomatically )
        {
            LaunchAnimation();
        }
    }

    void Update()
    {
        if ( CurrentAnim != null )
        {
            if ( Input.GetKeyUp(KeyCode.K) && !_IsPlaying )
            {
                LaunchAnimation();
                Debug.Log("Time " + Time.time);
            }

            if ( !_IsPlaying && !CurrentAnim.isPlaying )
            {
                CurrentAnim.Stop(CurrentAnimation);
            }

            if ( !CurrentAnim.isPlaying && _IsPlaying )
            {
                SetObjectVisibility(false);
                CurrentAnim.Rewind();
                _IsPlaying = false;
            }
        }

        //NormTime = Anim.gameObject.animation[CurrentAnimation].normalizedTime;

        //if ( Anim.gameObject.animation[CurrentAnimation] != null )
        //{
            //if ( !_IsPlaying )
            //{
            //    Anim.Stop(CurrentAnimation);
            //}
            //if ( Anim[CurrentAnimation].normalizedTime >= 0.95f )
            //{
            //    SetObjectVisibility(false);
            //    Anim.Rewind();
            //    _IsPlaying = false;

            //}
        //    Debug.Log(CurrentAnimation + " is not null");
        //}
    }

    void SetObjectVisibility(bool visible)
    {
        if ( !visible )
        {
            if ( Aircraft != null )
            {
                Aircraft.SetActiveRecursively(false);
            }
            if ( Helo != null )
            {
                Helo.SetActiveRecursively(false);
            }

            return;
        }


        switch ( TakeOffMode )
        {
            case AnimMode.FixedWingTakeOff:
            case AnimMode.FixedWingLanding:
                if ( Aircraft != null )
                {
                    Aircraft.SetActiveRecursively(true);
                }
                if ( Helo != null )
                {
                    Helo.SetActiveRecursively(false);
                }
                break;
            case AnimMode.HelicopterTakeOff:
            case AnimMode.HelicopterLanding:
                if ( Aircraft != null )
                {
                    Aircraft.SetActiveRecursively(false);
                }
                if ( Helo != null )
                {
                    Helo.SetActiveRecursively(true);
                }
                break;
            default:
                break;
        }

    }

}
