using UnityEngine;
using System.Collections;



public class BallInteraction : MonoBehaviour {

	// this is making reference to the cardboard object
	public Transform CB;
	// this variable is used to know which ball is selected in case there is more than one
	public GameObject selectedBALL;
	// this is the head gameObject
	public Transform head;
	// this is the speed of the ball when folowing the head movement
	public float displacementSpeed;
	// this is the force value 
	public float throwingForce=20f;
	// this is the rigid body attached to the ball
	Rigidbody rigBody;
	// this is the distance of the ball to the player, in case you want to create advanced interactions
	float distanceToPlayer;
	// this is the time parameter, that can be used to check the time between shots
	float elapsed;
	// chose Basketball or football
	public bool isBasket=false;

	public int state=0;  
	/* 
		sate=0 --> normal cardboard movement     <----
		state=1 --> moving with the ball on the hand  |
		state=2 --> trow object ---> goes to state 0  |

	*/


	// Use this for initialization

	void Start () 
	{
		

	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		// obtain distance to player
		if (selectedBALL != null) {
			distanceToPlayer = (selectedBALL.transform.position - CB.transform.position).magnitude;

		}
				
		//restart ball position if the distance is realy big >15 for example
		if (distanceToPlayer > 15)
		{
			// reset ball position
			selectedBALL.transform.position=head.position + 1.2f * head.forward;
			rigBody.velocity = new Vector3 (0, 0, 0);
		}

		// if state ==0 do nothing
		if (state == 0) 
		{
			
		}


		// if the player is holding the ball, move it accordingly
		if(state==1 && selectedBALL!=null)
		{

			// the position is obtained thanks to the head gameObject
			Vector3 objective = head.position + 1.2f * head.forward;//-0.5f*head.up;

			Vector3 objetiveFottball=new Vector3(objective[0],0.3f, objective[2]);

			if (isBasket) {
				selectedBALL.transform.position = Vector3.Lerp (selectedBALL.transform.position, objective, displacementSpeed);
			} else {
				selectedBALL.transform.position = Vector3.Lerp (selectedBALL.transform.position, objetiveFottball, displacementSpeed);
			}
			selectedBALL.transform.rotation= Quaternion.Lerp(selectedBALL.transform.rotation,head.rotation*Quaternion.Euler(90,0,0),displacementSpeed);

			// stop the speed of the ball also and the gravity (fixed in space)
			rigBody.velocity = new Vector3 (0, 0, 0);
			rigBody.useGravity = false;

		}


		// if state ==2, trhow tehe ball and go back to state 0
		if( state==2 )
		{				
			elapsed=0;
							
			rigBody.velocity=throwingForce*head.forward+ throwingForce/2*head.up;
			rigBody.useGravity = true;
			// go back to first state 
			state = 0;
		}



	}



	// THIS FUNCTION IS CALLED TO CHANGE THE STATE and CHOOSE A SELECTED OBJECT
	public void pointerClick(GameObject go)
	{
		if (state == 0) {
			state = 1;

			selectedBALL= go;
			rigBody = selectedBALL.GetComponent<Rigidbody> ();

		} else if (state == 1) {
			state = 2;
		} 


	}




}
