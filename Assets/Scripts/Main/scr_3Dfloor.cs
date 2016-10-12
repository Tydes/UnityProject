using UnityEngine;
using System.Collections;

public class scr_3Dfloor : MonoBehaviour {
	
	private bool destroyed = false;

	public GameObject floor;
	private GameObject fl;
	private GameObject fl2;
	private int floorMaker;



	public GameObject wall;
	public GameObject door;
	public GameObject doorframe;

	public GameObject chest;



	void Start()	{

		/* LEVEL GENERATION - DO NOT ENTER */

		floorMaker = Random.Range (0, 150);

		/* Make the hallway thicker */
		Instantiate (floor, new Vector3 (transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.identity);
		Instantiate (floor, new Vector3 (transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity);
		Instantiate (floor, new Vector3 (transform.position.x - 2, transform.position.y, transform.position.z), Quaternion.identity);
		Instantiate (floor, new Vector3 (transform.position.x + 2, transform.position.y, transform.position.z), Quaternion.identity);
		Instantiate (floor, new Vector3 (transform.position.x - 3, transform.position.y, transform.position.z), Quaternion.identity);
		Instantiate (floor, new Vector3 (transform.position.x + 3, transform.position.y, transform.position.z), Quaternion.identity);

		/* makes a long hallway with narrow path */
		if (floorMaker == 0) {
			/* long path */
			for (int i = 0; i < 40; i++) {
				for (int o = 0; o < 2; o++) {																				// makes it thicker
					Instantiate (floor, new Vector3 (transform.position.x + 4 + i, transform.position.y, transform.position.z + o), Quaternion.identity);
				}
			}

			/* makes the left turn */
			for (int i = 0; i < 15; i++) {
				Instantiate (floor, new Vector3 (transform.position.x + 44, transform.position.y, transform.position.z + i), Quaternion.identity);
			}
		}
	
		/* Small Room */
		if (floorMaker == 1) {
			/* Small Room path */
			Instantiate (floor, new Vector3 (transform.position.x - 4, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 4, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 5, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 6, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 7, transform.position.y, transform.position.z- 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 8, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 9, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 10, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 11, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 12, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 13, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 14, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 15, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 16, transform.position.y, transform.position.z- 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 17, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 17, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 18, transform.position.y, transform.position.z- 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 18, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 19, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 20, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 21, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 22, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 23, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 24, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 25, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 26, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 27, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 28, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 29, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y, transform.position.z), Quaternion.identity);

			/* Door frame */
			Instantiate (doorframe, new Vector3 (transform.position.x - 29, transform.position.y-0.5f, transform.position.z), Quaternion.identity);

			/* Small room */
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z-1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z+1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z-2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z+2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z-3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z+3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z-4), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 30, transform.position.y , transform.position.z+4), Quaternion.identity);

			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z-1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z+1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z-2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z+2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z-3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z+3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z-4), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 31, transform.position.y , transform.position.z+4), Quaternion.identity);

			/* Create chest */
			Instantiate (chest, new Vector3 (transform.position.x - 32, transform.position.y+0.8f, transform.position.z+Random.Range(-3,4)), Quaternion.identity);

			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z-1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z+1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z-2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z+2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z-3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z+3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z-4), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 32, transform.position.y , transform.position.z+4), Quaternion.identity);
		
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y, transform.position.z), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z-1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z+1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z-2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z+2), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z-3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z+3), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z-4), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x - 33, transform.position.y , transform.position.z+4), Quaternion.identity);


			/* Creates the walls */
			// back and front
			for (int i = 0; i < 5; i++) {
				for (int ii = -5; ii < 6; ii++) {
					Instantiate (wall, new Vector3 (transform.position.x - 34, transform.position.y + i, transform.position.z +ii), Quaternion.identity);
				// front -	Instantiate (wall, new Vector3 (transform.position.x - 29, transform.position.y + i, transform.position.z -ii-1), Quaternion.identity);

				}
			}
			// left and right
			for (int iii = 1; iii < 6; iii++)	{ // width
				for (int o = 0; o < 5; o++)	{ // height
					Instantiate (wall, new Vector3 (transform.position.x -34+iii, transform.position.y+o, transform.position.z-5), Quaternion.identity);
					Instantiate (wall, new Vector3 (transform.position.x -34+iii, transform.position.y+o, transform.position.z+5), Quaternion.identity);

				}
			}
				
		}

		/* This needs changing */
		if (floorMaker == 2) {
			Instantiate (floor, new Vector3 (transform.position.x + 4, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 5, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 6, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 7, transform.position.y, transform.position.z- 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 8, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 9, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 10, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 11, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 12, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 13, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 14, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 15, transform.position.y, transform.position.z - 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 16, transform.position.y, transform.position.z- 1), Quaternion.identity);
			Instantiate (floor, new Vector3 (transform.position.x + 17, transform.position.y, transform.position.z - 1), Quaternion.identity);
		}

		/* long hall way */
		if (floorMaker == 3) {
			for (int i = 0; i < 30; i++) {
				for (int ii = 0; ii < 4; ii++) {
					Instantiate (floor, new Vector3 (transform.position.x - 4-i, transform.position.y, transform.position.z-ii), Quaternion.identity);
				}
			}
		}

		/* Two lane hallway, chest at the end */
		if (floorMaker == 4) {

			// hallway connector
			Instantiate (floor, new Vector3 (transform.position.x - 4-Random.Range(5,30), transform.position.y, transform.position.z+1), Quaternion.identity);

			// two land hallway
			for (int i = 0; i < 35; i++) {
				for (int ii = 0; ii < 35; ii++) {
					Instantiate (floor, new Vector3 (transform.position.x - 4-i, transform.position.y, transform.position.z), Quaternion.identity);
					Instantiate (floor, new Vector3 (transform.position.x - 4-ii, transform.position.y, transform.position.z+2), Quaternion.identity);
				}
			}

			// creates the room 
			for (int i = 0; i < 15; i++) {
				for (int ii = -5; ii < 15; ii++) {
					Instantiate (floor, new Vector3 (transform.position.x - 39-i, transform.position.y, transform.position.z-ii), Quaternion.identity);
				}
			}

		}




	}

	void Update()	{
		if (GameObject.Find ("3DGen") != null) {
			GameObject GO = GameObject.Find ("3DGen");
			scr_3dgen gscript = GO.GetComponent<scr_3dgen> ();
		
			if (gscript.blocksperlevel >= gscript.blockcounter) {
				
				Destroy (GetComponent<Rigidbody> ());
				Destroy (GetComponent<BoxCollider> ());
				gscript.destroyedStuff = true;
				Destroy (GetComponent<scr_3Dfloor> ());




			}
		}
	}

	void OnCollisionStay(Collision coll)	{
		if (coll.gameObject.tag == "floor") {
			if(destroyed == false) {
				Destroy (coll.gameObject);
				coll.gameObject.GetComponent<scr_3Dfloor>().destroyed = true;
			}


		}
	}





}
