using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helmetcontroller : MonoBehaviour
{
    GameObject HelmetLoPoly;
    public float amp;
    public float rotate;
    void Start() {  
        HelmetLoPoly = gameObject;
    }

    void Update() {
        HelmetLoPoly.transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * amp, transform.position.z);
        HelmetLoPoly.transform.Rotate(new Vector3(0, 20, 0) * Time.deltaTime);
    }
         
}
