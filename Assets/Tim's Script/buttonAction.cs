using UnityEngine;
using System.Collections;

//Tim's script, probably unused right now.
public class buttonAction: MonoBehaviour {

	public string buttonName;
	public bool returnMenu;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnClick()
	{
        //if (returnMenu == true)
        //{
        //    MainMenu.buttonDisable = false;
        //}
        //else
        //{
        //    MainMenu.buttonDisable = true;
        //}

		MainMenu.MenuNav(buttonName, returnMenu);

		//print("Clicked");
		//GameObject.Find("Panel1").GetComponent<UIPlayAnimation>().Play(true);
	}
}
