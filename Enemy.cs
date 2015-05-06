using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	Rigidbody body; //the rigidbody of enemy
	BoxCollider boxCollider; //the collider of the enemy

	//Forward
 	public float forwardAcceleration = 100.0f;
	public float backwardAcceleration = 25.0f;
	float thrust = 0.0f;
	
	//Turning
	public float turnPower = 10.0f;

	//Hovering
	public float hoverForce = 9.0f;
	public float hoverHeight = 2.0f;
	public float damping; //for damping movement
	
	//Hover Points
	public GameObject[] hoverPoints;
	int layerMask;

	//the direction to turn to
	Vector3 direction;
	bool collisionIncoming = false; //is there collision incoming

	//health
	public Slider healthBarSlider;
	public Text healthText;
	public GameObject explosion;
	int startingHealth = 100;
	float currentHealth;
	float damage = 0.1f;
	AudioSource deathSound;

	// Use this for initialization
	void Start () 
	{
		body = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider> ();
		deathSound = GetComponent<AudioSource> ();

		//set the starting health
		currentHealth = (float)startingHealth;

		//set a layer mask to ignore when casting rays
		layerMask = 1 << LayerMask.NameToLayer ("Enemy");
		layerMask = ~layerMask;
	}

	public void takeDamage(float amount)
	{
		//set the health after damage taken
		currentHealth -= amount;
		healthBarSlider.value -= amount;

		//destroy when dead
		if(currentHealth <= 0)
		{
			death ();
			Destroy(gameObject);
		}

	}

	void calculateHealth()
	{
		//use current health figure to display enemy health on hud
		healthText.text = "Enemy Health: " + (int)currentHealth;

	}

	void death()
	{
		//destroy object
		boxCollider.isTrigger = true;
		deathSound.Play ();
		Instantiate (explosion, transform.position, transform.rotation);
	}

	void randomMovement()
	{
		//have to set local coordinates of the game object as the imported mesh has messed up pivot points
		direction.x = transform.up.y;
		direction.y = transform.forward.z;
		direction.z = transform.right.x;

		//raycast ahead (transform.right is forward) (I know, I got peed off with all these directions changes too :P).
		Vector3 ahead = transform.right;
		float rayLength = 150.0f;
		RaycastHit hit;
		ahead = ahead.normalized;

		//cast a ray ahead
		if(Physics.Raycast(transform.position, ahead, out hit, rayLength))
		{
			//turn in correspondance to normal to what was hit if its a wall
			if(hit.transform.gameObject.tag == "Wall")
			{
				collisionIncoming = true;
				Vector3 normal = hit.normal;
				direction += normal * 50;				
			}
		}
		//don't go upwards
		direction.y = 0.0f; 
	}
	void Update()
	{
		calculateHealth ();
	}

	//fixed update for physics
	void FixedUpdate()
	{
		//Hovering
		RaycastHit hit;
		for (int i = 0; i < hoverPoints.Length; i++)
		{
			GameObject hoverPoint = hoverPoints [i];
			if (Physics.Raycast (hoverPoint.transform.position, transform.TransformDirection(Vector3.back), out hit, 
			                     hoverHeight, layerMask))
			{
				if(hit.transform.tag == "Ground")
				{
					thrust = forwardAcceleration;
					float distanceFromFloor = hoverHeight - hit.distance;
					float upVelocity = body.GetRelativePointVelocity(hoverPoint.transform.position).z;
					body.AddForceAtPosition(transform.forward * (hoverForce * distanceFromFloor - upVelocity * damping), hoverPoint.transform.position);
					
				}
			} 
			else
			{
				thrust = forwardAcceleration / 2;
				float dropDamping = forwardAcceleration / turnPower;
				body.AddForceAtPosition(Vector3.down * hoverForce * dropDamping, hoverPoint.transform.position);
			}
		}

		//get a direction if wall incoming
		randomMovement ();

		//add forward force
		body.AddRelativeForce(thrust, 0.0f, 0.0f) ;
	
		//find angle between current position and direction to go
		float angleDifference = Vector3.Angle (transform.right, direction);

		//also the cross product
		Vector3 crossProd = Vector3.Cross (transform.right, direction);

		//apply torque towards this direction BUT only if collision incoming. Otherwise applys torque with every change in direction
		if(collisionIncoming == true)
		{
		 	body.AddTorque(turnPower * angleDifference * crossProd);
			collisionIncoming = false;
		}
	}

	//for taking bullets, if hit by bullet then take damage
	void OnCollisionEnter(Collision theCollided)
	{
		if (theCollided.gameObject.tag == "Bullet") {

			takeDamage (damage);
		}
	}

}
