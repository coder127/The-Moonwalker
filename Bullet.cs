using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Rigidbody projectile;
	AudioSource bulletSound;
	GameObject theCollided;

	float speed = 500.0f;

	void Start () 
	{
		bulletSound = GetComponent<AudioSource> ();
	}

	void makeClone()
	{
		//instantiate a bullet (capsule) at the position specfied and apply forces forward
		Rigidbody clone = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;
		clone.velocity = transform.TransformDirection(new Vector3 (0, -speed, 0));
		clone.AddForce(transform.TransformDirection(Vector3.right));
		bulletSound.Play ();
		clone.tag = "Bullet";

		//destroy the bullet after 3 seconds
		Destroy(clone.gameObject, 3.0f);
	}

	void Update () {
	
		//shoot when space pressed
		if(Input.GetKey("space"))
		{
			makeClone();
		}
	
	}
	
}
