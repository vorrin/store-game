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
    public List<Customer> possibleCustomersPool = new List<Customer>();
    public UIPlayAnimation fader;


    public ScoreTracker score; 
        //public int totalCustomersProcessed = 0;
        //public int totalCustomersLost = 0;
        //public float totalNPSForTheDay = 0;


    //List<Customer> customersToRemove; 

    public void CustomerLost(Customer customer)
    {
        if (customer.currentZone != null ){
            customer.currentZone.RemoveCustomer(customer);
        }
        
        score.totalCustomersLost++;
        score.totalCustomersProcessed++;
        score.totalNPSForTheDay += 1f;
        List<Customer> removedList = new List<Customer>(customers);
        removedList.Remove(customer);
        customers = removedList;


    }

    public void CustomerProcessedSuccesfully(Customer customer)
    {
        score.totalCustomersProcessed++;
        customers.Remove(customer);
        score.totalNPSForTheDay += customer.nps;
    }

    
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


	private List<Customer> test = new List<Customer>();


    public void Start()
    {
        possibleCustomersPool = ProcessCSV();
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
        //Customer newCustomer = new Customer();
        //newCustomer.Create();


		int num = Random.Range(0, 27); // This is only a test
        //You did the range wrong, 1 , 28 brings results... well, between 1 and 27. As the array size is 27, that means:
        //1- you never get the array[0] element
        //2- if you try and access array[27] everything breaks, cause it doesn't exist (it goes from 0 to 26).

		AddCustomer(possibleCustomersPool[num]);

        //AddCustomer(newCustomer);

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
                if (customer.totalTimeAvailable <= 0f)
                {
                    customer.Die();
                }
            }

            //Here be mood enhancing magiks
        }
    }


    public List<Customer> ProcessCSV() //This is where i want to be working "Tim"
    {

        string[,] grid = CSVReader.SplitCsvGrid(csv.text);
        //string[] lines = CSVReader.SplitCsvLine(csv.text);
        //Debug.Log(lines.Length);
        //Debug.Log(lines[3]);
        //Debug.Log(grid.GetLength(1));


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
        List<Customer> generatedCustomers = new List<Customer>(); // HERE it is
        
		string[] sexType = {"MALE", "FEMALE"};
		string[] ageType = {"TEENS", "ASPIRING", "FAMILY (PREGNANT/BABY)", "MIDDLE AGED (CASUAL)", "MIDDLE AGED (BUSINESS)", "PENSIONER"};
		string[] EthnicityType = {"FAIR", "MID", "DARK"};

        for (int y = 0; y < grid.GetLength(1) - 1; y++)
        {

	        //Checks if the line is a customer line or not at all.
	        if (grid[sexIndex, y].ToUpper() != "MALE" && grid[sexIndex, y].ToUpper() != "FEMALE")
			{
				if (grid[sexIndex, y].ToUpper().Trim () == "")
				{
					break;
				}
			}
			else
			{

				string sex = grid[sexIndex, y].ToUpper ();

				if (checkList(sex, sexType) == false)
				{
					throw new System.Exception("Gender in row: " + (y + 1) + " has got a problem. Value: " + sex);
				}



				string age = grid[ageIndex , y];

				if (checkList(age, ageType) == false)
				{
					throw new System.Exception("Age in row: " + (y + 1) + " has got a problem. Value: " + age);
				}

				string ethnicity = grid[ethnicityIndex , y];

				if (checkList(ethnicity, EthnicityType) == false)
				{
					throw new System.Exception("Ethnicity in row: " + (y + 1) + " has got a problem. Value: " + ethnicity);
				}


		        string scenario = grid[scenarioIndex , y ];
				string nps = grid[npsIndex , y ];
				string[] npsWords = nps.Split(' ');
				int npsValue = 0;
				try
				{
					npsValue = int.Parse (npsWords[0]);
					if (npsValue < 1 || npsValue > 10)
					{
						throw new System.Exception("NPS in row: " + (y + 1) + " has got a problem. Value: " + nps);
					}
				}
				catch (UnityException e)
				{
					throw new System.Exception("NPS in row: " + (y + 1) + " has got a problem. Value: " + nps);
				}

		        string timeAvailable = grid[timeAvailableIndex , y];

				string[] timeWords = timeAvailable.Split(' ');

				float timeMins = 0;

				try
				{
					timeMins = float.Parse (timeWords[0]);
				}
				catch (UnityException e)
				{
					throw new System.Exception("Time Avaliable in row: " + (y + 1) + " has got a problem. Value: " + timeAvailable);
				}


		        string bestZone = grid[bestZoneIndex , y ].ToUpper();
		        string secondBestZone = grid[secondBestZoneIndex , y ].ToUpper ();

				string upsell = grid[ upsellIndex , y ].ToUpper();
		        

				bool upSellVal = false;
				

				if (upsell != "YES" && upsell != "NO")
				{
					throw new System.Exception("Upsell in row: " + (y + 1) + " has got a problem. Value: " + upsell);
				}
				else
				{

					if (upsell == "YES")
					{
						upSellVal = true;
					}
					else
					{
						upSellVal = false;
					}
				}

				string spend = grid[spendIndex, y];
				float spendVal = 0;

				try
				{
					spendVal =  float.Parse(spend);
				}
				catch (UnityException e)
				{
					throw new System.Exception("Spend in row: " + (y + 1) + " has got a problem. Value: " + spend);
				}


				//double spendVal = double.Parse(spend);

				Customer player = new Customer();
				player.Create(sex, age, ethnicity, scenario, npsValue, timeMins, bestZone, secondBestZone, upSellVal, spendVal);
				generatedCustomers.Add(player);


		        //Customer objects get created here and stored somewhere. (possibleCustomersPool)
        	}
		}
		return generatedCustomers;

    }

	bool checkList(string word, string[] checkWords)
	{
		bool match = false;
		foreach(string check in checkWords)
		{
			if (word.ToUpper() == check.ToUpper())
			{
				match = true;
				break;
			}
		}
		return match;
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