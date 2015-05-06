using UnityEngine;
using System.Collections;

public class Engine : HoverCraft {
	
	ParticleSystem exhaust;
	Light spotLight;

	public GameObject theLight;
	
	void Start () 
	{
		//get the components
		exhaust = GetComponent<ParticleSystem> ();
		spotLight = theLight.GetComponent<Light> ();
	}
	

	void Update () 
	{
		//play the exhaust particles and light effects upon acceleration
		float movement = Input.GetAxis ("Vertical");

		exhaust.emissionRate = movement * forwardAcceleration;

		//set the light and exhaust fumes to show when the hovercraft is moving
		if(movement > 0.1f)
		{
			spotLight.intensity = 1.0f;
		}
		else 
		{
			spotLight.intensity = 0.0f;
		}


	}
}
