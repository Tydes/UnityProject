using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class scr_player : MonoBehaviour {

	/* movement */
	private int SpaceToMove = 1; 
	private int RotateAngle = 90;
	private bool CanMove = true;
	private bool disableInput = false;

	/* Distance */
	public float RaycastDistance = 1;

	/* Weapons */
	public GameObject WeaponPlacement;
	public GameObject WeaponPlacement2;

	public GameObject collidedWeapon;

	public int WeaponSlotSelected = 1;

	/* Other */
	public GameObject[] Blocks;
	public GameObject followMeObject;

	/* Camera */
	private int CameraRotate = 90;

	/* item stuff */
	public GameObject hitItem;

	/* Invetory Stuff */
	private int inventoryLimit = 100;
	private int currentInventoryNumber = 0;

	public List<GameObject> InventoryList;


	void Start()	{
		followMeObject = GameObject.Find ("ToFollow(Clone)");
	}




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
		//Debug.DrawLine (new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z), fwd * RaycastDistance);
		if (Physics.Raycast (new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z), fwd * RaycastDistance, out hit)) {
			
			if (hit.distance <= 1)	{

				/* Opens chest */
				if (hit.transform.tag == "chest") {
					if (Input.GetKeyDown (KeyCode.E)) {
						hit.collider.gameObject.GetComponent<scr_chest> ().CreateWep ();
					}
				}

				/* Stop the player from moving? */
				if (hit.transform.tag == "wall") {
					

				}

				if (hit.transform.tag == "item") {
					Debug.Log ("Hitting Item!");
					if (Input.GetKeyDown (KeyCode.E)) {
						if (currentInventoryNumber < inventoryLimit) {
							hitItem = hit.transform.gameObject;
							//hitItem.transform.localScale -= Vector3.one * Time.deltaTime;
							InventoryList.Insert (InventoryList.Count, hitItem);
							Debug.Log (InventoryList.Count);
							Destroy (hitItem);
						} else {
							Debug.Log ("No room!");
						}
					}
				}


				/* Picks up weapon */
				if (hit.transform.tag == "weapon") {
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
			if (disableInput == false) {
				GameObject Followme = GameObject.Find ("Main Camera");
				scr_camera scrcam = Followme.GetComponent<scr_camera> ();

				/* Movement */
				if (Input.GetKeyDown (KeyCode.W)) {
					transform.position += transform.forward * SpaceToMove;
			
				}

				if (Input.GetKeyDown (KeyCode.A)) {
					transform.position -= transform.right * SpaceToMove;
			
				}

				if (Input.GetKeyDown (KeyCode.S)) {
					transform.position -= transform.forward * SpaceToMove;

				}

				if (Input.GetKeyDown (KeyCode.D)) {
					transform.position += transform.right * SpaceToMove;
			
				}
				
				/* Rotation */
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					scrcam.CanRotate = true;
					transform.Rotate (Vector3.down * RotateAngle);
					followMeObject.transform.Rotate (Vector3.down * RotateAngle);
					/* Camera Rotation */
					Followme.transform.RotateAround (transform.position, Vector3.down, CameraRotate);
		
				}

				if (Input.GetKeyDown (KeyCode.RightArrow)) {
					scrcam.CanRotate = true;
					transform.Rotate (Vector3.up * RotateAngle);
					followMeObject.transform.Rotate (Vector3.up * RotateAngle);

					/* Camera Rotation */
					Followme.transform.RotateAround (transform.position, Vector3.up, CameraRotate);
				}

				if (!Input.GetKeyDown (KeyCode.RightArrow) && !Input.GetKeyDown (KeyCode.LeftArrow)) {
					scrcam.CanRotate = false;
				}
			}

		}

	}






/*	void ClosestBlocks()	{
		// Optimisation 
	//	Blocks = GameObject.FindGameObjectsWithTag ("block");
	//	List<GameObject> blist = new List<GameObject> (Blocks);

		//blist.Sort ();
		// NEEDS TO BE CALLED
	}*/
		




}
