using System;
using System.Collections;
using System.Collections.Generic;
using Gley.DailyRewards;
using UnityEngine;
using UnityEngine.UI;

public class Rewarder : MonoBehaviour
{
    int reward = 0;
    public Text rewardText;

    void Start()
    {
        Gley.DailyRewards.API.Calendar.AddClickListener(CalendarButtonClicked);
        // Gley.DailyRewards.API.TimerButton.AddClickListener(RewardButtonClicked);
        Debug.Log("Timer started");
        
    }

    // private void RewardButtonClicked(TimerButtonIDs buttonID, bool timeExpired)
    // {
    //     if (timeExpired)
    //     {
    //         if (buttonID == TimerButtonIDs.RewardButton)
    //         {
    //             //give a reward for this button ID
    //             if (buttonID == TimerButtonIDs.RewardButton)
    //             {
    //                 Debug.Log("Get reward for button");
    //             }
    //             //  if (buttonID == TimerButtonIDs.RewardButton1)
    //             // {
    //             //     Debug.Log("Get reward for button");
    //             // }
    //         }
    //     }
    //     else
    //     {
    //         //not ready yet, you have to wait
    //         Debug.Log("Not ready for reward");
    //           Debug.Log("Wait " + Gley.DailyRewards.API.TimerButton.GetRemainingTime(buttonID));
    //     }
    // }


    #region 
    private void CalendarButtonClicked(int dayNumber, int rewardValue, Sprite rewardSprite)
    {
        reward += rewardValue;
        rewardText.text = this.reward.ToString();
    }

    public void ShowCalendar()
    {
        Gley.DailyRewards.API.Calendar.Show();
    }
    #endregion
}
