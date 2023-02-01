using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helmetcontroller : MonoBehaviour
{
    public GameObject HelmetLoPoly;
    public float amp;
    public float rotate;
    void Start(){  }

    void Update()
    {
        HelmetLoPoly.transform.position = new Vector3(0, Mathf.Sin(Time.time) * amp, 0);
        HelmetLoPoly.transform.Rotate(new Vector3(0, 20, 0) * Time.deltaTime);

    }
         
}
