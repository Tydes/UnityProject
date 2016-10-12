using UnityEngine;
using System.Collections;

public class scr_3dgen : MonoBehaviour {


	private int TurnAngle;
	public int blocksperlevel = 0;
	public int blockcounter = 1000;
	public bool destroyedStuff = false;

	private int PickNumbers = 0;

	public GameObject floor;
	public GameObject player;
	public GameObject ToFollowObject;

	void Start () {

		while (blocksperlevel < blockcounter) {

			if (TurnAngle == 0) {
				transform.position += new Vector3 (0, 0, 1); // right
			//	TurnAngle = 1;
				TurnAngle = 1;
//				Debug.Log ("SUP");

			}
			if (TurnAngle == 1) {
				transform.position += new Vector3 (0, 0, -1); // left
			//	PickNumbers = Random.Range (0, 5);
			//	if (PickNumbers != 0) {
			//		TurnAngle = 2;
			//	} else {
			//		TurnAngle = 1;
			//	}
		//		Debug.Log ("SUP3");


			}
			if (TurnAngle == 2) {
				transform.position += new Vector3 (1, 0, 0); // up
			//	PickNumbers = Random.Range (0, 10);
			//	if (PickNumbers != 0) {
			//		TurnAngle = 1;
			//	} else {
			//		TurnAngle = 0;
			//	}
		//		Debug.Log ("SUP3");

			}
			Instantiate (floor, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
			blocksperlevel++;




		} 

	}

	void Update () {


		if (blocksperlevel >= blockcounter) {
			if (destroyedStuff == true) {
				Instantiate(ToFollowObject,new Vector3 (transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

				Instantiate(player,new Vector3 (transform.position.x, transform.position.y+0.5f, transform.position.z), Quaternion.identity);


				Destroy (gameObject);
			}
		}

	}

	/* void OnTriggerStay2D(Collider coll)	{
		//if (coll.gameObject.tag == "floor") {
		//	CollidingWithFloor = true;
	//		Destroy(coll.gameObject);
	//	}
	}

	void OnTriggerExit2D(Collider coll2)	{
		if (coll2.gameObject.tag == "floor") {
			CollidingWithFloor = false;
		}
	}
	*/
}
