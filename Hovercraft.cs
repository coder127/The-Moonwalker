using UnityEngine;
using System.Collections;

public class HoverCraft : MonoBehaviour 
{
	Rigidbody body; //the rigidbody
	
	float isMovingZone = 0.1f; //is the spacecraft moving?
	
	//Forward
	public float forwardAcceleration = 100.0f;
	public float backwardAcceleration = 25.0f;
	float thrust = 0.0f;
	
	//Turning
	public float turnPower = 10.0f;
	float turn = 0.0f;
	
	//Hovering
	public float hoverForce = 9.0f;
	public float hoverHeight = 2.0f;
	public float damping; //for damping movement

	//Hover Points
	int layerMask;
	public GameObject[] hoverPoints;

	// Use this for initialization
	void Start () 
	{
		body = GetComponent<Rigidbody>();

		//set a layer mast to character which the hovercraft will ignore when raycasting
		layerMask = 1 << LayerMask.NameToLayer ("Characters");
		layerMask = ~layerMask;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//get thrust value for moving the hovercraft forwards or backwards
		thrust = 0.0f;
		float direction = Input.GetAxis ("Vertical");
		if(direction > isMovingZone)
		{
			thrust = direction * forwardAcceleration;
		}
		else if (direction < -isMovingZone)
		{
			thrust = direction * backwardAcceleration;
		}
		
		//turning the hovercraft 
		turn = 0.0f;
		float rotation = Input.GetAxis ("Horizontal");
		if(Mathf.Abs(rotation) > isMovingZone)
		{
			turn = rotation;
		}

	}
	
	void FixedUpdate()
	{
		//Hovering
		RaycastHit hit;
		for (int i = 0; i < hoverPoints.Length; i++)
		{
			//check all hoverpoints by raycasting downwards
			GameObject hoverPoint = hoverPoints [i];
			if (Physics.Raycast (hoverPoint.transform.position, transform.TransformDirection(Vector3.back), out hit, 
			                     hoverHeight, layerMask))
			{
				//if theres something below push up
				if(hit.transform.tag == "Ground")
				{
					float distanceFromFloor = hoverHeight - hit.distance;
					float upVelocity = body.GetRelativePointVelocity(hoverPoint.transform.position).z;
					body.AddForceAtPosition(transform.forward * (hoverForce * distanceFromFloor - upVelocity * damping), hoverPoint.transform.position);

				}
			} 
			else
			{
				//otherwise push down
				thrust = 0.0f;
				body.AddForceAtPosition(hoverPoint.transform.TransformDirection(Vector3.back) * hoverForce * 10, hoverPoint.transform.position);
			}
		}
		
		if(Mathf.Abs(thrust) > 0)
		{
			//move forward
			body.AddRelativeForce(0.0f, -thrust, 0.0f) ;
		}

		//turn
		if(turn > 0)
		{

			body.AddRelativeTorque(0.0f, 0.0f, turn * turnPower);
		}
		else if (turn < 0)
		{
			body.AddRelativeTorque(0.0f, 0.0f, turn * turnPower);
		}

		
	}
}
