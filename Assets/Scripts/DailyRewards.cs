using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DailyRewards : MonoBehaviour
{
    [Serializable]
    public class DaiylyReward
    {
        public string rewardType;
        public int rewardTypeID;
        public int moneyCount;
    }

    public List<DaiylyReward> _DaiylyRewards = new List<DaiylyReward>();

    public CanvasGroup dailyRewardGroup, claimPanel;
    public List<GameObject> daysCleared = new List<GameObject>();
    public List<GameObject> itemsCelared = new List<GameObject>();
    public int currendDay;
    private int currentReward;
    private bool isClaim;
    void Start()
    {
        DateTime theDate = DateTime.Now;
        if (PlayerPrefs.GetInt("CurrentDay", -1) == -1)
        {
            PlayerPrefs.SetInt("CurrentDay", theDate.Day);
            currendDay = -1;
        }
        else
        {
            currendDay = PlayerPrefs.GetInt("CurrentDay", 1);
        }
        isClaim = false;
        currentReward = PlayerPrefs.GetInt("CurrentDayReward", 0);
        if (currendDay != theDate.Day)
        {
            if (currentReward <= 6)
            {
                SetActiveReward();
            }
        }
    }

    void SetActiveReward()
    {
        dailyRewardGroup.DOFade(1f, 0.5f);
        dailyRewardGroup.blocksRaycasts = dailyRewardGroup.interactable = true;
        claimPanel.DOFade(1f, 0.5f);
        claimPanel.blocksRaycasts = claimPanel.interactable = true;
        for (int i = 0; i < currentReward; i++)
        {
            daysCleared[i].SetActive(true);
        }
        daysCleared[currentReward].SetActive(true);
        itemsCelared[currentReward].SetActive(true);
    }
    
    public void CloseDailyReward()
    {
        dailyRewardGroup.DOFade(0f, 0.5f);
        dailyRewardGroup.blocksRaycasts = false;
    }
    
    public void Claim()
    {
        if (!isClaim)
        {
            claimPanel.DOFade(0f, 0.5f);
            claimPanel.blocksRaycasts = claimPanel.interactable = false;
            DateTime theDate = DateTime.Now;
            PlayerPrefs.SetInt("CurrentDay", theDate.Day);
            PlayerPrefs.SetInt("CurrentDayReward", PlayerPrefs.GetInt("CurrentDayReward", 0) + 1);
            isClaim = true;

            if (currendDay < _DaiylyRewards.Count)
            {
                if (_DaiylyRewards[currentReward].rewardType == "money")
                {
                    PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) +_DaiylyRewards[currentReward].moneyCount );
                }
                else
                {
                    PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) +_DaiylyRewards[currentReward].moneyCount );
                    PlayerPrefs.SetInt(_DaiylyRewards[currentReward].rewardType + "-" + _DaiylyRewards[currentReward].rewardTypeID, 1);
                }
            }
            UIMainMenu.instance.ResetText();
        }
    }
}
