using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {

    public List<GameObject> staffs;
    public List<Customer> customers;
    public float staffPower = .3f; //this is a percentage (.3f = 30%) 
    public int maxQueue = 5;
    public bool queueOpen = true;
    public ZoneView zoneView;
    public Customer currentlyProcessedCustomer;
    public bool processingCustomer = false;
    float processingStartTime;
    public float processingTimeInSecondsAtHundredPercent = 40f;






	// Use this for initialization
	void Start () {
        currentlyProcessedCustomer = null;
        customers = new List<Customer>();
        zoneView = GetComponent<ZoneView>();
	}

    public void AddCustomer(Customer customer)
    {
        customers.Add(customer);
        zoneView.UpdateCustomerNumber();
        CheckIfQueueIsFull();
        
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

    void StartCustomerProcessing()
    {
        processingStartTime = Time.time;
        //processingStartTime = Time.time;
        currentlyProcessedCustomer = customers[0];
        processingCustomer = true;
        customers[0].waiting = false ;

    }

    void CustomerSuccesfullyProcessed()
    {
        customers.RemoveAt(0);
        processingCustomer = false;
        zoneView.UpdateCustomerNumber();
        CheckIfQueueIsFull();

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
