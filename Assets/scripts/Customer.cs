using UnityEngine;
using System.Collections;

[System.Serializable] public class Customer {
	public float mood = 1f;
	public string scenario = "I want to buy them things, fast!";
	public string avatarName = "customer";
	public float upsellChance = .5f;
	public float totalTimeAvailable = 600f;
	public float elapsedTime = 0f;
    public Zone currentZone;
	public bool waiting = false;


    public  void Create()
    {
        Debug.Log("CUSTOMER CRREATION CALLED");
        mood = 1f;
        currentZone = God.instance.entrance.GetComponent<Zone>();
    }

    public string ToString() {
        
        return "ASDASDA";
    }

	// Use this for initialization
    //void Start () {
    //    GetComponent<UISpriteAnimation> ().namePrefix = avatarName;
    //    currentZone = God.instance.entrance.GetComponent<Zone>();
    //}
        


	// Update is called once per frame
    //void Update () {
    //    if (waiting) {
    //        elapsedTime += Time.deltaTime;
    //            }
	
    //}

    public void ChangeZone(Zone zone)
    {
        Debug.Log(zone);
        if (zone == currentZone) return;
        currentZone.customers.Remove(this);
        currentZone = zone;
        zone.customers.Add(this);
    }

}
