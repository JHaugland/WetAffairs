using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    private float _musicVolume = 0.5f;
    private float _soundEffectsVolume = 0.5f;
    private float _comChatterVolume = 0.5f;

    private JukeboxController _music = null;
    private JukeboxController _sound = null;
    private JukeboxController _comChatter = null;

    private List<AudioSource> _Sounds;



    #region Public Properties
    public float MusicVolume
    {
        get
        {
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;
        }
    }
    public float SoundEffectsVolume
    {
        get
        {
            return _soundEffectsVolume;
        }
        set
        {
            _soundEffectsVolume = value;
        }
    }
    public float ComChatterVolume
    {
        get
        {
            return _comChatterVolume;
        }
        set
        {
            _comChatterVolume = value;
        }
    }



    #endregion

    #region SoundControllers
    public JukeboxController MusicController
    {
        get
        {
            if ( _music == null )
            {
                _music = new JukeboxController();
                if ( _music == null )
                    Debug.Log(@"Could not locate an AManager object. \
											You have to have exactly one AManager in the scene.");
            }

            return _music;
        }
    }

    public JukeboxController SoundEffectsController
    {
        get
        {
            if ( _sound == null )
            {
                _sound = new JukeboxController();
                if ( _sound == null )
                    Debug.Log(@"Could not locate an AManager object. \
											You have to have exactly one AManager in the scene.");
            }

            return _sound;
        }
    }

    public JukeboxController ComChatterController
    {
        get
        {
            if ( _comChatter == null )
            {
                _comChatter = new JukeboxController();
                if ( _comChatter == null )
                    Debug.Log(@"Could not locate an AManager object. \
											You have to have exactly one AManager in the scene.");
            }

            return _comChatter;
        }
    }
    #endregion

    #region Script Variables

    public AudioClip BackgroundMusic;
    public AudioSource GUISounds;

    public AudioClip ButtonClick;


    #endregion
 void Awake()
    {

        //~ DontDestroyOnLoad(this);
        _music = new JukeboxController();
        _sound = new JukeboxController();
        _comChatter = new JukeboxController();
    }

    void Start()
    {
        _music.Volume = _musicVolume;
        _sound.Volume = _soundEffectsVolume;
        _comChatter.Volume = _comChatterVolume;


    }

    void Update()
    {
        if ( _music != null )
        {
            _music.Update();
        }
    }

    public void AddSound(AudioSource audioSource)
    {
        _Sounds.Add(audioSource);
    }

    public void PlayClickButtonSound()
    {
        GUISounds.PlayOneShot(ButtonClick);
    }

    public void Mute(bool mute)
    {

        foreach ( AudioSource audioSource in _Sounds )
        {
            if ( mute )
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
        }
    }





}