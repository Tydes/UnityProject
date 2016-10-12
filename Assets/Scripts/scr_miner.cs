using UnityEngine;
using System.Collections;

public class scr_miner : MonoBehaviour {
	private int TurnAngle = 0;
	private int blockToRemove = 100;

	private int ScaleChange = 6;


	public GameObject RoomMiner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		ScaleChange = Random.Range (0, 5);
		TurnAngle = Random.Range(0,3);

		if (ScaleChange == 0) {
			transform.localScale = new Vector3 (2, 2, 0);
		} else {
			transform.localScale = new Vector3 (1, 1, 0);
		}

		if (TurnAngle == 0) {
			transform.position += new Vector3 (-0.16f, 0, 0); // right
		}
		if (TurnAngle == 1) {
			transform.position += new Vector3 (0.16f, 0, 0); // left
		}
		if (TurnAngle == 2) {
			transform.position += new Vector3 (0, 0.16f, 0); // up
		}
	//	if (TurnAngle == 3) {
		//	transform.position += new Vector3 (0, -0.16f, 0); // down
		//}


	
		if (blockToRemove <= 0) {
			Destroy (gameObject);
		}

		if (blockToRemove == 75) {
			Instantiate (RoomMiner, new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
		}

		if (blockToRemove == 50) {
			Instantiate (RoomMiner, new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
		}

		if (blockToRemove == 25) {
			Instantiate (RoomMiner, new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
		}


	}



	void OnTriggerStay2D(Collider2D coll)	{
		if (coll.gameObject.tag == "floor") {
			Destroy (coll.gameObject);
			blockToRemove--;
		}
	}


}
