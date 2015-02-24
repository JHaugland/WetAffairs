using UnityEngine;
using System.Collections;

public class KillTimer : MonoBehaviour {

	public float m_SecondsBeforeDestruction = 1.0f;
	private float m_secondsElapsed = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		m_secondsElapsed += Time.deltaTime;
		
		if(m_secondsElapsed >= m_SecondsBeforeDestruction)
		{
			Destroy(gameObject);
		}
		
		
	}
}
