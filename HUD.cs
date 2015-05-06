using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour 
{
	//the speed HUD
	Text speedHUD;

	//the hovercraft and it's rigidbody
	public GameObject hovercraft;
	Rigidbody body;


	void Start ()
	{
		speedHUD = GetComponent<Text>();
		body = hovercraft.GetComponent<Rigidbody> ();
	}
	

	void Update ()
	{
		//calculate mph using velocity and set value to hud
		float mph = (float)(body.velocity.magnitude * 2.237);
		speedHUD.text = mph.ToString("F0") + "  MPH";
	}
}
