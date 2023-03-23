using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float travelHeight = 20f;
    public float timeLimit = 4f;
    public GameObject car;
    public Material greenZone, yellowZone, redZone;
    public GameObject zone;
    public Transform elevator;
    public Transform button;
    public Transform playerLoc;

    bool isElevating = false;
    bool isGreen = false;
    float time = 0f;

    void Update() {
        float distance = GetDistance();
        UpdateMaterials(distance);

        distance = GetButtonDistance();
        UpdateButton(distance);
    }

    void UpdateMaterials(float distance) {
        if(!isElevating) {
            if(distance < 4 && IsColliding()) {
                ChangeMaterial(1);
                isGreen = false;
            }
            else if(distance < 4) {
                ChangeMaterial(0);
                isGreen = true;
            }
            else {
                ChangeMaterial(2);
                isGreen = false;
            }
        }
    }

    void UpdateButton(float distance) {
        if(distance < 2 && isGreen) {
            PressButton();
            InvokeRepeating("DepressButton", 1.5f, 5f);
        }
    }

    void PressButton() {
        isGreen = false;
        isElevating = true;
        InvokeRepeating("ElevatePlatform", 0f, .01f);
        button.position = new Vector3(button.position.x, button.position.y - .03f, button.position.z);
    }

    void DepressButton() {
        button.position = new Vector3(button.position.x, button.position.y + .03f, button.position.z);
        CancelInvoke("DepressButton");
    }

    void ChangeMaterial(int matNum) {
        Material newMat = null;

        switch(matNum) {
            case 0: {
                newMat = greenZone;
                break;
            }
            case 1: {
                newMat = yellowZone;
                break;
            }
            case 2: {
                newMat = redZone;
                break;
            }
        }

        for(int i = 0; i<4; i++) {
            zone.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = newMat;
        }
    }

    bool IsColliding() {
        BoxCollider carCollider = car.GetComponent<BoxCollider>();
        BoxCollider[] zoneColliders = new BoxCollider[4];

        for(int i = 0; i<4; i++) {
            zoneColliders[i] = zone.transform.GetChild(i).gameObject.GetComponent<BoxCollider>();
        }
        foreach(BoxCollider zoneCollider in zoneColliders) {
            if(carCollider.bounds.Intersects(zoneCollider.bounds)) {
                return true;
            }
        }
        return false;
    }

    float GetDistance() {
        return Vector3.Distance(car.transform.position, zone.transform.position);
    }

    float GetButtonDistance() {
        return Vector3.Distance(playerLoc.position, button.position);
    }

    void ElevatePlatform() {
        if(time < timeLimit) {
            time += .01f;

            car.transform.position = new Vector3(car.transform.position.x, car.transform.position.y + .01f * (travelHeight / timeLimit), car.transform.position.z);
            elevator.position = new Vector3(elevator.position.x, elevator.position.y + .01f * (travelHeight / timeLimit), elevator.position.z);
        }
        else {
            isElevating = false;
            CancelInvoke("ElevatePlatform");
            time = 0f;
        }
    }
}
