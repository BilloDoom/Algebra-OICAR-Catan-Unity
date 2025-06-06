using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [Tooltip("Set the desired frame rate limit. Use -1 for no limit."), Min(-1)]
    public int targetFPS = 60;

    void Start()
    {
        Application.targetFrameRate = targetFPS;
    }
}
