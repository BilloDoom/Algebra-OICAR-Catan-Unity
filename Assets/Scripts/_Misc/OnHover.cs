using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnHover : MonoBehaviour
{
    [Header("Hover Animation Settings")]
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float animationSpeed = 5f;
    public bool useColorTint = false;

    private Vector3 originalScale;
    private bool isHovered = false;
    private Renderer targetRenderer;

    void Start()
    {
        originalScale = transform.localScale;
        targetRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        DetectHover();

        // Animate scale
        Vector3 targetScale = isHovered ? hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    void DetectHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        isHovered = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isHovered = true;
            }
        }
    }
}
