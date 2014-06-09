using UnityEngine;
using System.Collections;

public class RankGen : MonoBehaviour {

	public GameObject rank;

	// Use this for initialization
	void Start () {
		for(int i = 1; i < 11; i ++)
		{
			/*
			GameObject bar = (GameObject)Instantiate(rank, new Vector3(transform.position.x, transform.position.y + (i * 50), 0), Quaternion.identity);
			bar.GetComponent<RankScript>().setInformation(i + 1);
			bar.transform.parent = transform;
			*/

			GameObject bar = NGUITools.AddChild(transform.gameObject, rank);
			bar.transform.localPosition = new Vector3(transform.position.x, transform.position.y - (i * 50), transform.position.z);
			bar.GetComponent<RankScript>().setInformation(i, "None");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
