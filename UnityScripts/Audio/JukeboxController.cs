using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    Usage:
    JukeboxController jukebox = new JukeboxController();
    jukebox.AddClip("mysong", myclip);
    jukebox.PlayClip("mysong");
    jukebox.StopClip();

    */

public class JukeboxController {

    Hashtable jukebox;
    string current_clip;
	
	private bool _loopCurrent = false;
	private bool _loopAll = false;
	private bool _shuffle = false;
	private bool _isPlaying = false;
	
	private int _currentIndex = 0;
	private List<GameObject> _jukeBox;
	
	
	#region Public Properties
	
	public List<GameObject> JukeBox
	{
		get
		{
			return _jukeBox;
		}
		set
		{
			JukeBox = value;
		}
	}
	
	public bool LoopCurrent
	{
		set
		{
			_loopCurrent = value;
			_loopAll = false;
		}
	}
	public bool LoopAll
	{
		set
		{
			_loopAll = value;
			_loopCurrent = false;
		}
	}
	public bool Shuffle
	{
		set
		{
			_shuffle = value;
		}
	}
	public int CurrentIndex
	{
		get
		{
			return _currentIndex;
		}
		set
		{
			StopClip();
			_currentIndex = value;
			if(_currentIndex >= JukeBox.Count)
			{
				_currentIndex = 0;
			}
			if(_currentIndex < 0)
			{
				_currentIndex = JukeBox.Count  - 1;
			}
		}
	}
	
	
	
	
	
	public float Volume
	{
		set
		{
			for(int i = 0 ; i < JukeBox.Count ; ++i)
			{
				GameObject ac =  JukeBox[i] as GameObject;
				if(ac)
				{
					ac.audio.volume = Mathf.Clamp(value, 0, 1);
				}
			}
		}
	}
	
	#endregion

    public JukeboxController()
    {
		_jukeBox = new List<GameObject>();
        //~ jukebox = new Hashtable();
        //~ current_clip = null;
    } // constructor

    public void AddClip(AudioClip clip)
    {
        GameObject obj;
        obj = new GameObject();
        obj.AddComponent("AudioSource");
        obj.audio.clip = clip;
		obj.audio.playOnAwake = false;
		//~ obj.audio.volume = Volume;
        obj.audio.ignoreListenerVolume = true;
        //~ Object.DontDestroyOnLoad(obj);
        JukeBox.Add(obj);
    } // AddClip()
	
    /*
        Play a named audio clip.
        Does not restart the clip if it is played twice in a row.
        Will stop a previously playing clip to play this new clip.
    */
    public void Play(int index)
    {
		Debug.Log(CurrentIndex);
        if (JukeBox[CurrentIndex] != null)
        {
            StopClip();
        } // if
       
        ((GameObject)JukeBox[index]).audio.Play();
    } // PlayClip()
   
	public void Pause()
	{
		if (JukeBox[CurrentIndex] != null)
        {
            ((GameObject)JukeBox[CurrentIndex]).audio.Pause();
        }
	}
	
	public void Resume()
	{
		if (JukeBox[CurrentIndex] != null)
        {
			Play(CurrentIndex);
        }
	}
	
	public void Next()
	{
		Play(CurrentIndex++);
	}
	
	public void Previous()
	{
		Play(CurrentIndex--);
	}
	
    public void StopClip()
    {
         ((GameObject)JukeBox[CurrentIndex]).audio.Stop();
    } // StopClip()
	
	
	
	public void Update()
	{
		if(_isPlaying)
		{
			 if(!((GameObject)JukeBox[CurrentIndex]).audio.isPlaying)
			 {
				 
			 }
		}
	}

}