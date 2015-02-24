var sunSpeed : float = 2;
private var sunAngle : float = 0;
var fadeRange : float = 35;
var maxLightIntensity : float = .6;
var minLightIntensity : float = 0;
var maxAmbient : float = .45;
var minAmbient : float = .15;
var maxSkyboxBrightness : float = 1;
var minSkyboxBrightness : float = .1;
var sunsetColor : Color;
var dayColor : Color;
private var speedMultiply : float;

function Update () {
	speedMultiply = Input.GetKey("f")? 20 : 1;
	
	sunAngle = (sunAngle + sunSpeed*speedMultiply*Time.deltaTime) % 360;
	light.intensity = sunAngle < 90?
		SuperLerp (minLightIntensity, maxLightIntensity, 0.0, fadeRange, sunAngle)
		: SuperLerp (maxLightIntensity, minLightIntensity, 180.0-fadeRange, 180.0, sunAngle);
	light.color = sunAngle < 90?
		SuperColorLerp (sunsetColor, dayColor, 0.0, fadeRange, sunAngle)
		: SuperColorLerp (dayColor, sunsetColor, 180.0-fadeRange, 180.0, sunAngle);
	RenderSettings.ambientLight = sunAngle < 90?
		Gray (SuperLerp (minAmbient, maxAmbient, 0.0, fadeRange, sunAngle))
		: Gray (SuperLerp (maxAmbient, minAmbient, 180.0-fadeRange, 180.0, sunAngle));
	RenderSettings.skybox.SetColor("_Tint", sunAngle < 90?
		Gray (SuperLerp (minSkyboxBrightness, maxSkyboxBrightness, 0.0, fadeRange, sunAngle))
		: Gray (SuperLerp (maxSkyboxBrightness, minSkyboxBrightness, 180.0-fadeRange, 180.0, sunAngle))
	);
	light.enabled = sunAngle < 180? true : false;
	
	transform.eulerAngles = Vector3(sunAngle, 0, 0);
}

function SuperLerp (a : float, b : float, c : float, d : float, t : float) : float {
	return Mathf.Lerp(a, b, Mathf.InverseLerp(c, d, t));
}

function SuperColorLerp (a : Color, b : Color, c : float, d : float, t : float) : Color {
	return Color.Lerp(a, b, Mathf.InverseLerp(c, d, t));
}

function Gray (t : float) : Color {
	return Color(t, t, t);
}