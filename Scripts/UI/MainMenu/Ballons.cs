using UnityEngine;

public class Ballons : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 20f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.localPosition = new Vector3(
            startPosition.x,
            startPosition.y + yOffset,
            startPosition.z
        );
    }
}