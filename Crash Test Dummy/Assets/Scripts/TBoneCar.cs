using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBoneCar : MonoBehaviour
{
    public GameObject wheelMeshFR, wheelMeshFL, wheelMeshBR, wheelMeshBL;
    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        MoveCar();
    }

    void FixedUpdate() {
        UpdateMeshRotations();
    }

    void MoveCar() {
        rb.AddForce(7.5f * transform.forward, ForceMode.Impulse);
    }

    void UpdateMeshRotations() {
        wheelMeshFR.transform.Rotate(4f, 0f, 0f);
        wheelMeshFL.transform.Rotate(4f, 0f, 0f);
        wheelMeshBR.transform.Rotate(4f, 0f, 0f);
        wheelMeshBL.transform.Rotate(4f, 0f, 0f);
    }
}
