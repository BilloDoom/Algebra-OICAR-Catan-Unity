using Mirror;
using UnityEngine;

public class NetworkRTSCamera : NetworkBehaviour
{
    public float moveSpeed = 10f;
    public Transform cameraRig;
    public Camera localCamera;

    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector3 syncedPosition;

    private void Start()
    {
        if (isOwned)
        {
            localCamera.gameObject.SetActive(true);
        }
        else
        {
            localCamera.gameObject.SetActive(false);
        }

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(cameraRig);
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale = Vector3.one * 0.5f;
    }

    private void Update()
    {
        if (!isOwned || cameraRig == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        cameraRig.position += move;

        if (Vector3.Distance(cameraRig.position, syncedPosition) > 0.01f)
        {
            CmdUpdatePosition(cameraRig.position);
        }
    }

    [Command]
    void CmdUpdatePosition(Vector3 pos)
    {
        syncedPosition = pos;
    }

    void OnPositionChanged(Vector3 oldPos, Vector3 newPos)
    {
        if (!isOwned && cameraRig != null)
        {
            cameraRig.position = newPos;
        }
    }
}
