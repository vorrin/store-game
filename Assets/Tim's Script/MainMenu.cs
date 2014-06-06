using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public static bool musicSound;
	public static bool sfxSound;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("Music") == 1)
		{
			GameObject.Find("Music").GetComponent<UIToggle>().value = true;
		}
		else
		{
			GameObject.Find("Music").GetComponent<UIToggle>().value = false;
		}

		if (PlayerPrefs.GetInt("SFX") == 1)
		{
			GameObject.Find("SFX").GetComponent<UIToggle>().value = true;
		}
		else
		{
			GameObject.Find("SFX").GetComponent<UIToggle>().value = false;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public static void MenuNav(string buttonName, bool returnMenu)
	{
		print(buttonName);
		if (returnMenu == true)
		{
			GameObject.Find("Panel1").GetComponent<UIPlayAnimation>().Play(false);
			GameObject.Find("Panel " + buttonName).GetComponent<UIPlayAnimation>().Play(false);
		}
		else if (buttonName == "Option" || buttonName == "Leader")
		{
			GameObject.Find("Panel1").GetComponent<UIPlayAnimation>().Play(true);
			GameObject.Find("Panel " + buttonName).GetComponent<UIPlayAnimation>().Play(true);
		}


		/*
		switch(buttonName)
		{
		case ("Option"):

		}
		*/

	}

	public static void CheckSounds()
	{
		musicSound = GameObject.Find("Music").GetComponent<UIToggle>().value;
		sfxSound = GameObject.Find("SFX").GetComponent<UIToggle>().value;
		if(musicSound == true)
		{
			PlayerPrefs.SetInt("Music", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Music", 0);
		}

		if(sfxSound == true)
		{
			PlayerPrefs.SetInt("SFX", 1);
		}
		else
		{
			PlayerPrefs.SetInt("SFX", 0);
		}
	}
}
