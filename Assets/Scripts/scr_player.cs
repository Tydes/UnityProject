using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
//using System.Linq; // used to sort the list

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
	private int inventoryLimit = 25;
	private int currentInventoryNumber = 0;

	public List<GameObject> InventoryList;
   
	/* User Interface */
	private bool interfaceOpen = false;
	public Text inventoryItemName;
	public Text inventoryItemDescription;
	public GameObject displayObject;

	/* User Interface List */
	public GameObject textObject1;
	public Text text1;
	public string desc1;

	public GameObject textObject2;
	public Text text2;
	public string desc2;

	public GameObject textObject3;
	public Text text3;
	public string desc3;

	public GameObject textObject4;
	public Text text4;
	public string desc4;

	public GameObject textObject5;
	public Text text5;
	public string desc5;

	public GameObject textObject6;
	public Text text6;
	public string desc6;

	public GameObject textObject7;
	public Text text7;
	public string desc7;

	public GameObject textObject8;
	public Text text8;
	public string desc8;

	public GameObject textObject9;
	public Text text9;
	public string desc9;

	public GameObject textObject10;
	public Text text10;
	public string desc10;

	public GameObject textObject11;
	public Text text11;
	public string desc11;

	public GameObject textObject12;
	public Text text12;
	public string desc12;

	public GameObject textObject13;
	public Text text13;
	public string desc13;

	public GameObject textObject14;
	public Text text14;
	public string desc14;

	public GameObject textObject15;
	public Text text15;
	public string desc15;

	public GameObject textObject16;
	public Text text16;
	public string desc16;

	public GameObject textObject17;
	public Text text17;
	public string desc17;

	public GameObject textObject18;
	public Text text18;
	public string desc18;

	public GameObject textObject19;
	public Text text19;
	public string desc19;

	public GameObject textObject20;
	public Text text20;
	public string desc20;

	public GameObject textObject21;
	public Text text21;
	public string desc21;

	public GameObject textObject22;
	public Text text22;
	public string desc22;

	public GameObject textObject23;
	public Text text23;
	public string desc23;

	public GameObject textObject24;
	public Text text24;
	public string desc24;

	public GameObject textObject25;
	public Text text25;
	public string desc25;

	public GameObject textObject26;
	public Text text26;
	public string desc26;

	/* Inventory List */
	public bool invent1 = false;
	public bool invent2 = false;
	public bool invent3 = false;
	public bool invent4 = false;
	public bool invent5 = false;
	public bool invent6 = false;
	public bool invent7 = false;
	public bool invent8 = false;
	public bool invent9 = false;
	public bool invent10 = false;
	public bool invent11 = false;
	public bool invent12 = false;
	public bool invent13 = false;
	public bool invent14 = false;
	public bool invent15 = false;
	public bool invent16 = false;
	public bool invent17 = false;
	public bool invent18 = false;
	public bool invent19 = false;
	public bool invent20 = false;
	public bool invent21 = false;
	public bool invent22 = false;
	public bool invent23 = false;
	public bool invent24 = false;
	public bool invent25 = false;
	public bool invent26 = false;

	/* Canvas group- Need to find them in the world space at the start event*/
	public GameObject inventoryUI;

	void Start()	{
		followMeObject = GameObject.Find ("ToFollow(Clone)");
		inventoryUI = GameObject.Find ("InventoryCanvasGroup");

		/* Text UI */
		textObject1 = GameObject.Find ("Text1");
		text1 = textObject1.transform.GetComponent<Text> ();

		textObject2 = GameObject.Find ("Text2");
		text2 = textObject2.transform.GetComponent<Text> ();

		textObject3 = GameObject.Find ("Text3");
		text3 = textObject3.transform.GetComponent<Text> ();

		textObject4 = GameObject.Find ("Text4");
		text4 = textObject4.transform.GetComponent<Text> ();

		textObject5 = GameObject.Find ("Text5");
		text5 = textObject5.transform.GetComponent<Text> ();

		textObject6 = GameObject.Find ("Text6");
		text6 = textObject6.transform.GetComponent<Text> ();

		textObject7 = GameObject.Find ("Text7");
		text7 = textObject7.transform.GetComponent<Text> ();

		textObject8 = GameObject.Find ("Text8");
		text8 = textObject8.transform.GetComponent<Text> ();

		textObject9 = GameObject.Find ("Text9");
		text9 = textObject9.transform.GetComponent<Text> ();

		textObject10 = GameObject.Find ("Text10");
		text10 = textObject10.transform.GetComponent<Text> ();

		textObject11 = GameObject.Find ("Text11");
		text11 = textObject11.transform.GetComponent<Text> ();

		textObject12 = GameObject.Find ("Text12");
		text12 = textObject12.transform.GetComponent<Text> ();

		textObject13 = GameObject.Find ("Text13");
		text13 = textObject13.transform.GetComponent<Text> ();

		textObject14 = GameObject.Find ("Text14");
		text14= textObject14.transform.GetComponent<Text> ();

		textObject15 = GameObject.Find ("Text15");
		text15= textObject15.transform.GetComponent<Text> ();

		textObject16= GameObject.Find ("Text16");
		text16 = textObject16.transform.GetComponent<Text> ();

		textObject17 = GameObject.Find ("Text17");
		text17 = textObject17.transform.GetComponent<Text> ();

		textObject18 = GameObject.Find ("Text18");
		text18 = textObject18.transform.GetComponent<Text> ();

		textObject19 = GameObject.Find ("Text19");
		text19 = textObject19.transform.GetComponent<Text> ();

		textObject20 = GameObject.Find ("Text20");
		text20 = textObject20.transform.GetComponent<Text> ();

		textObject21 = GameObject.Find ("Text21");
		text21 = textObject21.transform.GetComponent<Text> ();

		textObject22 = GameObject.Find ("Text22");
		text22 = textObject22.transform.GetComponent<Text> ();

		textObject23 = GameObject.Find ("Text23");
		text23 = textObject23.transform.GetComponent<Text> ();

		textObject24 = GameObject.Find ("Text24");
		text24 = textObject24.transform.GetComponent<Text> ();

		textObject25 = GameObject.Find ("Text25");
		text25 = textObject25.transform.GetComponent<Text> ();

		textObject26 = GameObject.Find ("Text26");
		text26 = textObject26.transform.GetComponent<Text> ();


		text1.text = "";
		text2.text = "";
		text3.text = "";
		text4.text = "";
		text5.text = "";
		text6.text = "";
		text7.text = "";
		text8.text = "";
		text9.text = "";
		text10.text = "";
		text11.text = "";
		text12.text = "";
		text13.text = "";
		text14.text = "";
		text15.text = "";
		text16.text = "";
		text17.text = "";
		text18.text = "";
		text19.text = "";
		text20.text = "";
		text21.text = "";
		text22.text = "";
		text23.text = "";
		text24.text = "";
		text25.text = "";
		text26.text = "";

		/* Starting the Inventory List */
		List<GameObject> InventoryList = new List<GameObject>();

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

        // Debug 
		// Debug.DrawLine (new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z), fwd * RaycastDistance);


		if (Physics.Raycast (new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z), fwd * RaycastDistance, out hit)) {
			if (hit.distance <= 1)	{

				// Opens chest 
				if (hit.transform.tag == "chest") {
					if (Input.GetKeyDown (KeyCode.E)) {

                        // Run the CreateWep function in the chest script 
						hit.collider.gameObject.GetComponent<scr_chest> ().CreateWep ();
					}
				}

				if (hit.transform.tag == "item" || hit.transform.tag == "weapon") {
					if (Input.GetKeyDown (KeyCode.E)) {
						if (InventoryList.Count < inventoryLimit) {

                            // Assign the object and add it to the list 
                            hitItem = hit.transform.gameObject;
                            InventoryList.Add(hitItem);

                            // Move and make the object invisible 
                            hitItem.GetComponent<MeshRenderer>().enabled = false;
                            hitItem.transform.position = new Vector3(0, -1000, 0);

                            // Destroying the object makes it null in the list! 
            
                        }
                    }





				}
			}
		}

        /* User Interface */


        if (InventoryList.Count > 0 && InventoryList[0] != null)    
        {
            text1.text = InventoryList[0].gameObject.GetComponent<scr_objectInformation>().name;
        }
          
        if (InventoryList.Count > 1 && InventoryList[1] != null)    
        {
            text2.text = InventoryList[1].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 2 && InventoryList[2] != null)
        {
            text3.text = InventoryList[2].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 3 && InventoryList[3] != null)
        {
            text4.text = InventoryList[3].gameObject.GetComponent<scr_objectInformation>().name;
        }
        if (InventoryList.Count > 4 && InventoryList[4] != null)
        {
            text5.text = InventoryList[4].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 5 && InventoryList[5] != null)
        {
            text6.text = InventoryList[5].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 6 && InventoryList[6] != null)
        {
            text7.text = InventoryList[6].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 7 && InventoryList[7] != null)
        {
            text8.text = InventoryList[7].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 8 && InventoryList[8] != null)
        {
            text9.text = InventoryList[8].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 9 && InventoryList[9] != null)
        {
            text10.text = InventoryList[9].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 10 && InventoryList[10] != null)
        {
            text11.text = InventoryList[10].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 11 && InventoryList[11] != null)
        {
            text12.text = InventoryList[11].gameObject.GetComponent<scr_objectInformation>().name;
        }
        if (InventoryList.Count > 12 && InventoryList[12] != null)
        {
            text13.text = InventoryList[12].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 13 && InventoryList[13] != null)
        {
            text14.text = InventoryList[13].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 14 && InventoryList[14] != null)
        {
            text15.text = InventoryList[14].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 15 && InventoryList[15] != null)
        {
            text16.text = InventoryList[15].gameObject.GetComponent<scr_objectInformation>().name;
        }
        if (InventoryList.Count > 16 && InventoryList[16] != null)
        {
            text17.text = InventoryList[16].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 17 && InventoryList[17] != null)
        {
            text18.text = InventoryList[17].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 18 && InventoryList[18] != null)
        {
            text19.text = InventoryList[18].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 19 && InventoryList[19] != null)
        {
            text20.text = InventoryList[19].gameObject.GetComponent<scr_objectInformation>().name;
        }
        if (InventoryList.Count > 20 && InventoryList[20] != null)
        {
            text21.text = InventoryList[20].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 21 && InventoryList[21] != null)
        {
            text22.text = InventoryList[21].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 22 && InventoryList[22] != null)
        {
            text23.text = InventoryList[22].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 23 && InventoryList[23] != null)
        {
            text24.text = InventoryList[23].gameObject.GetComponent<scr_objectInformation>().name;
        }

        if (InventoryList.Count > 24 && InventoryList[24] != null)
        {
            text25.text = InventoryList[24].gameObject.GetComponent<scr_objectInformation>().name;
        }
        if (InventoryList.Count > 25 && InventoryList[25] != null)
        {
            text26.text = InventoryList[25].gameObject.GetComponent<scr_objectInformation>().name;
        }



        /* Weapon Stuff */
        if (Input.GetKeyDown (KeyCode.Alpha1)) {
			WeaponSlotSelected = 1;
		}	

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			WeaponSlotSelected = 2;
		}

		/* User Interface */
		if (Input.GetKeyDown (KeyCode.I)) {
			if (interfaceOpen == true) {
				interfaceOpen = false;
			} else {
				interfaceOpen = true;
			}
		}
			
		if (interfaceOpen == true) {
			inventoryUI.GetComponent<CanvasGroup> ().alpha = 1;
			inventoryUI.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			disableInput = true;
		} else {
			inventoryUI.GetComponent<CanvasGroup> ().alpha = 0;
			inventoryUI.GetComponent<CanvasGroup> ().blocksRaycasts = false;
            disableInput = false;
        }

        /* Player's Turn */
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



	/*
	public void AlphabetUp()
	{
		InventoryList.OrderBy(go => go.name).ToList();

	}

	public void AlphabetDown()
	{
		 InventoryList.OrderBy(go => go.name).ToList();

	}



	*/









}
