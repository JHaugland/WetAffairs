// ParticleLayer.js

var layerheight = 0.00;

private var position : Vector3;
position = transform.localPosition;

function Update ()
{
	object = Camera.main.gameObject;

	vector = object.transform.position - transform.position;

	if(transform.parent && (vector.magnitude > layerheight || layerheight < 0.00) )
	{
		transform.position = transform.parent.TransformPoint(position) + vector.normalized * layerheight;
	}
}