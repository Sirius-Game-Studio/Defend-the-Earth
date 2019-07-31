using System.Collections;
using UnityEngine;
using UnityEngine.Monetization;

public class WatchSaveMeAd : MonoBehaviour
{
    private string gameID = "3229625";

    void Start()
    {
#if UNITY_IOS
        gameID = "3229624";
#elif UNITY_ANDROID
        gameID = "3229625";
#endif
        Monetization.Initialize(gameID, false);
    }

    public void showAd()
    {
        StartCoroutine(waitForAd());
    }

    IEnumerator waitForAd()
    {
        while (!Monetization.IsReady("rewardedVideo")) yield return null;
        if (Monetization.GetPlacementContent("rewardedVideo") is ShowAdPlacementContent ad) ad.Show(isAdFinished);
    }

    void isAdFinished(ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            GameController.instance.startSaveMe();
            transform.parent.gameObject.SetActive(false);
            enabled = false;
        } else if (showResult == ShowResult.Failed)
        {
            Debug.LogError("Could not finish ad due to a error!");
        }
    }
}