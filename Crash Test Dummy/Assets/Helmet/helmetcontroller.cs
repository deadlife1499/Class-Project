using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helmetcontroller : MonoBehaviour
{
    public GameObject HelmetLoPoly;
    public float amp;
    public float rotate;
    float positionX;
    float positionY;
    float positionZ;
    void Start(){  
        positionX = transform.position.x;
        positionY = transform.position.y;
        positionZ = transform.position.z;



    }


    void Update()
    {
        HelmetLoPoly.transform.position = new Vector3(0+positionX, positionY + Mathf.Sin(Time.time) * amp, 0 + positionZ);
        HelmetLoPoly.transform.Rotate(new Vector3(0, 20, 0) * Time.deltaTime);

    }
         
}
