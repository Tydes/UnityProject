using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class scr_player : MonoBehaviour {

	private int SpaceToMove = 1; 
	private int RotateAngle = 90;
	private bool CanMove = true;

	public float RaycastDistance = 1;

	public GameObject WeaponPlacement;
	public GameObject WeaponPlacement2;

	public GameObject collidedWeapon;

	public int WeaponSlotSelected = 1;

	public GameObject[] Blocks;




	void Update () {
		GameObject gogame = GameObject.Find ("Game");
		scr_game gscript = gogame.GetComponent<scr_game> ();

		/* Drops the current weapon @@@@@@@@@@@@@@@@@@ needs work?
		if (Input.GetKeyDown (KeyCode.E)) {
			if (WeaponSlotSelected == 1) {
				if (WeaponPlacement.transform.childCount == 0) {
					WeaponPlacement.transform.DetachChildren ();
				}
			} else {
				if (WeaponPlacement2.transform.childCount == 0) {
					WeaponPlacement2.transform.DetachChildren ();

				}
			}
		}
		*/

		Vector3 fwd = transform.TransformDirection (Vector3.forward);

		/* Player Movement */
		RaycastHit hit;

		/* Detects Wall and Chest */
	//	Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y+0.1f, transform.position.z), fwd * RaycastDistance);
		if (Physics.Raycast (new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z), fwd * RaycastDistance, out hit)) {
		//	Debug.Log ("wat");
			if (hit.distance <= 1)	{
			//if (Vector3.Distance(transform.position,new Vector3 (transform.position.x, )) <= 1)	{

				/* Opens chest */
				if (hit.transform.tag == "chest") {
					if (Input.GetKeyDown (KeyCode.E)) {
						hit.collider.gameObject.GetComponent<scr_chest> ().CreateWep ();
					}
				}

				/* Stop the player from moving? */
				if (hit.transform.tag == "wall") {
			

				}

				/* Picks up weapon */
				if (hit.transform.tag == "weapon") {
					Debug.Log ("weapon");

					if (Input.GetKeyDown (KeyCode.E)) {
						if (WeaponSlotSelected == 1) {
							if (WeaponPlacement.transform.childCount == 0) {
								hit.collider.transform.parent = WeaponPlacement.transform;
								hit.collider.transform.position = WeaponPlacement.transform.position;

								Weapons wepscript = hit.transform.GetComponent<Weapons> ();
								wepscript.Equipped = true;
							} else {

								Transform child1 = WeaponPlacement.transform.GetComponentInChildren<Transform>();
								child1.localPosition = new Vector3 (child1.transform.localPosition.x, child1.transform.localPosition.y, child1.transform.localPosition.y);
								Weapons wepscript = WeaponPlacement.GetComponentInChildren<Weapons>();
								wepscript.Equipped = false;

								WeaponPlacement.transform.DetachChildren();
								hit.collider.transform.parent = WeaponPlacement.transform;
								hit.collider.transform.position = WeaponPlacement.transform.position;
							}
						} else {
							if (WeaponPlacement2.transform.childCount == 0) {
								hit.collider.transform.parent = WeaponPlacement2.transform;
								hit.collider.transform.position = WeaponPlacement2.transform.position;

							} else {
								/* Drops Child - Oops? */

								Transform child2 = WeaponPlacement2.transform.GetChild (0);
								child2.localPosition = new Vector3 (child2.transform.localPosition.x, child2.transform.localPosition.y, child2.transform.localPosition.y);

								Weapons wepscript = WeaponPlacement2.GetComponentInChildren<Weapons>();
								wepscript.Equipped = false;

								WeaponPlacement2.transform.DetachChildren();
								hit.collider.transform.parent = WeaponPlacement2.transform;
								hit.collider.transform.position = WeaponPlacement2.transform.position;

							}

						}
					}




				}



			}
		}


		/* Weapon Stuff */


		// User Interface
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			WeaponSlotSelected = 1;
		}	

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			WeaponSlotSelected = 2;
		}

			

		/* When it's the player's turn, do this... */

		if (gscript.turn == "players") {
			GameObject Followme = GameObject.Find ("Main Camera");
			scr_camera scrcam = Followme.GetComponent<scr_camera> ();

			/* Movement */
			if (Input.GetKeyDown (KeyCode.W)) {
				transform.position += transform.forward * SpaceToMove;
		
			}

			if (Input.GetKeyDown (KeyCode.A)) {
				transform.position -= transform.right * SpaceToMove;
				//transform.position = new Vector3 (transform.position.x - SpaceToMove, transform.position.y, transform.position.z);
			}

			if (Input.GetKeyDown (KeyCode.S)) {
				transform.position -= transform.forward * SpaceToMove;

			}

			if (Input.GetKeyDown (KeyCode.D)) {
				transform.position += transform.right * SpaceToMove;

				//transform.position = new Vector3 (transform.position.x + SpaceToMove, transform.position.y, transform.position.z);
			}



			/* Rotation */
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				transform.Rotate (Vector3.down * RotateAngle);
				//Followme.gameObject.transform.position = new Vector3 (Followme.transform.position.x, Followme.transform.position.y + 15, Followme.transform.position.z - 60);
			//	scrcam.xPos = scrcam.xPos + 15; 
			//	scrcam.zPos = scrcam.zPos - 10;
			//	Followme.gameObject.transform.RotateAround(Followme.transform.position,Vector3.left,RotateAngle);
			}

			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				transform.Rotate (Vector3.up * RotateAngle);
				//Followme.gameObject.transform.position = new Vector3 (Followme.transform.position.x, Followme.transform.position.y + 15, Followme.transform.position.z + 30);
			//	scrcam.xPos = scrcam.xPos - 15; 
			//	scrcam.zPos = scrcam.zPos + 10;
			//	Followme.gameObject.transform.RotateAround(Followme.transform.position,Vector3.right,RotateAngle);

			}
		}

		ClosestBlocks ();
	}






	void ClosestBlocks()	{
		Blocks = GameObject.FindGameObjectsWithTag ("block");
		List<GameObject> blist = new List<GameObject> (Blocks);

		//blist.Sort ();
	}
		




}
