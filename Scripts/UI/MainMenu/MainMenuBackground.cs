using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Images")]
    [SerializeField]
    private Sprite pcBackground;
    [SerializeField]
    private Sprite mobileImage;
    private PlatfromEnum currentPlatform;
    private Image img;
    void Start()
    {
        img = GetComponent<Image>();
        currentPlatform = PlatformManager.CurrentPlatform;
        Debug.Log(currentPlatform);
        if(currentPlatform == PlatfromEnum.Desktop)
        {
            img.sprite = pcBackground;
        }
        else
        {
            img.sprite = mobileImage;
        }
    }
}