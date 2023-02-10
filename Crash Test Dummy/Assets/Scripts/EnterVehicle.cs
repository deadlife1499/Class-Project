using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterVehicle : MonoBehaviour
{
    GameObject player;
    bool inCar;
    public GameObject currentCar;
    public Camera camera;
    public GameObject playerMesh;
    public BeanMovement playerScript;
    BasicVehControls carControls;
    public Camera carCamera;
    public Transform playerExitLoc;
    public GameObject EnterText;

    void Start() {
        player = gameObject;
        inCar = false;
        currentCar.GetComponent<BasicVehControls>().enabled = false;
        carCamera.enabled = false;
    }

    void Update() {
        float distance = Vector3.Distance(transform.position, currentCar.transform.position);

        if(distance < 5 && !inCar) {
            EnterText.SetActive(true);
        }
        else {
            EnterText.SetActive(false);
        }

        if(!inCar) {
            if(Input.GetKeyDown(KeyCode.E) && distance < 5) {
                inCar = true;
                carControls = currentCar.GetComponent<BasicVehControls>();
                currentCar = currentCar;
                EnterCar(currentCar);
                return;
            }
        }
        else if(Input.GetKeyDown(KeyCode.E)) {
            inCar = false;
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
        transform.position = new Vector3(0, 0, 0);
        transform.position = playerExitLoc.position;
        
        carControls.enabled = false;
        carCamera.enabled = false;

        camera.enabled = true;
        playerMesh.SetActive(true);
        playerScript.enabled = true;
    }
}
