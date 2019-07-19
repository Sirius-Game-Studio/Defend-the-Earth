using UnityEngine;

public class QualityChanger : MonoBehaviour
{
    public void changeQuality(int qualityLevel)
    {
        if (qualityLevel > 0)
        {
            QualitySettings.SetQualityLevel(qualityLevel, true);
        } else
        {
            QualitySettings.SetQualityLevel(0, true);
        }
    }
}
