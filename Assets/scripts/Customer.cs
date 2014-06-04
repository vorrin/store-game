﻿using UnityEngine;
using System.Collections;


[System.Serializable] public class Customer {
	public string sex;
	public string age;
	public string ethnicity;
	public string scenario;
	public float nps = 1f;
	public string avatarName = "customer";
	public bool upsell;
	public float totalTimeAvailable = 60f; // 1f == 1 second
	public string bestZone;
	public string secondBestZone;
	public float spend;
    //public float elapsedTime = 0f;
    public Zone currentZone;
	public bool waiting = false;
    public CustomerView customerView;

	public  enum ZoneMatchingResults {
		Fail,
		SecondBest,
		Best
	}

	public void Create(string gender, string age, string ethnicity, string scenario, int npsValue, float timeMins, string bestZone, string secondBestZone, bool upSellVal, float spend)

    {
        //I think validating data for customers here would be a good idea.
        // Ie. sex, age, ethnicity, nps (range 1 -10), upsell (yes/no)

        //System.Enum.IsDefined(typeof(CustomerSex), sex);
        //Debug.Log("CUSTOMER CREATION CALLED");

		this.sex = gender;
		this.age = age;
		this.ethnicity = ethnicity;
		this.scenario = scenario;
		this.nps = npsValue;
		this.totalTimeAvailable = timeMins * 6f; // Converting from minutes to seconds. Dividing the XLS values by a factor of 10 currently (30 mins is kinda crazy)
		this.bestZone = bestZone;
		this.secondBestZone = secondBestZone;
		this.upsell = upSellVal;
		this.spend = spend;

        //scenario = "Hello, I'm looking for some more mobile prowess in my current pocket monster. Please could you point me towards the beefiest specimen in your shop, so I can play all the 3D games like a breeze? Thank you person!";

        waiting = true;
        //ChangeZone(God.instance.entrance.GetComponent<Zone>());
    }

    public ZoneMatchingResults DroppedInZone(Zone zone)
    {
        //HERE THE DIFFERENT RESULTS WILL COME AND MAKE ICONS
        int res = 1;
        if (zone.zoneName == bestZone ) // best
        {
            return ZoneMatchingResults.Best;
        }
        else if (zone.zoneName == secondBestZone) {
            return ZoneMatchingResults.SecondBest;
        }
        else
        {
            return ZoneMatchingResults.Fail;
        }
    }

    public void Die()
    {

        
        //Customer dies while in zone...
        if (currentZone != null)
        {
            //Zone takes care of zonepanel removing too, so this all we need if the customer is in a zone.
            currentZone.RemoveCustomer(this);
        }
            //customer dies while in queue...
        else
        {
            //foreach (Transform customerViewTrans in God.instance.customersQueue.transform)
            //{
            //    CustomerView customerView = customerViewTrans.GetComponent<CustomerView>();
            //    if (customerView.customerModel == this)
            //    {
            //        GameObject.Destroy(customerView.gameObject);
        // THIS SHOULD ALL BE MOVED IN CUSTOMERVIEW...
            if (customerView.gameObject)
            {
                if (customerView.GetComponent<DragDropCustomer>().dragging)
                {
                    //This is so if the customer dies of running out of time when dragged, all looks good.
                    customerView.EndDrag();
                    GameObject.Destroy(customerView.gameObject);
                }
                else
                {
                    God.instance.customersQueue.GetComponent<UIGrid>().repositionNow = true;
                }
                
            }
            //    }
            //}
        }

        God.instance.CustomerLost(this);
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

    //public void ChangeZone(Zone zone)
    //{
    //    if (currentZone)
    //    {
    //        if (zone == currentZone) return;
    //        currentZone.customers.Remove(this);
    //    }
    //    currentZone = zone;
    //    zone.customers.Add(this);
    //}


}
