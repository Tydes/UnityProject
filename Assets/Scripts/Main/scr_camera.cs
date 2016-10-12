using UnityEngine;
using System.Collections;

public class scr_camera : MonoBehaviour {


	public GameObject target;
	public int xPos = 0;
	public int yPos = 8;
	public int zPos = 104;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {



		if (GameObject.Find ("ToFollow(Clone)") != null) {
			target = GameObject.Find ("ToFollow(Clone)");
			transform.LookAt (target.transform);
			transform.position = new Vector3 (target.transform.position.x + xPos, target.transform.position.y + yPos, target.transform.position.z - zPos);
		}

	

	
	}
}
