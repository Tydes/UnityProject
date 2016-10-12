using UnityEngine;
using System.Collections;

public class scr_followme : MonoBehaviour {


	public GameObject targetO;
	public float speed = 0.5f;

	private bool lookAtPlayer = true;


	// Use this for initialization
	void Start () {
	//	if (GameObject.Find ("Player Object(Clone)") != null) {
		//	targetO = GameObject.Find ("Player Object(Clone)");
			transform.position = targetO.transform.position;
	//	}
	}
	
	// Update is called once per frame
	void Update () {

		if (lookAtPlayer == true) {
			if (GameObject.Find ("Player Object(Clone)") != null) {
				targetO = GameObject.Find ("Player Object(Clone)");
			}
		} else {
			
		}

		//speed = speed * Vector3.Distance (transform.position, targetO.transform.position);
		transform.position = Vector3.MoveTowards (transform.position, targetO.transform.position, speed);

	}




}
