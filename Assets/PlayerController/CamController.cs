using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class CamController : NetworkBehaviour
{

    [Range(0, 1)]
    public float smoothTime;

    public Transform playerTransform;

    public override void OnNetworkSpawn()
    {
        // Temporary
        if (!IsOwner)
        {
            Camera ca = GetComponent<Camera>();
            ca.enabled = false;
        }
    }

    public void FixedUpdate()
    {

        Vector3 pos = GetComponent<Transform>().position;

        pos.x = Mathf.Lerp(pos.x, playerTransform.position.x, smoothTime);
        pos.y = Mathf.Lerp(pos.y, playerTransform.position.y, smoothTime);


        GetComponent<Transform>().position = pos;


    }

}
