using UnityEngine;
using System.Collections;

public class scr_game : MonoBehaviour {


	public int difficulty = 0;
	public string turn = "players";
	public float GameTimer = 100;

	public Texture2D cursorImage;


	// Use this for initialization
	void Start () {


        Cursor.SetCursor(cursorImage,Vector2.zero,CursorMode.Auto);
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
