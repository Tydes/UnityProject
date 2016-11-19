using UnityEngine;
using System.Collections;

public class scr_camera : MonoBehaviour {

	/* Game object import */
	public GameObject target;

	/* Speed of the camera */
	private float speed = 0.5f;

	/* Position */
	public int xPos = 0;
	public int yPos = 0;
	public int zPos = 0;

	/* Rotation */
	public int xRot = 0;
	public int yRot = 0;
	public int zRot = 0;

	/* If you can turn the camera */
	public bool CanRotate = false;

	/* Choose who it faces */
	public string CameraState = "player";

	/* These are the lists of stuff the camera can look at:
	 * player
	 * boss1
	 * boss2
	 * ..... etc
	 * 
	 */

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {



		if (GameObject.Find ("ToFollow(Clone)") != null) {
			target = GameObject.Find ("ToFollow(Clone)");

			if (CameraState == "player") {
				if (CanRotate == false) {
					transform.LookAt (new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z));
					if (Vector3.Distance (transform.position, target.transform.position) >= 10) {
						transform.position = Vector3.MoveTowards (transform.position, new Vector3(target.transform.position.x, target.transform.position.y+5, target.transform.position.z), speed);
					}
				}

			} else if (CameraState == "boss1") {

			}


			//if (CanRotate == false)	{
			//	transform.LookAt ( new Vector3(target.transform.position.x + xRot, target.transform.position.y + yRot, target.transform.position.z + zRot));
			//	transform.position = new Vector3 (target.transform.position.x + xPos, target.transform.position.y + yPos, target.transform.position.z + zPos);
			//}
		}


	

	
	}
}
