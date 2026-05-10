using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatfromEnum CurrentPlatform
    {
        get 
        {
            if(!hasDetectedPlatform)
            {
                DetectPlatform();
            }
            return currentPlatform;
        }
        private set
        {
            currentPlatform = value;
        }
    }

    private static PlatfromEnum currentPlatform;
    private static bool hasDetectedPlatform;

    private void Awake()
    {
        DetectPlatform();
    }

    private static void DetectPlatform()
    {
        CurrentPlatform = Application.isMobilePlatform ? PlatfromEnum.Mobile : PlatfromEnum.Desktop;
        hasDetectedPlatform = true; 
    }


}
