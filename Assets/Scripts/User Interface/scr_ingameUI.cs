using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class scr_ingameUI : MonoBehaviour {

	public Image wep1slot;
	public Image wep2slot;
	public Image weaponIcons;
	public Image wepslot1ui;
	public Image wepslot2ui;

	/*                        Inventory Stuff                        */
	public bool interfaceOpen = false;     // if the inventory is open
	public Text inventoryItemName;          // the name for said Item
	public Text inventoryItemDescription;   // the description for said Item
	public GameObject displayObject;        // the 3D object you display
	public GameObject panelItemPrefab;      // this is the object that is created when you pick an item up

    public InventoryBar panelItem;  // the panel item script
    public GameObject tmpPanelItem; // the panel object

    void Start () {


	}
	
	void Update () {
		
		/* Ingame UI */
		if (GameObject.FindGameObjectWithTag("Player") != null) {

			GameObject player = GameObject.FindGameObjectWithTag("Player");
			scr_player plyscr = player.GetComponent<scr_player> ();

			if (plyscr.WeaponSlotSelected == 1) {
				wep1slot.transform.localScale = new Vector2 (0.6f, 0.6f);
				wep2slot.transform.localScale = new Vector2 (0.4f, 0.4f);
			} else {
				wep1slot.transform.localScale = new Vector2 (0.4f, 0.4f);
				wep2slot.transform.localScale = new Vector2 (0.6f, 0.6f);;
			}
			if (plyscr.WeaponPlacement.transform.childCount != 0) {
				wepslot1ui.sprite = plyscr.WeaponPlacement.GetComponentInChildren<Weapons> ().MainImage;
			} 


			if (plyscr.WeaponPlacement2.transform.childCount != 0) {
				wepslot2ui.sprite = plyscr.WeaponPlacement2.GetComponentInChildren<Weapons> ().MainImage;
			} 
		}
	}





	public void OpenInventory()
	{
		if (interfaceOpen == true)
		{
			interfaceOpen = false;

            GameObject player = GameObject.FindGameObjectWithTag("Player"); // finding the player
            List<Item> inventory = player.GetComponent<scr_player>().InventoryList;
            

        }
		else
		{
			interfaceOpen = true;
			GameObject player = GameObject.FindGameObjectWithTag("Player"); // finding the player
			List<Item> inventory = player.GetComponent<scr_player>().InventoryList;

            if (inventory.Count != 0)
            {
                foreach (Item item in inventory)    // finding the player's Inventory
                {
                    tmpPanelItem = Instantiate(panelItemPrefab) as GameObject;
                    InventoryBar tmpInventoryBar = tmpPanelItem.GetComponent<InventoryBar>() as InventoryBar;

                    if (tmpInventoryBar != null) {
                        tmpInventoryBar.item = item;
                        tmpInventoryBar.itemName.text = item.itemName;
                        tmpInventoryBar.description = item.itemDesc;
                        tmpInventoryBar.gObject = item.gItem;
                    }
                    



                    tmpPanelItem.transform.SetParent(player.GetComponent<scr_player>().inventoryUI.transform.FindChild("Inventory").transform.FindChild("Left Side").transform.FindChild("InventoryList").transform.FindChild("Width"));


                }



            }
		}

	}







}
