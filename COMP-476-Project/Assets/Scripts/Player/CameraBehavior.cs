using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform Target;

    [Header("Offsets")]
    public float xOffset;
    public float yOffset;
    public float zOffset;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 Pos=transform.position;
        Pos.x = Target.position.x+xOffset;
        Pos.y = Target.position.y+yOffset;
        Pos.z = Target.position.z+zOffset;

        transform.position = Pos;

        Vector3 LookAtPosition=Target.position;
        LookAtPosition.y += 3;
        transform.LookAt(LookAtPosition);
    }
}
