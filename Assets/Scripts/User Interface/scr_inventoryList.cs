using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class scr_inventoryList : MonoBehaviour //, ISelectHandler
{

    private string Name;
    private string Desc;
    private GameObject PreviewObject;

    public Text MainName;
    public Text Description;
    private scr_player playerObj;

    /* Selected List Item */
    public GameObject SelectedItem;

    /* Stated when the item is dropped or used (to reset the inventory) */
    public bool DroppedOrUsed = false;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>();
        }
    }


    void Update()
    {
        if (this.gameObject.GetComponentInChildren<Text>().text == "")
        {
            this.gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            this.gameObject.GetComponent<Button>().interactable = true;
        }
    }
}
    /*
    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        playerObj.GetComponent<scr_player>().ListSelected = true;
        
        // if there's SOMETHING in the inventory - Causes bug if not in 
        if (this.gameObject.GetComponentInChildren<Text>().text != null) {
            Debug.Log("Somethingis in here!");
            // Set the name and description of the text to the inventory list's gameobject 
            // if the list is that number. Text1 = Inventory List 1.

            if (this.gameObject.GetComponentInChildren<Text>().name == "Text1")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[0].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[0].gameObject.GetComponent<scr_objectInformation>().description;

            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text2")
            {
                SelectedItem = playerObj.InventoryList[1].gameObject;
                MainName.text = playerObj.InventoryList[1].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[1].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text3")
            {
                SelectedItem = playerObj.InventoryList[2].gameObject;
                MainName.text = playerObj.InventoryList[2].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[2].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text4")
            {
                SelectedItem = playerObj.InventoryList[3].gameObject;
                MainName.text = playerObj.InventoryList[3].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[3].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text5")
            {
                SelectedItem = playerObj.InventoryList[4].gameObject;
                MainName.text = playerObj.InventoryList[4].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[4].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text6")
            {
                SelectedItem = playerObj.InventoryList[5].gameObject;
                MainName.text = playerObj.InventoryList[5].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[5].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text7")
            {
                SelectedItem = playerObj.InventoryList[6].gameObject;
                MainName.text = playerObj.InventoryList[6].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[6].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text8")
            {
                SelectedItem = playerObj.InventoryList[7].gameObject;
                MainName.text = playerObj.InventoryList[7].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[7].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text9")
            {
                SelectedItem = playerObj.InventoryList[8].gameObject;
                MainName.text = playerObj.InventoryList[8].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[8].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text10")
            {
                SelectedItem = playerObj.InventoryList[9].gameObject;
                MainName.text = playerObj.InventoryList[9].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[9].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text11")
            {
                SelectedItem = playerObj.InventoryList[10].gameObject;
                MainName.text = playerObj.InventoryList[10].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[10].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text12")
            {
                SelectedItem = playerObj.InventoryList[11].gameObject;
                MainName.text = playerObj.InventoryList[11].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[11].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text13")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[12].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[12].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text14")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[13].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[13].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text15")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[14].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[14].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text16")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[15].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[15].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text17")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[16].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[16].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text18")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[17].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[17].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text19")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[18].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[18].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text20")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[19].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[19].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text21")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[20].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[20].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text22")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[21].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[21].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text23")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[22].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[22].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text24")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[23].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[23].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text25")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[24].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[24].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text26")
            {
                SelectedItem = playerObj.InventoryList[0].gameObject;
                MainName.text = playerObj.InventoryList[25].gameObject.GetComponent<scr_objectInformation>().Name;
                Description.text = playerObj.InventoryList[25].gameObject.GetComponent<scr_objectInformation>().description;
            }








        }
        else
        {
            // This needs to be inside if before this. 
            // if the list is that number. Text1 = Inventory List 1.
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text1")
            {
                Debug.Log("test LOL");
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text2")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text3")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text4")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text5")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text6")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text7")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text8")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text9")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text10")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text11")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text12")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text13")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text14")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text15")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text16")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text17")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text18")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text19")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text20")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text21")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text22")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text23")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text24")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text25")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text26")
            {
                SelectedItem = null;
                MainName.text = "";
                Description.text = "";
                this.gameObject.GetComponentInChildren<Text>().text = "";
            }
        }
    }

    public void DropItem()
    {
        Instantiate(SelectedItem, new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, playerObj.transform.position.z), Quaternion.identity);
        playerObj.GetComponent<scr_player>().InventoryList.Remove(SelectedItem);
        DroppedOrUsed = true;
        //playerObj.GetComponent<scr_player>().InventoryList.Sort();
    }

}

*/