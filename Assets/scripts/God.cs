using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
//using System.Xml;

/// AManager is a singleton.
/// To avoid having to manually link an instance to every class that needs it, it has a static property called
/// instance, so other objects that need to access it can just call:
///        AManager.instance.DoSomeThing();
///
public class God : MonoBehaviour {
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.

	
	private static God s_Instance = null;

    public float[] customerSpawnMinMax;

    public GameObject customersQueue;
    public UISprite[] queueButtons;
	//public  List<GameObject> zones = new List<GameObject>();
    public List<Zone> zones = new List<Zone>();
    public List<Customer> customers = new List<Customer>();
    public GameObject customerPrefab;
    public GameObject gameScreen;
    public GameObject feedbackIconPrefab;
    public GameObject zoneFeedbackIconPrefab;
    public CustomerPanelManager customerPanelManager;
    public ZonePanelManager zonePanelManager;
    public TextAsset csv;
    public List<Customer> possibleCustomersPool = new List<Customer>();
    public GameObject fader;
    public float daytimeTotal = 600f;
    public float daytimeRemaining = 600f;
    public GameObject buyPanel;
    public bool customerDragging = false;
    

    public float moodModifierBonusForBestChoice = 2f;
    public float moodModifierMalusForSecondBestChoice = 3f;
    public bool endOfDayPhase = false;
    public EndOfDayPanelManager endScreenPanel;
    public float trainingStepCost = 20f;
    public float hireNewStaffCost = 80f;
    public int currentLevel = 0;

    public DifficultyLevelEntry[] difficultyLevels;
    public UISprite difficultyLevelSprite;
    public bool gameStarted = false;
    public GameObject customerIconPrefab;
    //public AudioManager audioManager;
    [DoNotSerialize] public static float amberMoodTreshold = 9;
    [DoNotSerialize] public static float redMoodTreshold = 6;
    
    //public enum PossibleMoods {
    //    green ,
    //    amber ,
    //    red 
    //}

    //public PossibleMoods moood;

    //public List<string, UILabel> scoreLabels;



    public ScoreTracker score;
    public MainScreenIconDictionary scoreLabels;
    public GameObject mainMenuContainer;

    //SCORE TRACKING BITS





