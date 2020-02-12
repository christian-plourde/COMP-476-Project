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

    private float xOldOffset;
    private float yOldOffset;
    private float zOldOffset;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        xOldOffset = xOffset;
        yOldOffset = height;
        zOldOffset = zOffset;
    }

    void Update()
    {
        CamControl();
        ZoomControl();
    }

    void CamControl()
    {
        Vector3 pos = new Vector3();
        pos.x = Target.position.x+xOffset;
        pos.y = Target.position.y+height;
        pos.z = Target.position.z+zOffset;

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothValue);


        // move on z axis
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if(zOffset<-2)
                zOffset += 0.2f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if(zOffset>-10)
                zOffset -= 0.2f;
        }
    }

    void ZoomControl()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            if(height>2)
                height -= 0.2f;
            if(xOffset>2)
                xOffset -= 0.2f;
            if(zOffset<-2)
                zOffset += 0.2f;
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            if(height<15)
                height += 0.2f;
            if(xOffset<15)
                xOffset += 0.2f;
            if(zOffset>-15)
                zOffset -= 0.2f;
        }

        if (Input.GetKey(KeyCode.R))
        {
            //Reset Camera
            Debug.Log("Resetting Camera Offsets to Default");
            height = yOldOffset;
            xOffset = xOldOffset;
            zOffset = zOldOffset;
        }
    }
    
}
