using UnityEngine;
using System.Collections;

public class scr_game : MonoBehaviour {


	public int difficulty = 0;
	public string turn = "players";
	public float GameTimer = 100;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel (0);
		}


		if (turn == "players") {
			GameTimer = GameTimer - Time.deltaTime;
			if (GameTimer <= 0) {
				turn = "enemies";
				GameTimer = 10;
			}
		}


		if (turn == "enemies") {
			GameTimer = GameTimer - Time.deltaTime;
			if (GameTimer <= 0) {
				turn = "players";
				GameTimer = 10;
			}
		}



//		Debug.Log (GameTimer);
	
	}
}
