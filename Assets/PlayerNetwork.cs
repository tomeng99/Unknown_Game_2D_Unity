using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<Vector3> _netPos = new(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> _test = new(writePerm: NetworkVariableWritePermission.Owner);


    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsOwner)
        {
            _netPos.Value = transform.position;
            _test.Value = transform.localScale;
        }
        else
        {
            transform.position = _netPos.Value;
            transform.localScale = _test.Value;
        }
    }
}
