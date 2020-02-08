using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // 3rd Person
    // has reference to target, rotate target with rotation on X axis
    public Transform Target;
    public float smoothValue = 0.3f;

    [Header("Camera Offsets")]
    public float height;
    public float xOffset;
    public float zOffset;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        
    }

    void Update()
    {
        CamControl();
    }

    void CamControl()
    {
        Vector3 pos = new Vector3();
        pos.x = Target.position.x+xOffset;
        pos.y = Target.position.y+height;
        pos.z = Target.position.z+zOffset;

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothValue);
    }
    
}
