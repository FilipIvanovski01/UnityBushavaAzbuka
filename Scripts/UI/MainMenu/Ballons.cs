using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Floating balloon + click-to-pop. Pointer events must hit a <see cref="Graphic"/>; if the
/// <see cref="Image"/> lives on a child, a small forwarder is added so clicks still register.
/// </summary>
public class FloatingBalloonUI : MonoBehaviour, IPointerDownHandler
{
    [Header("Floating")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 20f;

    [Header("Pop")]
    [SerializeField] private AudioClip popSound;
    [SerializeField] private float growScale = 1.25f;
    [SerializeField] private float growSpeed = 18f;
    [SerializeField] private float shrinkSpeed = 25f;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private float randomOffset;

    private bool isPopping;
    private bool isShrinking;

    private Image image;
    private AudioSource audioSource;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
            image = GetComponentInChildren<Image>(true);

        if (image != null)
        {
            image.raycastTarget = true;

            if (image.gameObject != gameObject &&
                image.gameObject.GetComponent<BalloonPointerForward>() == null)
            {
                BalloonPointerForward forward = image.gameObject.AddComponent<BalloonPointerForward>();
                forward.Init(this);
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        originalScale = transform.localScale;
        if (originalScale == Vector3.zero)
            originalScale = Vector3.one;
    }

    private void Start()
    {
        startPosition = transform.localPosition;
        originalScale = transform.localScale;
        if (originalScale == Vector3.zero)
            originalScale = Vector3.one;

        randomOffset = Random.Range(0f, 10f);
    }

    private void Update()
    {
        if (isPopping)
        {
            AnimatePop();
            return;
        }

        FloatBalloon();
    }

    private void FloatBalloon()
    {
        float yOffset = Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;

        transform.localPosition = new Vector3(
            startPosition.x,
            startPosition.y + yOffset,
            startPosition.z
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BeginPop();
    }

    /// <summary>Called from <see cref="BalloonPointerForward"/> when the clickable <see cref="Image"/> is on a child.</summary>
    internal void BeginPop()
    {
        if (isPopping)
            return;

        isPopping = true;

        if (image != null)
            image.raycastTarget = false;

        if (popSound != null)
        {
            Vector3 pos = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(popSound, pos);
        }
    }

    private void AnimatePop()
    {
        if (!isShrinking)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale * growScale,
                growSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.localScale, originalScale * growScale) < 0.05f)
                isShrinking = true;
        }
        else
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.zero,
                shrinkSpeed * Time.deltaTime
            );

            if (transform.localScale.x < 0.05f)
                gameObject.SetActive(false);
        }
    }
}

/// <summary>Forwards pointer down to <see cref="FloatingBalloonUI"/> when the raycast hits a child <see cref="Image"/>.</summary>
[DisallowMultipleComponent]
internal sealed class BalloonPointerForward : MonoBehaviour, IPointerDownHandler
{
    private FloatingBalloonUI owner;

    public void Init(FloatingBalloonUI balloon)
    {
        owner = balloon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        owner?.BeginPop();
    }
}
