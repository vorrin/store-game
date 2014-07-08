using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour
{
    public GameObject[] slides;
    int visibleSlideIndex = 0;
    public UIButton leftButton;
    public UIButton rightButton;
    public GameObject panelInfo;

    // Use this for initialization
    void Start()
    {
        leftButton.isEnabled = false;
        //Checks if game is running for the first time.
        if (PlayerPrefs.GetInt("SimgameFirstRun") != 1)
        {
            panelInfo.GetComponent<TweenPosition>().Play(true);
            PlayerPrefs.SetInt("SimgameFirstRun", 1);
        }
    }

    public void ButtonLeft()
    {
        Slide(-1);
    }
    public void ButtonRight()
    {
        Slide(1);
    }

    public void CloseInstructions()
    {

    }

    public void Slide(int i)
    {
        GameObject slideToMoveOut = slides[visibleSlideIndex];

        Go.to(slideToMoveOut.transform, .3f, new GoTweenConfig().localPosition(new Vector3(i * -700, 0, 0)).onComplete(goTween =>
        {
            slideToMoveOut.SetActive(false);
        }));

        GameObject slideToMoveIn = slides[visibleSlideIndex + i];
        slideToMoveIn.transform.localPosition = new Vector3(i * 700, 0, 0);
        slideToMoveIn.SetActive(true);
        Go.to(slideToMoveIn.transform, .3f, new GoTweenConfig().localPosition(new Vector3(0, 0, 0)).onComplete(goTween =>
        {
        }));


        visibleSlideIndex += i;
        if (visibleSlideIndex == 0)
        {
            leftButton.isEnabled = false;
        }
        else if (visibleSlideIndex == slides.Length - 1)
        {
            rightButton.isEnabled = false;
        }
        else
        {
            leftButton.isEnabled = true;
            rightButton.isEnabled = true;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
