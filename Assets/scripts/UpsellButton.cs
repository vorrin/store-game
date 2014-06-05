using UnityEngine;
using System.Collections;

public class UpsellButton : MonoBehaviour {

    public bool attemptingUpsell = false;
    public UIButton button;
    public string normalSprite;
    public string pressedSprite;


    public void setUpselling(bool tryingToUpsell)
    {
        attemptingUpsell = tryingToUpsell;
        if (tryingToUpsell == true)
        {
            button.normalSprite = pressedSprite;
        }
        else
        {
            button.normalSprite = normalSprite;
        }
    }

    public void OnClick()
    {
        setUpselling(!attemptingUpsell);
        transform.parent.GetComponent<CustomerPanelManager>().currentCustomer.attemptingUpsell = attemptingUpsell;
        
    }



	// Use this for initialization
    void Start()
    {
        button = GetComponent<UIButton>();
        normalSprite = button.normalSprite;
        pressedSprite = button.pressedSprite;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
