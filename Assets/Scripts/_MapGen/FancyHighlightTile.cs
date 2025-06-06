using UnityEngine;

public class FancyHighlightTile : MonoBehaviour
{
    private Vector3 basePosition;
    private float currentLift = 0f;

    void Start()
    {
        basePosition = transform.position;

        FindAnyObjectByType<TileHighlightManager>().RegisterTile(this);
    }

    public Vector3 WorldPosition => basePosition;

    public void SetLiftTarget(float targetLift, float smoothSpeed)
    {
        currentLift = Mathf.Lerp(currentLift, targetLift, Time.deltaTime * smoothSpeed);
        transform.position = basePosition + new Vector3(0f, currentLift, 0f);
    }
}
