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

    bool paused;

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

        // for screenshots
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                Time.timeScale = 1;
                paused = false;
            }
            else
            {
                Time.timeScale = 0;
                paused = true;
            }
        }
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
            if(zOffset<-0.1)
                zOffset += 0.1f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if(zOffset>-4)
                zOffset -= 0.1f;
        }
    }

    void ZoomControl()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            if(height>0.5)
                height -= 0.1f;
            if(xOffset>0.5)
                xOffset -= 0.1f;
            if(zOffset<-0.5)
                zOffset += 0.1f;
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            if(height<3)
                height += 0.1f;
            if(xOffset<3)
                xOffset += 0.1f;
            if(zOffset>-3)
                zOffset -= 0.1f;
        }

        if (Input.GetKey(KeyCode.R))
        {
            //Reset Camera
            height = yOldOffset;
            xOffset = xOldOffset;
            zOffset = zOldOffset;
        }
    }
    
}
