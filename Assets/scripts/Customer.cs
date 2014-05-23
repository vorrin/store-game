using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour {
	public float mood = 1f;
	public string scenario = "I want to buy them things, fast!";
	public string avatarName = "customer";
	public float upsellChance = .5f;
	public float totalTimeAvailable = 600f;
	public float elapsedTime = 0f;
    public Zone currentZone;
	public bool waiting = false;

	// Use this for initialization
	void Start () {
		GetComponent<UISpriteAnimation> ().namePrefix = avatarName;
        currentZone = God.instance.Entrance.GetComponent<Zone>();
	}
        


	// Update is called once per frame
	void Update () {
		if (waiting) {
			elapsedTime += Time.deltaTime;
				}
	
	}

    public void ChangeZone(Zone zone)
    {
        Debug.Log(zone);
        if (zone == currentZone) return;
        currentZone.customers.Remove(this);
        currentZone = zone;
        zone.customers.Add(this);
    }

}
