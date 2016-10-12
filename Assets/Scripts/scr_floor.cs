using UnityEngine;
using System.Collections;

public class scr_floor : MonoBehaviour {


	//private int RandMove = 0;

	private bool destroyed = false;

	// Use this for initialization
	void Start () {
	//	RandMove = Random.Range (0, 6);
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void OnTriggerStay2D(Collider2D coll)	{
		if (coll.gameObject.tag == "floor") {
			//Debug.Log ("TEST");
			if(destroyed == false) {
			//	Destroy(coll.gameObject);
			//	GameObject GO = GameObject.Find ("Gen");
			//	scr_gen gScript = GO.GetComponent<scr_gen> ();
			//	gScript.blocksperlevel--;
			/*	if (RandMove == 0) {
					transform.position += new Vector3 (-0.16f, 0, 0); // right
					RandMove = Random.Range (0, 4);
				}
				if (RandMove == 1) {
					transform.position += new Vector3 (0.16f, 0, 0); // left
					RandMove = Random.Range (0, 4);
				}
				if (RandMove == 3) {
					transform.position += new Vector3 (0, -0.16f, 0); // up
					RandMove = Random.Range (0, 4);
				}
				if (RandMove == 4) {
					transform.position += new Vector3 (0, 0.16f, 0); // down
					RandMove = Random.Range (0, 4);
				}
				if (RandMove == 5) {*/
					Destroy (coll.gameObject);
					destroyed = true;
					//RandMove = Random.Range (0, 4);
				}

	
		}
	}





}
