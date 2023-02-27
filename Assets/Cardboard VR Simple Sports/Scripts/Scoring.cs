using UnityEngine;
using System.Collections;

public class Scoring : MonoBehaviour {

	// Use this for initialization

	//this is the public score for the game
	public int score;
	// this is the time variable used to prevent double-scoring
	public float elapsed;
	// text scoring basket
	public UnityEngine.UI.Text textScore;
	// animation of the net of the basket
	public Animator animNet;

	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		elapsed += Time.fixedDeltaTime;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Ball" && elapsed >1.0f ) {
			score += 1;
			elapsed = 0;
			Debug.Log ("Score ="+score);

			// score update
			if (score == 11) {
				textScore.text = "1 1";
			} else {
				textScore.text = "" + score;
			}

			if (animNet != null) {
				animNet.SetTrigger ("Score");
			}


		}
	}


}
