using UnityEngine;
using System.Collections;

public class RankScript : MonoBehaviour {

	public UILabel rankNumber;
	public UILabel rankName;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setInformation(int r, string n)
	{
		rankNumber.text = r.ToString();
		rankName.text = n.ToString();
	}
}
