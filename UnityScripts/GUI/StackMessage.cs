using UnityEngine;
using System.Collections;

public class StackMessage {

    public Message MessageToDisplay;

    private float _LifeTime = 0;
    private float _KillTime;
    private bool _Dead;

    public StackMessage()
    {
    }

    public StackMessage(Message message, float killTime)
    {
        MessageToDisplay = message;
        _KillTime = killTime;
    }
	// Update is called once per frame
	public void Update () {
        _LifeTime += Time.deltaTime;

        if (_LifeTime >= _KillTime)
        {
            Kill();
        }
	}

    public bool Dead
    {
        get
        {
            return _Dead;
        }
        private set
        {
            _Dead = value;
        }
    }

    private void Kill()
    {
        //Set dead
        Dead = true;
    }
}
