using UnityEngine;
using System.Collections;

public class scr_gen : MonoBehaviour {


	private int TurnAngle = 0;
	public int blocksperlevel = 0;
	private int blockcounter = 1000;

	public GameObject floor;

	private bool CollidingWithFloor = false;

	void Start () {



	}

	void Update () {
		Debug.Log (CollidingWithFloor);

		if (blocksperlevel < blockcounter) {
			blocksperlevel++;


			//	if (CollidingWithFloor == false) {
			//	yield WaitForSeconds(5);
			//if (CollidingWithFloor == false) 	{
				if (TurnAngle == 0) {
					transform.position += new Vector3 (0.16f, 0, 0); // right
				}
				if (TurnAngle == 1) {
					transform.position += new Vector3 (-0.16f, 0, 0); // left
				}
				if (TurnAngle == 2) {
					transform.position += new Vector3 (0, 0.16f, 0); // up
				}
			//	if (TurnAngle == 3) {
			//		transform.position += new Vector3 (0, -0.16f, 0); // down
			//	}
				Instantiate (floor, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

			//}



			TurnAngle = Random.Range(0,3);

		}

	}

	void OnTriggerStay2D(Collider2D coll)	{
		if (coll.gameObject.tag == "floor") {
			CollidingWithFloor = true;
		}
	}
	void OnTriggerExit2D(Collider2D coll2)	{
		if (coll2.gameObject.tag == "floor") {
			CollidingWithFloor = false;
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
