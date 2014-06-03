using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ZonePanelManager : MonoBehaviour {

    public Zone currentZone;
    public ZoneGrid queue;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Display(Zone zone)
    {
        currentZone = zone;
        ClearZonePanel();
        //DEBUG tmp hack
        GetComponent<UIPlayAnimation>().Play(true);
        God.instance.fader.clipName = "ZoneFaderAnim";
        God.instance.fader.Play(true);
        PopulateZonePanel();

    }

    public void ReorderCustomerList( BetterList<Transform> list ){ // THIS HAPPENS IN REPOSITION, ZoneGrid.cs !!!! 
        //print(" THE LIST IS SIZED SO : " + list.size);
        if (list.size == 0) return;
        List<Customer> customers = new List<Customer>();
        foreach (Transform customerTransform in list)
        {
            customers.Add(customerTransform.GetComponent<CustomerView>().customerModel);
        }
        currentZone.customers = customers;
        //currentZone.currentlyProcessedCustomer.waiting = false;
        if (customers[0] != currentZone.currentlyProcessedCustomer) // First customer has changed.
        {
            //Debug.Log("ALL IS DIFFERENT FOREVERS " );
            print("ALL IS DIFFERENT FOREVERS " + customers[0] + " and " + currentZone.currentlyProcessedCustomer);
            currentZone.StartCustomerProcessing();
        }
    }

    public void PopulateZonePanel()
    {
        ClearZonePanel();

        if (currentZone.customers.Count == 0) return;
        foreach (Customer customer in currentZone.customers)
        {
            GameObject currentCustomer  = NGUITools.AddChild(queue.gameObject, God.instance.customerPrefab);

            //GameObject currentCustomer = Instantiate(God.instance.customerPrefab) as GameObject;
            currentCustomer.transform.localScale = Vector3.one;
            currentCustomer.GetComponent<CustomerView>().Create(customer);
            currentCustomer.transform.parent = queue.transform;
        }
        queue.Reposition();
    }

    public void ClearZonePanel()
    {
        foreach (Transform child in queue.transform)
        {
            if (!child.name.Contains("Spot"))
            {
                print("CLEARING!");

                
                Destroy(child.gameObject);
                child.parent = null;
            }
        }
    }

    public void RemoveCustomer(Customer processedCustomer)
    {
        if (!this.enabled)
        {
            return;
        }
        foreach (Transform customerView in queue.transform)
        {
            if (customerView.name.Contains("Spot")) continue;
            if (customerView.GetComponent<CustomerView>().customerModel == processedCustomer)
            {
                Destroy(customerView.gameObject);
                customerView.parent = null;
                queue.Reposition();
                return;
            }
            
        }
    }


    public void Hide()
    {
        GetComponent<UIPlayAnimation>().Play(false);
        God.instance.fader.clipName = "FaderAnim";
        God.instance.fader.Play(false);
        GetComponent<UIPlayAnimation>().disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
        currentZone.GetComponent<UIPlayAnimation>().Play(false);
    }

}
