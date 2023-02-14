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
    public GameObject enterText;
    public AudioListener playerListener;
    public AudioListener carListener;
    CharacterController playerController;

    void Start() {
        player = gameObject;
        inCar = false;
        currentCar.GetComponent<BasicVehControls>().enabled = false;
        carCamera.enabled = false;
        carListener.enabled = false;
        playerController = GetComponent<CharacterController>();
    }

    void Update() {
        float distance = Vector3.Distance(transform.position, currentCar.transform.position);

        if(distance < 5 && !inCar) {
            enterText.SetActive(true);
        }
        else {
            enterText.SetActive(false);
        }

        if(!inCar) {
            if(Input.GetKeyDown(KeyCode.E) && distance < 5) {
                carControls = currentCar.GetComponent<BasicVehControls>();
                EnterCar(currentCar);
                return;
            }
        }
        else if(Input.GetKeyDown(KeyCode.E)) {
            ExitCar();
        }
        else {
            transform.position = playerExitLoc.position;
        }
    }

    void EnterCar(GameObject car) {
        inCar = true;

        camera.enabled = false;
        playerMesh.SetActive(false);
        playerScript.enabled = false;
        playerListener.enabled = false;
        playerController.enabled = false;

        carControls.enabled = true;
        carCamera.enabled = true;
        carListener.enabled = true;
    }

    void ExitCar() {
        inCar = false;

        carControls.enabled = false;
        carCamera.enabled = false;
        carListener.enabled = false;

        camera.enabled = true;
        playerMesh.SetActive(true);
        playerScript.enabled = true;
        playerListener.enabled = true;
        playerController.enabled = true;
    }
}
