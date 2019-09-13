using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Monetization;

public class WatchRewardAd : MonoBehaviour
{
    [SerializeField] private Text moneyReward = null;
    [SerializeField] private AudioClip buttonClick = null;

    private AudioSource audioSource;
    private string gameID = "3229625";
    private long givenMoney = 15;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        if (moneyReward) moneyReward.text = "Claim $" + givenMoney;
    }

    public void showAd()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
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
            if (PlayerPrefs.GetString("Money") != "")
            {
                long money = long.Parse(PlayerPrefs.GetString("Money"));
                money += givenMoney;
                PlayerPrefs.SetString("Money", money.ToString());
            } else
            {
                PlayerPrefs.SetString("Money", givenMoney.ToString());
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

    float getVolumeData(bool isSound)
    {
        float volume = 1;
        if (isSound)
        {
            if (PlayerPrefs.HasKey("SoundVolume")) volume = PlayerPrefs.GetFloat("SoundVolume");
        } else
        {
            if (PlayerPrefs.HasKey("MusicVolume")) volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        return volume;
    }
}