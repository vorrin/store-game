using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerIcon : MonoBehaviour {
    public Customer referredCustomer;
    public UISprite moodCloud;
    public bool markedForDestruction = false;
    public List<Transform> spots;
    private GoTween walkingAroundTween;


    public void OnDeserialized()
    {
        if (markedForDestruction)
        {
            Destroy(gameObject);
        }
    }
	// Use this for initialization
	void Start () {
	
	}

    public void Init(Customer customer)
    {
        referredCustomer = customer;
        GetComponent<UISprite>().spriteName = customer.avatarName + "_QUEUE";
        moodCloud.GetComponent<UISprite>().spriteName = "mood_" + customer.GetMoodColor() + "_QUEUE";
        foreach ( Transform spotTransform in transform.parent ){
            if (spotTransform.name == "spot")
            {
                spots.Add(spotTransform);
            }
        }
        transform.localPosition = spots[ Random.Range(0,spots.Count)].localPosition;
        WalkToSpot();
    }
    public void WalkToSpot()
    {
        walkingAroundTween = Go.to(transform, Random.Range(.5f,1.5f), new GoTweenConfig().localPosition(spots[Random.Range(0, spots.Count)].localPosition).onComplete((x) => { WalkToSpot(); }).setDelay(Random.Range(.2f,1f)));
    }


    public void Die()
    {
        walkingAroundTween.pause();
        walkingAroundTween.destroy();
        markedForDestruction = true;
        Go.to(gameObject.transform, .3f, new GoTweenConfig().scale(0f).onComplete(goTween =>
        {
            Destroy(gameObject);
        }));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
