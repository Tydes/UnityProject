using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class scr_inventoryList : MonoBehaviour, ISelectHandler
{

    private string Name;
    private string Desc;
    private GameObject PreviewObject;

    public Text MainName;
    public Text Description;

    private scr_player playerObj;


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
            this.gameObject.GetComponent<Selectable>().interactable = false;
        }
        else
        {
            this.gameObject.GetComponent<Selectable>().interactable = true;
        }
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if (this.gameObject.GetComponentInChildren<Text>().text != null)
        {


            MainName.text = this.gameObject.GetComponentInChildren<Text>().text;

            if (this.gameObject.GetComponentInChildren<Text>().name == "Text1")
            {
                Description.text = playerObj.InventoryList[0].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text2")
            {
                Description.text = playerObj.InventoryList[1].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text3")
            {
                Description.text = playerObj.InventoryList[2].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text4")
            {
                Description.text = playerObj.InventoryList[3].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text5")
            {
                Description.text = playerObj.InventoryList[4].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text6")
            {
                Description.text = playerObj.InventoryList[5].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text7")
            {
                Description.text = playerObj.InventoryList[6].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text8")
            {
                Description.text = playerObj.InventoryList[7].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text9")
            {
                Description.text = playerObj.InventoryList[8].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text10")
            {
                Description.text = playerObj.InventoryList[9].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text11")
            {
                Description.text = playerObj.InventoryList[10].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text12")
            {
                Description.text = playerObj.InventoryList[11].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text13")
            {
                Description.text = playerObj.InventoryList[12].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text14")
            {
                Description.text = playerObj.InventoryList[13].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text15")
            {
                Description.text = playerObj.InventoryList[14].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text16")
            {
                Description.text = playerObj.InventoryList[15].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text17")
            {
                Description.text = playerObj.InventoryList[16].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text18")
            {
                Description.text = playerObj.InventoryList[17].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text19")
            {
                Description.text = playerObj.InventoryList[18].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text20")
            {
                Description.text = playerObj.InventoryList[19].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text21")
            {
                Description.text = playerObj.InventoryList[20].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text22")
            {
                Description.text = playerObj.InventoryList[21].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text23")
            {
                Description.text = playerObj.InventoryList[22].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text24")
            {
                Description.text = playerObj.InventoryList[23].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text25")
            {
                Description.text = playerObj.InventoryList[24].gameObject.GetComponent<scr_objectInformation>().description;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text26")
            {
                Description.text = playerObj.InventoryList[25].gameObject.GetComponent<scr_objectInformation>().description;
            }
        }
        
    }






}