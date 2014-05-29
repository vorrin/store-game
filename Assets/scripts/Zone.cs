using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {

    public List<GameObject> staffs;
    public List<Customer> customers;
    public float staffPower = 30f;
    public int maxQueue = 5;
    public bool queueOpen = true;
    public ZoneView zoneView;
    public Customer currentlyProcessedCustomer;
    public bool processingCustomer = false;


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
        currentlyProcessedCustomer = customers[0];
        processingCustomer = true;
        customers[0].waiting = false ;

    }

	// Update is called once per frame
	void Update () {
        if (customers.Count != 0 && processingCustomer == false)
        {
            StartCustomerProcessing();
        }


	}
}
