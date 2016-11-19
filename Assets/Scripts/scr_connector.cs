using UnityEngine;
using System.Collections;

public class scr_connector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
















	void OnTriggerEnter(Collider coll)	{

		if (coll.gameObject.tag == "connector") {
			Destroy (gameObject);
		}



	}



}
