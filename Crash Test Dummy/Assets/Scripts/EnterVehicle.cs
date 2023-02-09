using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterVehicle : MonoBehaviour
{
    GameObject[] vehicleObjs;
    GameObject player;
    bool inCar;
    GameObject currentCar;
    public Camera camera;
    public GameObject playerMesh;
    public BeanMovement playerScript;
    BasicVehControls carControls;
    Camera carCamera;
    public Transform playerExitLoc;

    void Start() {
        player = gameObject;
        inCar = false;
        vehicleObjs = GameObject.FindGameObjectsWithTag("DrivableCar");
        foreach(GameObject obj in vehicleObjs) {
            obj.GetComponent<BasicVehControls>().enabled = false;
            obj.GetComponent<Camera>().enabled = false;
        }
    }

    void Update() {
        if(!inCar) {
            foreach(GameObject obj in vehicleObjs) {
                //if(CollisionDetection.IsTouching(obj, player) && Input.GetKey(KeyCode.E)) {
                if(Input.GetKey(KeyCode.E)) {
                    inCar = true;
                    carControls = obj.GetComponent<BasicVehControls>();
                    currentCar = obj;
                    carCamera = obj.GetComponent<Camera>();
                    EnterCar(obj);
                    return;
                }
            }
        }
        else if(Input.GetKey(KeyCode.E)) {
            ExitCar();
        }
    }

    void EnterCar(GameObject car) {
        camera.enabled = false;
        playerMesh.SetActive(false);
        playerScript.enabled = false;

        carControls.enabled = true;
        carCamera.enabled = true;
    }

    void ExitCar() {
        carControls.enabled = false;
        carCamera.enabled = false;

        transform.position = playerExitLoc.position;

        camera.enabled = true;
        playerMesh.SetActive(true);
        playerScript.enabled = true;
    }
}
