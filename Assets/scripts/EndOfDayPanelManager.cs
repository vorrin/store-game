﻿using UnityEngine;
using System;
using System.Collections;

public class EndOfDayPanelManager : MonoBehaviour {
    public GoTween endOfDayPanelTween;

    public UILabel titleLabel;
    public UILabel npsLabel;

    public UILabel totalSpendLabel;
    public UILabel resultSpendLabel;
    public UILabel totalStaffLabel;
    
    public UILabel averageStaffLabel;


	// Use this for initialization
	void Start () {
	    endOfDayPanelTween = Go.to(transform, 0.3f, new GoTweenConfig().localPosition(Vector3.zero).startPaused());
        endOfDayPanelTween.autoRemoveOnComplete = false;
	}

    public void PopulateEndOfDayScreen(ScoreTracker currentScores)
    {
        npsLabel.text = currentScores.totalCustomersProcessed == 0 ? "0%" : Mathf.RoundToInt((currentScores.totalNPSForTheDay * 100f) / currentScores.totalCustomersProcessed).ToString("0") + "%";

        //RECENT NPS SYSTEM CHANGES ARE IN HERE 
        //npsLabel.text = (currentScores.totalNPSForTheDay / currentScores.totalCustomersProcessed).ToString("0.0");
        totalSpendLabel.text = (currentScores.totalSpendForTheDay).ToString("0");

        God.instance.score.resultSpending = currentScores.totalSpendForTheDay + 
            currentScores.totalSpendForTheDay  * (currentScores.totalCustomersProcessed == 0 ? 0f : (currentScores.totalNPSForTheDay ) / currentScores.totalCustomersProcessed);
            
            //Mathf.Ceil(

            //((currentScores.totalNPSForTheDay / (float) currentScores.totalCustomersProcessed) / 10) * currentScores.totalSpendForTheDay
            //);
        if (float.IsNaN(God.instance.score.resultSpending))
        {
            God.instance.score.resultSpending = 0f;
        }
        resultSpendLabel.text = God.instance.score.resultSpending.ToString("0");
        int totalStaff = 0;
        float totalStaffExp = 0f;
        God.instance.zones.ForEach(zone =>
        {
            totalStaff += zone.staffNumber;
            totalStaffExp += zone.staffPower;
        });
        totalStaffLabel.text = totalStaff.ToString();
        averageStaffLabel.text = ((totalStaffExp) / totalStaff).ToString("0.0") + "%";
             
    }

    public void Display( Action<AbstractGoTween> callback = null)
    {
        titleLabel.text = "Day " + (God.instance.currentDifficultyLevel + 1 ).ToString();
        PopulateEndOfDayScreen(God.instance.score);
        endOfDayPanelTween.playForward();
        endOfDayPanelTween.setOnCompleteHandler(callback );
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(true);
        AudioManager.instance.PlayAudioForEndOfDay();
    }

    public void ButtonHide()
    {
        Hide();
    }

    //OVERLOAD
    public void Hide(Action<AbstractGoTween> callback )
    {
        endOfDayPanelTween.setOnCompleteHandler(callback);
        ActualHiding();
    }

    public void Hide(){
        endOfDayPanelTween.setOnCompleteHandler(null);
        ActualHiding();
    }

    public void ActualHiding()
    {
        endOfDayPanelTween.playBackwards();
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(false);

    }
}
