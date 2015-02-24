using UnityEngine;
using System.Collections;

public class OrbitCamNWAC : MonoBehaviour {
	
	public Transform target ;
	public float distance = 10.0f;

	public float  xSpeed = 250.0f;
	public float  ySpeed = 120.0f;
	public float zoomSpeed = 10.0f;
	
	public float m_stiffness = 100f;
	public float m_damping = 10f;
	public float m_mass = 1f;
	public float m_velocity = 0.0f;

	public float yMinLimit = -20;
	public float yMaxLimit = 80;
	public float defaultDistance = 300;

	private float x = 0.0f;
	private float y = 0.0f;
	
	public float m_MinDistance = 100;
	public float m_MaxDistance = 500;
	
	public GameObject CameraController;
	
	public bool IsStandardMode = true;
	
	
	public Transform Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
			distance = defaultDistance;
			DoMove(Vector3.zero);
		}
	}
	// Use this for initialization
	void Start () {
		
			//~ if(!target)
			//~ {
				//~ target = this.transform;
			//~ }
			if(target)
			{
				Vector3 angles = transform.eulerAngles;
				x = angles.y;
				y = angles.x;

				// Make the rigid body not change rotation
				if (rigidbody)
					rigidbody.freezeRotation = true;
				
				x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
				
				y = ClampAngle(y, yMinLimit, yMaxLimit);
					   
				Quaternion rotation = Quaternion.Euler(y, x, 0);
				Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				
				transform.rotation = rotation;
				transform.position = position;
			}
	}
	
	void Update()
	{
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			if(!target)
			{
				target = GameManager.Instance.UnitManager.SelectedUnit.transform;
			}
			DoMove(Vector2.zero);
		}
		//~ DoMove(Vector2.zero);
		//~ if(GameManager.Instance.UnitManager.SelectedUnit == null)
		//~ {
			//~ GameManager.Instance.CameraManager.SwitchCamera(GameManager.Instance.CameraManager.SateliteCamera);
		//~ }
	}
		
	
	public void Move(Vector2 moveDirection)
	{
		DoMove(moveDirection);
	}
	
	public void Zoom(float deltaValue)
	{
		DoZoom(deltaValue);
	}
	
	private void DoMove(Vector2 moveDirection)
	{
		
		 if (target ){//&& (gameObject.GetComponent(typeof(Camera)) as Camera).enabled) {

				x += moveDirection.x * xSpeed * 0.02f;
				y -= moveDirection.y * ySpeed * 0.02f;
				
				y = ClampAngle(y, yMinLimit, yMaxLimit);
					   
				Quaternion rotation = Quaternion.Euler(y, x, 0);
				Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				
				transform.rotation = rotation;
				transform.position = position;
		}
	
	}
	
	private void DoZoom(float deltaValue)
	{
		if(target != null)
		{
			float force = -m_stiffness * deltaValue - m_damping * m_velocity;

				// Apply acceleration
			float acceleration = force / m_mass;
			m_velocity += acceleration * Time.deltaTime;
			 
			distance += m_velocity * zoomSpeed ;
			 distance = Mathf.Clamp(distance, m_MinDistance, m_MaxDistance);
			
			 if(deltaValue != 0 || m_velocity != 0.0f)
			 {
				 Vector3 position = transform.rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				 transform.position = position;
			 }	
		 }
		 else
		 {
			 //~ GameManager.Instance.CameraManager.SwitchCamera(GameManager.Instance.CameraManager.SateliteCamera);
		 }
	}
	
	
	float ClampAngle (float angle , float  min, float max) 
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}


