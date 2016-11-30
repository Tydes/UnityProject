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




    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        //   Debug.Log(this.gameObject.name + " was selected");
        if (this.gameObject.GetComponentInChildren<Text>().text != null)
        {
            MainName.text = this.gameObject.GetComponentInChildren<Text>().text;

            if (this.gameObject.GetComponentInChildren<Text>().name == "Text1")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc1;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text2")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc2;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text3")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc3;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text4")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc4;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text5")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc5;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text6")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc6;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text7")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc7;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text8")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc8;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text9")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc9;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text10")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc10;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text11")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc11;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text12")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc12;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text13")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc13;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text14")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc14;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text15")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc15;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text16")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc16;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text17")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc17;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text18")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc18;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text19")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc19;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text20")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc20;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text21")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc21;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text22")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc22;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text23")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc23;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text24")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc24;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text25")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc25;
            }
            if (this.gameObject.GetComponentInChildren<Text>().name == "Text26")
            {
                Description.text = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_player>().desc26;
            }
        }
    }






}