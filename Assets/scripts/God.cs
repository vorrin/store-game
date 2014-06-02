using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

/// AManager is a singleton.
/// To avoid having to manually link an instance to every class that needs it, it has a static property called
/// instance, so other objects that need to access it can just call:
///        AManager.instance.DoSomeThing();
///
public class God : MonoBehaviour {
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.

	
	private static God s_Instance = null;

    public int customerCountDown = 5;
    public GameObject customersQueue;
	//public  List<GameObject> zones = new List<GameObject>();
    public GameObject[] zones;
    public List<Customer> customers = new List<Customer>();
    public GameObject customerPrefab;
    public GameObject gameScreen;
    public GameObject feedbackIconPrefab;
    public CustomerPanelManager customerPanelManager;
    public ZonePanelManager zonePanelManager;
    public TextAsset csv;
    public Customer[] possibleCustomersPool;
    public UIPlayAnimation fader;

    
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static God instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (God)) as God;
			}
			
			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AManager");
				s_Instance = obj.AddComponent(typeof (God)) as God;
				Debug.Log ("Could not locate an AManager object.  AManager was Generated Automaticly.");
			}
			
			return s_Instance;
		}
	}
	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit() {
		s_Instance = null;
	}




    public void Start()
    {
        ProcessCSV();
        zones = GameObject.FindGameObjectsWithTag("zone") as GameObject[];
        StartCoroutine(DelayedAddingOfCustomers());

    }

   
        static public GameObject AddChild(GameObject parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = prefab.transform.localPosition;
                t.localRotation = prefab.transform.localRotation;
                t.localScale = prefab.transform.localScale;
                go.layer = parent.layer;
            }
            return go;
        }

    void TestingGame()
    {
        Customer newCustomer = new Customer();
        newCustomer.Create();
        AddCustomer(newCustomer);

    }

    IEnumerator DelayedAddingOfCustomers()
    {
        TestingGame();
        yield return new WaitForSeconds(customerCountDown);
        StartCoroutine(DelayedAddingOfCustomers());

    }


    public void FadeZones(bool inOrOut)
    {
        foreach (GameObject zone in zones)
        {
            zone.GetComponent<UIPlayAnimation>().Play(inOrOut);
        }
    }


    public void AddCustomer(Customer customer)
    {
        customers.Add(customer);
        
        //GameObject customerView  = AddChild(gameScreen, customerPrefab);   


        GameObject customerView = NGUITools.AddChild(customersQueue, customerPrefab);
        customerView.transform.localPosition = new Vector3(customersQueue.GetComponent<UIGrid>().cellWidth * customersQueue.transform.childCount + 100f, customerView.transform.localPosition.y, 0f);
        customerView.GetComponent<CustomerView>().Create(customer);

        customersQueue.GetComponent<UIGrid>().Reposition();
        //customersQueue.GetComponent<UIPanel>().Refresh();
        //BoxCollider coll = customersQueue.GetComponent<BoxCollider>();
      //  customerView.GetComponent<UISprite>().panel.Refresh() ;



        //coll.size = new Vector3(coll.size.x + customersQueue.GetComponent<UIGrid>().cellWidth * 2, coll.size.y, 0f);
        //collider.bounds.size = new Vector3(collider.bounds.size.x + 50, collider.bounds.size.y, 0f);
    }


    void UpdateCustomers()
    {
        foreach (Customer customer in customers)
        {

            if (customer.waiting)
            {
                customer.totalTimeAvailable -= Time.deltaTime;
            }

            //Here be mood enhancing magiks
        }
    }


    void ProcessCSV()
    {

        string[,] grid = CSVReader.SplitCsvGrid(csv.text);
        //string[] lines = CSVReader.SplitCsvLine(csv.text);
        //Debug.Log(lines.Length);
        //Debug.Log(lines[3]);
        Debug.Log(grid.GetLength(1));


        int sexIndex = 0;
        int ageIndex = 1;
        int ethnicityIndex = 2;
        int scenarioIndex = 3;
        int npsIndex = 6;
        int timeAvailableIndex = 7;
        int bestZoneIndex = 8;
        int secondBestZoneIndex = 9;
        int upsellIndex = 10;
        int spendIndex = 11;
        List<Customer> generatedCustomers = new List<Customer>();
        
        for (int y = 0; y < grid.GetLength(0); y++)
            {
                //Checks if the line is a customer line or not at all.
                if (grid[sexIndex, y] != "Male" && grid[sexIndex, y] != "Female") continue;


                string sex = grid[sexIndex, y];
                

                string age = grid[ageIndex , y];
                string ethnicity = grid[ ethnicityIndex , y];
                string scenario = grid[scenarioIndex , y ];
                string nps = grid[npsIndex , y ];
                string timeAvailable = grid[timeAvailableIndex , y];
                string bestZone = grid[bestZoneIndex , y ];
                string secondBestZone = grid[secondBestZoneIndex , y ];
                string upsell = grid[ upsellIndex , y ];
                string spend = grid[spendIndex, y ];


                //Customer objects get created here and stored somewhere. (possibleCustomersPool)
                
                
        }
        
    }

    void Update()
    {
        UpdateCustomers();
        
        //DEBUG AREA
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("God IS PUSHING");
            TestingGame();
            customerPanelManager.Hide();
        }
    }
	

}