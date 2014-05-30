using UnityEngine;
using System.Collections;

[System.Serializable] public class Customer {
	public float mood = 1f;
	public string scenario = "I want to buy them things, fast!";
	public string avatarName = "customer";
	public float upsellChance = .5f;
	public float totalTimeAvailable = 60f; // 1f == 1 second
    //public float elapsedTime = 0f;
    public Zone currentZone;
	public bool waiting = false;

    public enum CustomerSex
    {
        Male,
        Female
    }

    public  void Create(string sex ,string age ,string ethnicity ,string scenario ,string nps ,string timeAvailable ,string bestZone ,string secondBestZone ,string upsell ,string spend  )
    {
        
           
        Debug.Log("CUSTOMER CRREATION CALLED");
        scenario = "Hello, I'm looking for some more mobile prowess in my current pocket monster. Please could you point me towards the beefiest specimen in your shop, so I can play all the 3D games like a breeze? Thank you person!";
        mood = 1f;
        waiting = true;
        //ChangeZone(God.instance.entrance.GetComponent<Zone>());
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
        if (currentZone)
        {
            if (zone == currentZone) return;
            currentZone.customers.Remove(this);
        }
        currentZone = zone;
        zone.customers.Add(this);
    }

}
