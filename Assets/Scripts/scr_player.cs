using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class scr_player : MonoBehaviour
{

    /* movement */
    private int SpaceToMove = 10;
    private int RotateAngle = 90;
    private bool CanMove = true;
    private bool disableInput = false;

    private bool isMoving = false;
    private bool Strafing = false;
    private Vector3 ToDistance;

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
    public scr_objectInformation ObjectScript;

    /* Invetory Stuff */
    private int inventoryLimit = 25;
    private int currentInventoryNumber = 0;

    public List<Item> InventoryList = new List<Item>();

    /* This enables the "Drop/Use" function */
    public bool ListSelected = false;

    /* Canvas group - Need to find them in the world space at the start event*/
    public GameObject inventoryUI;

    /* UI Object */
    public GameObject UIobject;

    void Start()
    {
        followMeObject = GameObject.Find("ToFollow(Clone)");
        inventoryUI = GameObject.Find("InventoryCanvasGroup");

        UIobject = GameObject.Find("UI");

        /* Starting the Inventory List */
    //    List<Item> InventoryList = new List<Item>();

        Debug.Log(InventoryList.Count);


    }




    void Update()
    {
    

        /* Drops the current weapon - needs work? */
        /*
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

        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        /* Player Movement */
        RaycastHit hit;

        /* Detects Wall and Chest */
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), fwd * RaycastDistance, out hit))
        {
            if (hit.distance <= 1)
            {
                if (hit.transform.tag == "chest") // Opens chest 
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.gameObject.GetComponent<scr_chest>().CreateWep();  // Run the CreateWep function in the chest script 
                    } 
                }

                if (hit.transform.tag == "item" || hit.transform.tag == "weapon")
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (InventoryList.Count < inventoryLimit)
                        {
                            hitItem = hit.transform.gameObject; // Assign the object and add it to the list 
                            currentInventoryNumber++;
                            ObjectScript = hitItem.GetComponent<scr_objectInformation>();
                            Item tmpItem = new Item(ObjectScript.nameOfItem, ObjectScript.description, ObjectScript.iconOfItem, ObjectScript.gObject, ObjectScript.valueOfItem);
                            InventoryList.Add(tmpItem);
                            Destroy(hitItem);
                        }
                    }
                }
            }
        }
        /* User Interface */
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameObject.Find("UI").GetComponent<scr_ingameUI>().OpenInventory();
        }

        /* Weapon Stuff */
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponSlotSelected = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponSlotSelected = 2;
        }

        if (UIobject.GetComponent<scr_ingameUI>().interfaceOpen == true)
        {
            inventoryUI.GetComponent<CanvasGroup>().alpha = 1;
            inventoryUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
            disableInput = true;
        }
        else
        {
            inventoryUI.GetComponent<CanvasGroup>().alpha = 0;
            inventoryUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
            disableInput = false;
        }

        /* Player's Turn */
        GameObject gogame = GameObject.Find("Game");
        scr_game gscript = gogame.GetComponent<scr_game>();

        if (gscript.turn == "players")
        {
            if (disableInput == false)
            {
                GameObject Followme = GameObject.Find("Main Camera");
                scr_camera scrcam = Followme.GetComponent<scr_camera>();

                if (isMoving == false)
                {
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        ToDistance = transform.position + Vector3.forward * SpaceToMove;

                        isMoving = true;
                    }

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        ToDistance = transform.position + Vector3.left * SpaceToMove;

                        isMoving = true;
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        ToDistance = transform.position + Vector3.back * SpaceToMove;

                        isMoving = true;
                    }

                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        ToDistance = transform.position + Vector3.right * SpaceToMove;

                        isMoving = true;
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, ToDistance) > 0)
                    {
                        transform.position = Vector3.Lerp(transform.position, ToDistance, 0.1f);
                        isMoving = false;
                    }
                }


                /* Rotation */
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    scrcam.CanRotate = true;
                    transform.Rotate(Vector3.down * RotateAngle);
                    followMeObject.transform.Rotate(Vector3.down * RotateAngle);

                    /* Camera Rotation */
                    Followme.transform.RotateAround(transform.position, Vector3.down, CameraRotate);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    scrcam.CanRotate = true;
                    transform.Rotate(Vector3.up * RotateAngle);
                    followMeObject.transform.Rotate(Vector3.up * RotateAngle);

                    /* Camera Rotation */
                    Followme.transform.RotateAround(transform.position, Vector3.up, CameraRotate);
                }

                if (!Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    scrcam.CanRotate = false;
                }
            }
        }
    }
}


public class Item   {

    public string itemName;
    public string itemDesc;
    public Texture2D itemIcon;
    public GameObject gItem;
    public int itemValue;

    public Item(string name, string desc, Texture2D icon, GameObject goitem, int value)
    {
        itemName = name;
        itemDesc = desc;
        itemIcon = icon;
        gItem = goitem;
        itemValue = value;
    }
}
