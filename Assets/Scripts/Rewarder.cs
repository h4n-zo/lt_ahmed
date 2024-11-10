using System;
using System.Collections;
using System.Collections.Generic;
using Gley.DailyRewards;
using UnityEngine;
using UnityEngine.UI;

public class Rewarder : MonoBehaviour
{
    public int reward = 50;
    // public Text rewardText;

    void Start()
    {
        // Gley.DailyRewards.API.Calendar.AddClickListener(CalendarButtonClicked);
        Gley.DailyRewards.API.TimerButton.AddClickListener(RewardButtonClicked);
        // Debug.Log("Timer started");

    }

    private void RewardButtonClicked(TimerButtonIDs buttonID, bool timeExpired)
    {
        if (timeExpired)
        {
            if (buttonID == TimerButtonIDs.RewardButton)
            {
                //give a reward for this button ID
                if (buttonID == TimerButtonIDs.RewardButton)
                {
                    GameObject.FindObjectOfType<CurrencyManager>().AddCurrency(reward);

                    Debug.Log("Get reward for button");
                    AdsManager.Instance.ShowRewardedVideo();
                }
            }
        }
        else
        {
            //not ready yet, you have to wait
            Debug.Log("Not ready for reward");
            Debug.Log("Wait " + Gley.DailyRewards.API.TimerButton.GetRemainingTime(buttonID));
        }
    }


    #region 
    // private void CalendarButtonClicked(int dayNumber, int rewardValue, Sprite rewardSprite)
    // {
    //     reward += rewardValue;
    //     rewardText.text = this.reward.ToString();
    // }

    // public void ShowCalendar()
    // {
    //     Gley.DailyRewards.API.Calendar.Show();
    // }
    #endregion
}