    //DEBUG XML LOADER BAD
    public IEnumerator LoadDebugXML(string xmlName)
    {
        //Root is project root-root (assets/ cd ../../)
        print(Application.dataPath);
        WWW www;
        if (Application.isWebPlayer)
        {
            www = new WWW(Application.dataPath + @"/" + xmlName);
        }
        else
        {
            www = new WWW("file://" + xmlName);
        }

        yield return www;
        DifficultyLevelEntry debugDifficulty = XmlSupport.DeserializeXml<DifficultyLevelEntry>(www.text);
        difficultyLevels[0] = debugDifficulty;
        SetDifficultyLevel(difficultyLevels[0]);
        StopCoroutine("DelayedAddingOfCustomers") ;
        
        StartCoroutine("DelayedAddingOfCustomers");

        string text = www.text;
        
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Unpause()
    {
        Time.timeScale = 1f;
    }


    public void PostScoreToPlatform()
    {
        print("Posting score to platform, once that is done.");
        print("result spending: " + score.resultSpending);
        print("total spend: " + score.totalSpendForTheDay);
        print("total nps: " + score.totalNPSForTheDay);
        print("Game day: " + (currentLevel + 1).ToString());
    }

    public void SetDifficultyLevel(DifficultyLevelEntry level)
    {
        if (currentLevel > 4) currentLevel = 4;
        difficultyLevelSprite.spriteName = "icons_LEVEL_" + (currentLevel + 1).ToString();
        customerSpawnMinMax=  new float[2] {level.minSpawnTime, level.maxSpawnTime };
        daytimeTotal = level.durationOfDay;
        daytimeRemaining = daytimeTotal;
    }

    public void CustomerLost(Customer customer)
    {
        if (customer.currentZone != null ){
            
            customer.currentZone.RemoveCustomer(customer,ZoneFeedbackIcon.Icons.DeathInQueue);
        }
        if (customerPanelManager.currentCustomer == customer)
        {
            customerPanelManager.Hide();
        }
        score.totalCustomersLost++;
        score.totalCustomersProcessed++;
        score.totalNPSForTheDay += 1f;
        List<Customer> removedList = new List<Customer>(customers);
        removedList.Remove(customer);
        customers = removedList;
        
        UpdateScoresMenu();
    }


    public void CustomerProcessedSuccessfully(Customer customer, bool upselling = false )
    {
        if (upselling)
        {
            customer.spend = customer.spend * 2;
        }
        score.totalCustomersProcessed++;
        score.totalNPSForTheDay += customer.nps;
        score.totalSpendForTheDay += customer.spend;
        customers.Remove(customer);
        UpdateScoresMenu();
    } 

    public void CustomerProcessingCompleteGod(Customer customer)
    {
        //Could check for failed upsell here ? And skip the rest and call custmerlost if upselling failed... 
        if (customer.attemptingUpsell)
        {
            string currentMood = customer.GetMoodColor();
            if (!customer.upsellable)
                //customer is NOT upsellable
            {
                if (currentMood == "green")
                {
                    //nothing happens ,sale completes normally.
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.SaleFine);
                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.SaleFine);
                    CustomerProcessedSuccessfully(customer);
                    return;
                }
                else
                {
                    //Customer sale fails through upselling
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFail);

                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.UpsellFail);
                    CustomerLost(customer);
                    return;
                }
            }
            else //customer upsellable and upsold
            {
                if (currentMood == "red")
                {
                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.UpsellFail);
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFail);

                    CustomerLost(customer);
                    return;
                }
                else
                {
                    //SUCCESS CUSTOMER UPSELLABLE AND UPSOLD
                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.UpsellFine);
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFine);

                    CustomerProcessedSuccessfully(customer,true);
                    return;

                }
            }

            
            //Feedback for upsellfail somehow?

        }
        else
        {
            //Upsell not attempted
            customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.SaleFine);
            CustomerProcessedSuccessfully(customer);
            return;
        }

    }

    void EndWorkingDay()
    {

        zonePanelManager.Hide();
        customerPanelManager.Hide();
        endOfDayPhase = true;
        customers = new List<Customer>();   
        foreach (Zone zone in zones)
        {
            zone.ClearZone();
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
            //zone.GetComponent<ZoneView>().ZoneViewStateSetup();
        }
        foreach (GameObject customerView in GameObject.FindGameObjectsWithTag("customer"))
        {
            customerView.GetComponent<CustomerView>().DestroyCustomerView();
        }
        foreach (GameObject customerIcon in GameObject.FindGameObjectsWithTag("customerIcon"))
        {
            customerIcon.GetComponent<CustomerIcon>().Die();
        }
        StopCoroutine("DelayedAddingOfCustomers");
        endScreenPanel.Display( (x) => {
            RefreshStaffBuyingMenu();
            //CALLBACK -> do whatever you like / attach a method / whatever really!
        } );
        FadeZones(false);
        PostScoreToPlatform();
        

    }

    void UpdateScoresMenu()
    {

        scoreLabels.totalNPSLabel.text = Mathf.Floor(score.totalNPSForTheDay / ( score.totalCustomersProcessed == 0 ? 1 : score.totalCustomersProcessed ) ).ToString("0");
        scoreLabels.successfulCustomers.text = Mathf.Floor(score.totalCustomersProcessed - score.totalCustomersLost).ToString("0");
        scoreLabels.failedCustomers.text = Mathf.Floor(score.totalCustomersLost).ToString("0");
        scoreLabels.totalMoneyLabel.text = Mathf.Floor(score.totalSpendForTheDay).ToString("000");


    }

    void ResetScoresAtEndOfDay()
    {
        score.totalSpendForTheDay = score.resultSpending;
        score.totalCustomersLost = 0;
        score.totalCustomersProcessed = 0;
        score.totalNPSForTheDay = 0;
        score.resultSpending = 0;
    }

    public void DeleteOldSavegames()
    {
        //Debug script, tbd
        PlayerPrefs.DeleteKey(LevelSerializer.PlayerName + "__RESUME__");
        Application.LoadLevel(0);
        foreach (LevelSerializer.SaveEntry currentSvae in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
        {
            currentSvae.Delete();
        }

    }

	private List<Customer> test = new List<Customer>();

    public void OnApplicationFocus(bool isInFocus)
    {
        if (isInFocus)
        {
            //LOADING FOR MOBILES GOES HERE
         //   LoadState();
            // not the right one ... actually should work better... //if (LevelSerializer.CanResume) LoadState();
        }
        else
        {
            print("PAUSING AND SAVING ");
            SaveState();
        }
    }



    public void StartNextDay()
    {
        ResetScoresAtEndOfDay();
        currentLevel += 1;
        SetDifficultyLevel(difficultyLevels[currentLevel]);
        endOfDayPhase = false;
        daytimeRemaining = daytimeTotal;
        RefreshStaffBuyingMenu();
        zonePanelManager.RemoveStaffHiringButtons();
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
        };
        StartNewGame();
    }

    public void PlayButtonPressed()
    {
        //if (PlayerPrefs.GetInt("playedBefore") == 0)
        //{
     //   print(PlayerPrefs.GetInt("playedBefore"));

      

        if (gameStarted == true)
        {
            StopCoroutine("DelayedAddingOfCustomers");
            StartNewGameFromSave();
            return;
        }
        
        else {
            StartCoroutine("LoadDebugXML", "DebugSettings.xml");
            StartNewGame();
        }

    }


    void StartNewGame() // This works with next levels, too
    {

        print("brand new game starting");// lie
        gameStarted = true;
        if (currentLevel >= difficultyLevels.Length)
        {
            //Quick smart so if you go over 5th day you can keep playing (same harsh difficulty level) 
            SetDifficultyLevel(difficultyLevels[difficultyLevels.Length - 1]);
        }
        else
        {
            SetDifficultyLevel(difficultyLevels[currentLevel]);
        }

        UpdateScoresMenu();

        FindTheZones();
        possibleCustomersPool = CustomerImporter.ProcessCSV(csv);
        LevelSerializer.SaveGame("base");
        AddRandomCustomer();
        StartCoroutine("DelayedAddingOfCustomers");

    }

    void StartNewGameFromSave()
    {
        print(LevelSerializer.SavedGames.Count);
        foreach (GameObject customerIcon in GameObject.FindGameObjectsWithTag("customerIcon"))
        {
            customerIcon.GetComponent<CustomerIcon>().Die();
        }
        //print("loading new game from save");
        LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Data);
        foreach (Zone zone in zones)
        {
            zone.ClearZone();
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
            //zone.GetComponent<ZoneView>().ZoneViewStateSetup();
        }
        
        //    RefreshStaffBuyingMenu();
        zonePanelManager.RemoveStaffHiringButtons();

        AddRandomCustomer();
        //StartCoroutine("DelayedAddingOfCustomers");
    }
    public void FindTheZones()
    {
        GameObject[] zoneTagObjects = GameObject.FindGameObjectsWithTag("zone") as GameObject[];

        zones = new List<Zone>();
        foreach (GameObject possibleZone in zoneTagObjects)
        {
            if (possibleZone.GetComponent<Zone>() != null)
            {
                zones.Add(possibleZone.GetComponent<Zone>());
            }
        }
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

    void AddRandomCustomer()
    {
        bool difficultCustomer = false;
        if (Random.Range(0f, 100f) > difficultyLevels[currentLevel].percentageOfHardCustomers) {
            difficultCustomer = false;
        }
        else {
            difficultCustomer = true; // means next customer is a difficulty one
        }

        bool trying = true;
        while (trying)
        {
            int num = Random.Range(0, possibleCustomersPool.Count); // This is only a test
            if (possibleCustomersPool[num].difficult == difficultCustomer)
            {
                Customer customerToBeGenerated = possibleCustomersPool[num];
                trying = false;
                AddCustomer(new Customer(customerToBeGenerated));
                return;
            }
            //Customer customerToBeGenerated =
            //You did the range wrong, 1 , 28 brings results... well, between 1 and 27. As the array size is 27, that means:
            //1- you never get the array[0] element
            //2- if you try and access array[27] everything breaks, cause it doesn't exist (it goes from 0 to 26).
        }
    }

    IEnumerator DelayedAddingOfCustomers()
    {

        yield return new WaitForSeconds(Random.Range(customerSpawnMinMax[0],customerSpawnMinMax[1]));
        if (!endOfDayPhase)
        {
            AddRandomCustomer();
            StartCoroutine("DelayedAddingOfCustomers");
        }

    }


    public void FadeZones(bool inOrOut)
    {
        if (zonePanelManager.displayingZone)
        {
            return;
        }
     //   print("fadzones");
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach( (zoneView) =>
            {
                zoneView.GetComponent<UIPlayAnimation>().Play(inOrOut);
            });
        }
    }


    public void AddCustomer(Customer customer)
    {
        AudioManager.instance.AddCustomer();
        customers.Add(customer);
        
        //GameObject customerView  = AddChild(gameScreen, customerPrefab);   


        GameObject customerView = NGUITools.AddChild(customersQueue, customerPrefab);
        customerView.transform.localPosition = new Vector3(customersQueue.GetComponent<UIGrid>().cellWidth * customersQueue.transform.childCount + 100f, customerView.transform.localPosition.y, 0f);
        customerView.GetComponent<CustomerView>().Create(customer);

        customersQueue.GetComponent<UIGrid>().Reposition();

        UpdateScoresMenu();
    }


    void UpdateCustomers()
    {
        foreach (Customer customer in customers)
        {

            if (customer.waiting)
            {
                customer.currentTimeAvailable -= Time.deltaTime;
                if (customer.currentTimeAvailable <= 0f)
                {
                    customer.Die();
                }
            }

            //Here *could* be mood enhancing magiks
        }
    }


    public void SaveState()
    {
        if (gameStarted) LevelSerializer.Checkpoint();
    }

    public void LoadState()
    {
        LevelSerializer.Resume();
    }

    public void QuitGame()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            Application.Quit();
        }
    }
    void OnApplicationQuit()
    {
        s_Instance = null;
        if (gameStarted) SaveState();
    }
    void OnApplicationPause(bool isPaused) // THIS IS WHAT SAVES THE GAME ON MOBILE! 
    {
        print("PAUSING AND " + isPaused);
        if (isPaused && gameStarted)
        {
            SaveState();
        }
    }



    public void OnDeserialized()
    {
        print("deSERIALIZING GOD");
        mainMenuContainer.GetComponent<UITweener>().PlayForward();
        FindTheZones();
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
            //zone.GetComponent<ZoneView>().ZoneViewStateSetup();
        }
        //RefreshQueueButtons();
        zonePanelManager.RemoveStaffHiringButtons();
        fader.SetActive(false);
        UpdateScoresMenu();
        RefreshStaffBuyingMenu();
        customersQueue.GetComponent<UIGrid>().Reposition();
        Unpause();
        StartCoroutine("DelayedAddingOfCustomers");
    }

    public void RefreshStaffBuyingMenu()
    {
        foreach (Transform child in buyPanel.transform)
        {
            child.gameObject.SetActive( endOfDayPhase);
            
        };
        if (endOfDayPhase) buyPanel.GetComponentInChildren<UILabel>().text = score.resultSpending.ToString("0");
        RefreshQueueButtons();    
    }

    public void RefreshQueueButtons()
    {
        foreach (UISprite button in queueButtons)
        {
            button.enabled = !endOfDayPhase;
        }
    }

    void OnGUI()
    {
    //    CheckHoveredObjects();
    }

    void Update()
    {

        //DEBUG AREA

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddRandomCustomer();
            customerPanelManager.Hide();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //LevelSerializer.SaveGame("test");
            SaveState();

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeleteOldSavegames();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            EndWorkingDay();

        }
        if (Input.GetKeyDown(KeyCode.Z))        
        {
            PlayerPrefs.DeleteAll();
        }



        if (Input.GetKeyDown(KeyCode.A))
        {

            //LevelSerializer.SavedGames[LevelSerializer.SavedGames.Count - 1];
            LoadState();
            return;
            //LevelSerializer.LoadNow(LevelSerializer)
            //List<LevelSerializer.SaveEntry> tmp =  LevelSerializer.SavedGames.Get<string>(LevelSerializer.PlayerName);


            foreach (LevelSerializer.SaveEntry currentSvae in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
            {
                if (currentSvae.Name == "test")
                {
                    LevelSerializer.LoadSavedLevel(currentSvae.Data);
                }
            }

        } 

        //END OF DEBUG


        daytimeRemaining -= Time.deltaTime;
        if (endOfDayPhase) return;

        UpdateCustomers();

        if (daytimeRemaining <= 0 )
        {
            scoreLabels.totalTimeLabel.text = "0"; 
            //DO SOMETHING LIKE MAKE CUSTOMERS DISAPPEAR AND BRING UP THE END SCREEN. 
            EndWorkingDay();
        }
        else
        {
            scoreLabels.totalTimeLabel.text = Mathf.Ceil(daytimeRemaining / 60f).ToString();
        }
        


        
    }
    public static God instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(God)) as God;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(God)) as God;
                Debug.Log("Could not locate an AManager object.  AManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.



}