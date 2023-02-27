using UnityEngine;
using System.Collections;

public class ShadowBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

		//sets the position to zero (ground shadow)
		transform.position = new Vector3 (transform.parent.position [0], 0.001f, transform.parent.position [2]);
		transform.rotation = Quaternion.Euler (90, 0, 0);

		//scale changes with the position of the ball
		transform.localScale = new Vector3 (0.1f*transform.parent.position [1]+2,0.1f*transform.parent.position [1]+2, 0.1f*transform.parent.position [1]+2);
	}
}
