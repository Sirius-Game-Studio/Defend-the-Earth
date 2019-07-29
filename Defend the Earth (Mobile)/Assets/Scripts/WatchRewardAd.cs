using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Monetization;

public class WatchRewardAd : MonoBehaviour
{
    [SerializeField] private Text moneyReward = null;

    private string gameID = "3229625";
    private long givenMoney = 15;

    void Start()
    {
#if UNITY_IOS
        gameID = "3229624";
#elif UNITY_ANDROID
        gameID = "3229625";
#endif
        Monetization.Initialize(gameID, false);
        givenMoney = Random.Range(15, 40);
        if (PlayerPrefs.HasKey("WatchedAd"))
        {
            transform.parent.gameObject.SetActive(false);
            enabled = false;
        }
    }

    void Update()
    {
        if (moneyReward) moneyReward.text = "$" + givenMoney;
    }

    public void showAd()
    {
        StartCoroutine(waitForAd());
    }

    IEnumerator waitForAd()
    {
        while (!Monetization.IsReady("rewardedVideo")) yield return null;
        ShowAdPlacementContent ad = Monetization.GetPlacementContent("rewardedVideo") as ShowAdPlacementContent;
        if (ad != null) ad.Show(isAdFinished);
    }

    void isAdFinished(ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            if (PlayerPrefs.GetString("Money") != "")
            {
                long money = long.Parse(PlayerPrefs.GetString("Money"));
                money += givenMoney;
                PlayerPrefs.SetString("Money", money.ToString());
            } else
            {
                PlayerPrefs.SetString("Money", Random.Range(20, 40).ToString());
            }
            PlayerPrefs.SetInt("WatchedAd", 0);
            PlayerPrefs.Save();
            transform.parent.gameObject.SetActive(false);
            enabled = false;
        } else if (showResult == ShowResult.Failed)
        {
            Debug.LogError("Could not finish ad due to a error!");
        }
    }
}