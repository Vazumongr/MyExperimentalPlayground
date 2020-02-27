using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMovementScript : MonoBehaviour
{

    public float speedH = 0.2f;
    public float speedV = 0.2f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    float speed = .3f;

    bool cameraMove = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.down * speed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.up * speed);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            cameraMove = !cameraMove;
        if(cameraMove)
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        }


    }
}
