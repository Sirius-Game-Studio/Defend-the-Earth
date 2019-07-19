using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowControllerButton : MonoBehaviour
{
    void Update()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            gameObject.SetActive(true);
            print(Input.GetJoystickNames()[0]);
        } else
        {
            gameObject.SetActive(false);
        }
    }
}
