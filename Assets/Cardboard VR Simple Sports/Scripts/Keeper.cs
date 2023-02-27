using UnityEngine;
using System.Collections;

public class Keeper : MonoBehaviour {

	// Use this for initialization

	// initial postion to create the move-return effect
	Vector3 initialMov;
	// amplitude of the movement
	public float amplitude;
	public float speed;

	void Start () {
		initialMov = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.position = initialMov + amplitude*new Vector3 (0,0,Mathf.Sin(speed*Time.fixedTime));
	}
}
