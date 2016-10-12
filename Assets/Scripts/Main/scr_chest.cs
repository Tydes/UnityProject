using UnityEngine;
using System.Collections;

public class scr_chest : MonoBehaviour {


	public GameObject[] RandomWeapons;


	// Use this for initialization
	void Start () {
	
	}

	public void CreateWep()	{

		GameObject wep = RandomWeapons [Random.Range (0, RandomWeapons.Length)];
		Instantiate (wep, new Vector3 (transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
		Destroy (gameObject);
		//Instantiate(wep,
	}

		
}
