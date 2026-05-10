using UnityEngine;

public class GridLayout : MonoBehaviour
{
    private RectTransform targetUI;

    [Header("Positions")]
    [SerializeField] private Vector2 pcPosition;
    [SerializeField] private Vector2 mobilePosition;

    void Start()
    {
        targetUI = GetComponent<RectTransform>();
        if (targetUI)
        {
            if (PlatformManager.CurrentPlatform == PlatfromEnum.Mobile)
            {
                targetUI.anchoredPosition = mobilePosition;
            }
            else
            {
                targetUI.anchoredPosition = pcPosition;
            }
        }
    }
}
