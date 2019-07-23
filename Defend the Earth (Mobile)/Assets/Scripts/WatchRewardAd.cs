/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class WatchRewardAd : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] private string adPlacement = "";

    private Button button;
    #if UNITY_IOS
    private string gameID = "3229624";
    #elif UNITY_ANDROID
    private string gameID = "3229625";
    #endif

    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = Advertisement.IsReady(adPlacement);
        button.onClick.AddListener(showAd);
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, true);
    }

    void showAd()
    {
        Advertisement.Show(adPlacement);
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("Could not finish ad due to a error!");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            money += Random.Range(20, 50);
            PlayerPrefs.SetString("Money", money.ToString());
        } else if (showResult == ShowResult.Failed)
        {
            Debug.LogError("Could not finish ad due to a error!");
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == adPlacement) button.interactable = true;
    }
}
*/