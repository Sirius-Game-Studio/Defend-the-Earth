using UnityEngine;

public class LoadingTipArray : MonoBehaviour
{
    public static LoadingTipArray instance;

    public string[] tips = new string[0];

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
