using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {


    public string zoneName;
    public List<Customer> customers;
    public int staffPower = 20; //this is a percentage (.3f = 30%) 
    public int maxQueue = 5;
    public bool queueOpen = true;
    public List<ZoneView> zoneViews = new List<ZoneView>();
    public Customer currentlyProcessedCustomer;
    public bool processingCustomer = false;
    public float processingStartTime;
    public float processingTimeInSecondsAtHundredPercent = 40f;// Time that it takes to process a customer at 100% staff (1 staff, full training)
    public int staffNumber = 1;


	// Use this for initialization
	void Start () {
        currentlyProcessedCustomer = null;
        customers = new List<Customer>();
        foreach (ZoneView view in GetComponentsInChildren<ZoneView>())
        {
            zoneViews.Add(view);
        };

	}

    public void ClearZone()
    {
        customers = new List<Customer>();
        processingCustomer = false;
        zoneViews.ForEach((zoneView) =>
        {
            zoneView.UpdateCustomerNumber();
        });
    }

    public void OnDeserialized()
    {
        //if (LevelSerializer.IsDeserializing)
        //{
        //    //Zone is being reloaded from save
        zoneViews.ForEach((zoneView) =>
        {
            zoneView.UpdateCustomerNumber();
        });
    }

    public void AddCustomer(Customer customer)
    {
        customer.currentZone = this;
        customers.Add(customer);

        zoneViews.ForEach((zoneView) =>
        {
            zoneView.UpdateCustomerNumber();
        });
        CheckIfQueueIsFull();
    }



    public void CustomerDeadInQueue(Customer customer)
    {

   //     FireFeedbackToZones(ZoneFeedbackIcon.Icons.DeathInQueue);
        RemoveCustomer(customer, ZoneFeedbackIcon.Icons.DeathInQueue);
    }

    public void FireFeedbackToZones(ZoneFeedbackIcon.Icons icon, Customer customer){
        //if (God.instance.zonePanelManager.displayingZone)
        //{
        //    God.instance.zonePanelManager.FireFeedbackZonePanel( icon);
        //    // Here you got to fire up feedback in the zone too... 
        //}
        zoneViews.ForEach(zoneView =>
        {
            zoneView.FireFeedback(icon);
        });
    }

    public void RemoveCustomer(Customer customer, ZoneFeedbackIcon.Icons icon)
    {
        // CHECK IF CUSTOMER BEING DISPLAYED IN PANEL
        FireFeedbackToZones(icon, customer);
        God.instance.zonePanelManager.RemoveCustomer(customer, icon);
        customers.Remove(customer);
        customer.currentZone = null;
        zoneViews.ForEach((zoneView) =>
        {
            zoneView.UpdateCustomerNumber();
        });
        if (customer == currentlyProcessedCustomer)
        {
            processingCustomer = false;
        }
            CheckIfQueueIsFull();
       // StartCustomerProcessing();
    }


    void CheckIfQueueIsFull()
    {
        if (customers.Count == maxQueue)
        {
            queueOpen = false;
        }
        else
        {
            queueOpen = true;
        }
    }

    public void StartCustomerProcessing()
    {
        
        if (currentlyProcessedCustomer != null ) currentlyProcessedCustomer.waiting = true; // in case another customer was already being processed but switched out.
        processingStartTime = 0f;
        //processingStartTime = Time.time;
        currentlyProcessedCustomer = customers[0];
        processingCustomer = true;
        customers[0].waiting = false ;
    }


    void CustomerProcessingCompleteZone()
    {
        God.instance.CustomerProcessingCompleteGod(currentlyProcessedCustomer);
        //processingCustomer = false;
        //customers.RemoveAt(0);
        //zoneViews.ForEach((zoneView) =>
        //{
        //    zoneView.UpdateCustomerNumber();
        //});
        //CheckIfQueueIsFull();
        //if (God.instance.zonePanelManager.enabled)
        //{// This simply removes the customer from the zonepanel if the customer gets processed in that time.
        //    God.instance.zonePanelManager.RemoveCustomer(currentlyProcessedCustomer);
        //}
    }

	// Update is called once per frame
	void Update () {
        if (customers.Count != 0 && processingCustomer == false)
        {
            StartCustomerProcessing();
        }
        else if (processingCustomer == true)
        {
            processingStartTime += Time.deltaTime;
            float percentageOfCompletion = (processingStartTime) / ( processingTimeInSecondsAtHundredPercent  / ( staffPower /100f)  );
            zoneViews.ForEach((zoneView) =>
            {
                zoneView.UpdateProgressIndicator(percentageOfCompletion);
            });
            if (percentageOfCompletion >= 1f)
            {
                CustomerProcessingCompleteZone();
            }
        }
	}
}
