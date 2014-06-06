using UnityEngine;
using System.Collections;

public class RankScript : MonoBehaviour {

	public UILabel rankNumber;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setInformation(int r)
	{
		rankNumber.text = r.ToString();
	}
}
