﻿using UnityEngine;
using System.Collections;


[System.Serializable] public class Customer {
	public string sex;
	public string age;
	public string ethnicity;
	public string scenario;
    public string type;
    public string experienceLoop;
	public float nps = 1f;
	public string avatarName = "customer";
	public bool upsellable;
    public bool attemptingUpsell = false;
    public float currentTimeAvailable = 60f; // 1f == 1 second
    public float initialTimeAvailable = 60f; // 1f == 1 second
	public string bestZone;
	public string secondBestZone;
	public float spend;
    //public float elapsedTime = 0f;
    public Zone currentZone;
	public bool waiting = false;
    public CustomerView customerView;
    public bool difficult;


    

	public  enum ZoneMatchingResults {
		Fail,
		SecondBest,
		Best
	}

    public Customer()
    {

    }

    public Customer(string gender, string age, string ethnicity, string scenario, string type, string experienceLoop , int npsValue, float timeMins, string bestZone, string secondBestZone, bool upSellVal, float spend, string difficulty)
    {
    //}

    //public void Create(string gender, string age, string ethnicity, string scenario, int npsValue, float timeMins, string bestZone, string secondBestZone, bool upSellVal, float spend)

    //{
        //I think validating data for customers here would be a good idea.
        // Ie. sex, age, ethnicity, nps (range 1 -10), upsell (yes/no)

        //System.Enum.IsDefined(typeof(CustomerSex), sex);

		this.sex = gender;
		this.age = age;
		this.ethnicity = ethnicity;
		this.scenario = scenario;
        this.type = type;
        this.experienceLoop = experienceLoop;
		this.nps = npsValue;

        this.initialTimeAvailable = timeMins * 60f; // Converting from minutes to seconds. Dividing the XLS values by a factor of 10 currently (30 mins is kinda crazy)
        this.currentTimeAvailable = this.initialTimeAvailable;

		this.bestZone = bestZone;
		this.secondBestZone = secondBestZone;
		this.upsellable = upSellVal;
		this.spend = spend;
        if (difficulty == "HARD")
        {
            this.difficult = true;
        }
        else
        {
            this.difficult = false;
        }


        ////DEBUG NOT THE RIGHT WAY, DO DIFFERENT LATER.
        //if (Random.value < 0.5f)
        //{
        //    avatarName = "customer";
        //}
        //else
        //{
        //    avatarName = "customer1";
        //}
        avatarName = (this.sex + "_" + this.age + "_" + this.ethnicity).ToLower();

        //scenario = "Hello, I'm looking for some more mobile prowess in my current pocket monster. Please could you point me towards the beefiest specimen in your shop, so I can play all the 3D games like a breeze? Thank you person!";

        waiting = true;
        //ChangeZone(God.instance.entrance.GetComponent<Zone>());
    }



    public Customer(Customer customerCloneBase)
    {

        this.sex = customerCloneBase.sex;
        this.age = customerCloneBase.age;
        this.ethnicity = customerCloneBase.ethnicity;
        this.scenario = customerCloneBase.scenario;
        this.type = customerCloneBase.type;
        this.experienceLoop = customerCloneBase.experienceLoop;
        this.nps = customerCloneBase.nps;
        //DEBUG avatarname not to be used?
        this.avatarName = customerCloneBase.avatarName;
        this.initialTimeAvailable = customerCloneBase.initialTimeAvailable;
        this.currentTimeAvailable = customerCloneBase.initialTimeAvailable;

        this.bestZone = customerCloneBase.bestZone;
        this.secondBestZone = customerCloneBase.secondBestZone;
        this.upsellable = customerCloneBase.upsellable;
        this.spend = customerCloneBase.spend;
        this.difficult = customerCloneBase.difficult;
        waiting = true;
    }

    public ZoneMatchingResults DroppedInZone(Zone zone)
    {
        //HERE THE DIFFERENT RESULTS WILL COME AND MAKE ICONS

      
        if (zone.zoneName == bestZone.Trim() ) // best
        {
            nps = Mathf.Clamp(nps + God.instance.moodModifierBonusForBestChoice, 1f, 10f);
            return ZoneMatchingResults.Best;
        }
        else if (zone.zoneName == secondBestZone.Trim())
        {
            nps = nps - God.instance.moodModifierMalusForSecondBestChoice;
            if (nps < 1f) nps = 1;
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
            currentZone.CustomerDeadInQueue(this);
        }
            //customer dies while in queue...
        else
        {
            //foreach (Transform customerViewTrans in God.instance.customersQueue.transform)
            //{
            //    CustomerView customerView = customerViewTrans.GetComponent<CustomerView>();
            //    if (customerView.customerModel == this)
            //    {
        // THIS SHOULD ALL BE MOVED IN CUSTOMERVIEW...
            if (customerView.gameObject)
            {
                if (customerView.GetComponent<DragDropCustomer>().dragging)
                {
                    //This is so if the customer dies of running out of time when dragged, all looks good.
                    customerView.EndDrag();
                    customerView.DestroyCustomerView();

                }
                else// Customer is still in the queue
                {
                    customerView.DestroyCustomerView(new System.Action(() => God.instance.customersQueue.GetComponent<UIGrid>().repositionNow = true));
                }

                
            }
            //    }
            //}
        }

        God.instance.CustomerLost(this);
    }


    public void BackToQueueFromZone()
    {
        currentZone.RemoveCustomer(this, ZoneFeedbackIcon.Icons.Null);
        waiting = true;
        currentZone = null; 
        God.instance.customers.Remove(this); //avoids doubleadding
        God.instance.AddCustomer(this);
    }

    public string GetMoodColor()
    {
        string moodColor = "green";
        if (nps > God.amberMoodTreshold)
        {
            moodColor = "green";
        }
        else if (nps > God.redMoodTreshold)
        {
            moodColor = "amber";
        }
        else
        {
            moodColor = "red";
        }
        return moodColor;

    }
    public int GetPromoterOrDetractorState()
    {
        string color = this.GetMoodColor();
        switch (color)
        {
            case "green":
                return 1;
                break;
            case "amber":
                return 0;
                break;
            case "red":
                return -1;
                break;
            default:
                Debug.Log("this customer doesn't have a mood value: " + avatarName);
                throw new System.Exception("A customer doesn't have a mood value.");
                break;
        }
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
