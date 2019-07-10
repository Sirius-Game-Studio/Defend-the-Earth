using UnityEngine;
using UnityEngine.UI;

public class PriceCounter : MonoBehaviour
{
    [SerializeField] private string key = "";

    private Text counter;

    void Start()
    {
        counter = GetComponent<Text>();
    }

    void Update()
    {
        if (key != "") counter.text = "$" + PlayerPrefs.GetInt(key);
    }
}
