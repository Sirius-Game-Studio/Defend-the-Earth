using UnityEngine;
using UnityEngine.UI;

public class MoneyCount : MonoBehaviour
{
    private Text main;

    void Start()
    {
        main = GetComponent<Text>();
    }

    void Update()
    {
        if (PlayerPrefs.GetString("Money") != "")
        {
            main.text = "$" + PlayerPrefs.GetString("Money");
        } else
        {
            main.text = "$0";
        }
    }
}
