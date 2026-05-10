using UnityEngine;
using UnityEngine.UI;

public class Canva : MonoBehaviour
{
    private CanvasScaler scal;
    void Start()
    {
        scal = GetComponent<CanvasScaler>();
        if (scal)
        {
            if(PlatformManager.CurrentPlatform == PlatfromEnum.Mobile)
            {
                scal.referenceResolution = new Vector2(1080,1920);
            }
            else
            {
                scal.referenceResolution = new Vector2(1920,1080);
            }
        }
    }

   
}
