using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
//using SimpleJSON;
using System.Net;


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
    public float trainingStepCost = 100f;
    public float hireNewStaffCost = 500f;
    public int currentLevel = 0; //current difficulty level
    public DifficultyLevelEntry[] difficultyLevels;
    public UISprite difficultyLevelSprite;
    public bool gameStarted = false;
    public GameObject customerIconPrefab;
    [DoNotSerialize] public static float amberMoodTreshold = 8;
    [DoNotSerialize] public static float redMoodTreshold = 6;
    public ScoreTracker score;
    public MainScreenIconDictionary scoreLabels;
    public GameObject mainMenuContainer;
    string url = "http://vodafone-wayofretail.ambidectcloud.com/CompetitionApi/AddScore";
    string testingUser = "0980808a-2afc-411d-96a5-009487091a61";
    string testingContentKey = "91fa1062-255b-40fb-a108-3fa783bff6cd";



    //DEBUG rest request:
    HttpWebRequest webRequest;
        


    IEnumerator PostData(JSONObject json)
    {
        //WWWForm testWWW = new WWWForm();
        //testWWW.AddField("Score","12");
        //testWWW.AddField("ContentKey","91FA1062-255B-40FB-A108-3FA783BFF6CD");
        //testWWW.AddField("Level","31");
        //testWWW.AddField("User","0980808A-2AFC-411D-96A5-009487091A61");
        //testWWW.AddField("Date","2014-06-06 00,00,00Z");
        //testWWW.AddField("Powerbar","21");
        //WWW www = new WWW(url, testWWW);

        //string input = @"{""Date"":""" + System.DateTime.Now.ToString("mm/dd/yyyyTHH:mm:ss") +@"""}";
        //print(input);
        Hashtable headers = new Hashtable();
        headers.Add("Content-Type", "application/json");
    
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json.ToString());
        print(body[1]);
        WWW www = new WWW(url, body, headers);
        
        yield return www;
        if (www.error != null)
        {
            print("SUCCESS");
        }
    }

    //DEBUG XML LOADER BAD
    public IEnumerator LoadDebugXML(string xmlName)
    {
        //Root is project root-root (assets/ cd ../../)
        //This is just a debug thing for JEremy, calling this shall be REMOVED from a final version.
        print(" LOADING XML, THSI SHOULD NOT BE HAPPENING IN THE FINAL VERSION");
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

    [ContextMenu("AreWeOnINternet")]
    public void PostScoreToPlatform()
    {
        Debug.ClearDeveloperConsole();
        print("Posting score to platform, once that is done.");
        print("result spending: " + score.resultSpending);
        print("total spend: " + score.totalSpendForTheDay);
        print("total nps: " + score.totalNPSForTheDay);
        print("Game day: " + (currentLevel + 1).ToString());

        JSONObject currencyJson = new JSONObject();
        currencyJson.AddField("Date", System.DateTime.Now.ToString("mm/dd/yyyyTHH:mm:ss") );
        currencyJson.AddField("Powerbar", "Currency");
        currencyJson.AddField("ContentKey", testingContentKey);
        currencyJson.AddField("User", testingUser);
        currencyJson.AddField("Level", currentLevel + 1);
        currencyJson.AddField("Score", score.resultSpending );
        StartCoroutine(PostData(currencyJson));
        
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
                    CustomerProcessedSuccessfully(customer);
                    return;
                }
                else
                {
                    //Customer sale fails through upselling
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFail);
                    CustomerLost(customer);
                    return;
                }
            }
            else //customer upsellable and upsold
            {
                if (currentMood == "red")
                {
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFail);
                    CustomerLost(customer);
                    return;
                }
                else
                {
                    //SUCCESS CUSTOMER UPSELLABLE AND UPSOLD
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
        
        if (!isInFocus)
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

        if (gameStarted == true)
        {
            StopCoroutine("DelayedAddingOfCustomers");
            StartNewGameFromSave();
            return;
        }
        
        else {
        //    StartCoroutine("LoadDebugXML", "DebugSettings.xml");
            StartNewGame();
        }

    }


    void StartNewGame() // This works with next levels, too
    {

        gameStarted = true;
        
        if (currentLevel >= difficultyLevels.Length)
        {
            //Trick to keep replaying the 5th day after having passed that
            currentLevel = difficultyLevels.Length - 1;
        }

        SetDifficultyLevel(difficultyLevels[currentLevel]);

        UpdateScoresMenu();

        FindTheZones();
        possibleCustomersPool = CustomerImporter.ProcessCSV(csv);
        LevelSerializer.SaveGame("base");
        AddRandomCustomer();
        StartCoroutine("DelayedAddingOfCustomers");
    }

    void StartNewGameFromSave()
    {
        foreach (GameObject customerIcon in GameObject.FindGameObjectsWithTag("customerIcon"))
        {
            customerIcon.GetComponent<CustomerIcon>().Die();
        }
        LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Data);
        foreach (Zone zone in zones)
        {
            zone.ClearZone();
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
        }
        
        zonePanelManager.RemoveStaffHiringButtons();
        AddRandomCustomer();
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

    // Ensure that the instance is destroyed when the game is stopped in the editor.
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
        mainMenuContainer.GetComponent<UITweener>().PlayForward();
        FindTheZones();
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
        }
        zonePanelManager.RemoveStaffHiringButtons();
        fader.SetActive(false);
        fader.layer = LayerMask.NameToLayer("Zone");
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

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddRandomCustomer();
            customerPanelManager.Hide();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
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

            LoadState();
            return;

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
}