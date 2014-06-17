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
