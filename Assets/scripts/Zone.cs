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


	// Use this for initialization
	void Start () {
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
	

	// Update is called once per frame
	void Update () {
        if (customers.Count != 0)
        {

        }
	}
}
