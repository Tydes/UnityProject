using UnityEngine;
using System.Collections;

public class scr_roomcreator : MonoBehaviour {

	private int ChoosePath = 0;
	private int roomsToDestroy = 15;


	// Use this for initialization
	void Start () {
		ChoosePath = Random.Range (0, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (ChoosePath == 0) {
			transform.position += new Vector3 (-0.16f, 0, 0); // right
		} else {
			transform.position += new Vector3 (0.16f, 0, 0); // left
		}

		roomsToDestroy--;

		if (roomsToDestroy <= 5) {
			transform.localScale = new Vector3 (5, 5, 0);
		}

		if (roomsToDestroy <= 0) {
			Destroy (gameObject);
		}


	}


	void OnTriggerEnter2D(Collider2D coll)	{
		if (coll.gameObject.tag == "floor") {
			Destroy (coll.gameObject);

		}
	}
}
