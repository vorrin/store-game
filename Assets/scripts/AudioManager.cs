using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
    
    
    public AudioSource speaker;
    public AudioClip failedSale;
    public AudioClip successfulSale;
    public AudioClip successfulUpsell;
    public AudioClip failedUpsell;
    public AudioClip endOfDay;
    public AudioClip startOfDay;
    public AudioClip staffTrained;
    public AudioClip staffHired;
    public AudioClip backgroundMusic;
    public AudioClip bestChoice;
    public AudioClip secondBestChoice;
    public AudioClip button;
    public AudioClip queueFull;



    




    private static AudioManager s_Instance = null;
    public static AudioManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(AudioManager)) as AudioManager;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(AudioManager)) as AudioManager;
                Debug.Log("Could not locate an AManager object.  AManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }




    public void PlayAudioForIcon(ZoneFeedbackIcon.Icons icon)
    {
        if (icon == ZoneFeedbackIcon.Icons.SaleFine)
        {
            speaker.PlayOneShot(successfulSale);
        }
        else if (icon == ZoneFeedbackIcon.Icons.UpsellFail)
        {
            speaker.PlayOneShot(failedUpsell);
        }
        else if (icon == ZoneFeedbackIcon.Icons.UpsellFine)
        {
            speaker.PlayOneShot(successfulUpsell);
        }
        else if (icon == ZoneFeedbackIcon.Icons.DeathInQueue)
        {
            speaker.PlayOneShot(failedSale);
        }
        
    }
    public void PlayAudioForIcon(FeedbackIcon.Icons icon) {
        if (icon == FeedbackIcon.Icons.Fail)
        {
            speaker.PlayOneShot(failedSale);
        }
        else if (icon == FeedbackIcon.Icons.BestOption)
        {
            speaker.PlayOneShot(bestChoice);
        }
        else if (icon == FeedbackIcon.Icons.SecondBestOption)
        {
            speaker.PlayOneShot(secondBestChoice);
        }
        else if (icon == FeedbackIcon.Icons.Full)
        {
            print("PLAYING QUQUQUQUQ ");
            speaker.PlayOneShot(queueFull);
        }
}

    public void PlayAudioForEndOfDay()
    {
        speaker.PlayOneShot(endOfDay);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationQuit()
    {
        s_Instance = null;
    }

    

}
