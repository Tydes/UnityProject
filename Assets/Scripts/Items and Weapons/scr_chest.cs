using UnityEngine;
using System.Collections;

public class scr_chest : MonoBehaviour {


	public GameObject[] RandomWeapons;


	// Use this for initialization
	void Start () {
	
	}

	public void CreateWep()	{

		GameObject wep = RandomWeapons [Random.Range (0, RandomWeapons.Length)];


		float halfHeight = wep.GetComponent<Renderer> ().bounds.extents.y;


		Instantiate (wep, new Vector3 (transform.position.x, transform.position.y-halfHeight, transform.position.z), Quaternion.identity);
		Destroy (gameObject);
		//Instantiate(wep,
	}

		
}
