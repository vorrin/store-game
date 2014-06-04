using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {


    public string zoneName;
    public List<GameObject> staffs;
    public List<Customer> customers;
    public float staffPower = .3f; //this is a percentage (.3f = 30%) 
    public int maxQueue = 5;
    public bool queueOpen = true;
    public ZoneView zoneView;
    public Customer currentlyProcessedCustomer;
    public bool processingCustomer = false;
    public float processingStartTime;
    public float processingTimeInSecondsAtHundredPercent = 40f;// Time that it takes to process a customer at 100% staff (1 staff, full training)






	// Use this for initialization
	void Start () {
        currentlyProcessedCustomer = null;
        customers = new List<Customer>();
        zoneView = GetComponent<ZoneView>();
        
	}

    public void AddCustomer(Customer customer)
    {
        customer.currentZone = this;
        customers.Add(customer);

        zoneView.UpdateCustomerNumber();
        CheckIfQueueIsFull();
    }
    public void RemoveCustomer(Customer customer)
    {
        // CHECK IF CUSTOMER BEING DISPLAYED IN PANEL
        God.instance.zonePanelManager.RemoveCustomer(customer);
        customers.Remove(customer);
        zoneView.UpdateCustomerNumber();
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
        processingStartTime = Time.time;
        //processingStartTime = Time.time;
        currentlyProcessedCustomer = customers[0];
        processingCustomer = true;
        customers[0].waiting = false ;

    }


    //void CustomerLeftForTimeout()
    //{

    //}

    void CustomerSuccesfullyProcessed()
    {
        God.instance.CustomerProcessedSuccesfully(currentlyProcessedCustomer);
        customers.RemoveAt(0);
        processingCustomer = false;
        zoneView.UpdateCustomerNumber();
        CheckIfQueueIsFull();
        if (God.instance.zonePanelManager.enabled)
        {// This simply removes the customer from the zonepanel if the customer gets processed in that time.
            God.instance.zonePanelManager.RemoveCustomer(currentlyProcessedCustomer);
        }
    }

	// Update is called once per frame
	void Update () {
        if (customers.Count != 0 && processingCustomer == false)
        {
            StartCustomerProcessing();
        }
        else if (processingCustomer == true)
        {
            
            float percentageOfCompletion = (Time.time - processingStartTime) / ( processingTimeInSecondsAtHundredPercent / staffPower );
            zoneView.UpdateProgressIndicator(percentageOfCompletion);
            if (percentageOfCompletion >= 1f)
            {
                CustomerSuccesfullyProcessed();
            }
        }


	}
}
