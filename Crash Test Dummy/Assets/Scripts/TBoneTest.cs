using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TBoneTest : MonoBehaviour
{
    public Transform[] spawnLocs = new Transform[10];
    public GameObject car;
    public BasicVehControls carScript;
    public GameObject greenLights, yellowLights, redLights;
    public Transform startLocation;
    public float timeLimit = 20f;
    public GameObject frontBarrier, rearBarrier;
    float time;
    public TMP_Text timeRemaining;
    public GameObject text1, text2, text3, textGo;
    bool testInProgress;
    public GameObject[] prefabs = new GameObject[5];
    int carNum;
    ArrayList vehicles;

    void Start() {
        time = 0f;
        carNum = 0;
        timeRemaining.text = "";
        testInProgress = false;
        vehicles = new ArrayList();

        rearBarrier.SetActive(false);
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        textGo.SetActive(false);

        greenLights.SetActive(false);
        yellowLights.SetActive(false);
        redLights.SetActive(false);
    }

    void Update() {
        if(car.transform.position.x < 134f && !testInProgress) {
            StartTest();
        }
        if(testInProgress) {
            DetectCollisions();
        }
    }
    
    void DetectCollisions() {
        foreach(GameObject vehicle in vehicles) {
            if(vehicle != null) {
                BoxCollider playerCollider = car.GetComponent<BoxCollider>();
                BoxCollider vehicleCollider = vehicle.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();
                if(playerCollider.bounds.Intersects(vehicleCollider.bounds)) {
                    Reset();
                }
            }
        }
    }

    void StartTest() {
        rearBarrier.SetActive(true);
        frontBarrier.SetActive(false);
        carScript.enabled = false;

        InvokeRepeating("StartTimer", 0f, .01f);
        InvokeRepeating("SpawnCars", 0f, 5f);
        testInProgress = true;

        Vector3 pos = new Vector3(startLocation.position.x, car.transform.position.y, startLocation.position.z);
        car.transform.position = pos;
        car.transform.rotation = startLocation.rotation;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void SpawnCars() {
        int num = (int)UnityEngine.Random.Range(0, 5);
        GameObject obj = Instantiate(prefabs[num], spawnLocs[carNum].position, spawnLocs[carNum].rotation);
        vehicles.Add(obj);
        Destroy(obj, 4.75f);

        if(carNum > 2) {
            carNum -= 4;
        }
        else {
            carNum += 6;
        }

        CancelInvoke("SpawnCars");
        InvokeRepeating("SpawnCars", UnityEngine.Random.Range(0.35f, 0.5f), 5f);
    }

    void StartTimer() {
        time += .01f;
        Vector3 pos = new Vector3(startLocation.position.x, car.transform.position.y, startLocation.position.z);

        if(time <= 1) {
            car.transform.position = pos;
            car.transform.rotation = startLocation.rotation;
            car.GetComponent<Rigidbody>().velocity = Vector3.zero;  
        }
        else if(time > 1 && time < 2) {
            text1.SetActive(false);
            text2.SetActive(false);
            text3.SetActive(true);
            textGo.SetActive(false);
            redLights.SetActive(true);

            car.transform.position = pos;
            car.transform.rotation = startLocation.rotation;
            car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if(time > 1 && time < 3) {
            text3.SetActive(false);
            text2.SetActive(true);

            car.transform.position = pos;
            car.transform.rotation = startLocation.rotation;
            car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if(time > 1 && time < 4) {
            text2.SetActive(false);
            text1.SetActive(true);
            yellowLights.SetActive(true);

            car.transform.position = pos;
            car.transform.rotation = startLocation.rotation;
            car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if(time > 1 && time < 5) {
            text1.SetActive(false);
            textGo.SetActive(true);

            car.transform.position = pos;
            car.transform.rotation = startLocation.rotation;
            car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else {
            carScript.enabled = true;
            textGo.SetActive(false);
            timeRemaining.text = (timeLimit - (time - 5f)).ToString("F2");
            greenLights.SetActive(true);

            if(car.transform.position.x <= -24.5f) {
                time = 5 + timeLimit;
            }
        }
        if(time >= 5 + timeLimit) {
            timeRemaining.text = "";
            frontBarrier.SetActive(true);
            rearBarrier.SetActive(false);

            greenLights.SetActive(false);
            yellowLights.SetActive(false);
            redLights.SetActive(false);

            time = 0f;
            CancelInvoke("StartTimer");
            CancelInvoke("SpawnCars");
            if(car.transform.position.x <= -24.5f) {
                this.enabled = false;
            }
            else{
                Reset();
            }
        }
    }

    void Reset() {
        timeRemaining.text = "";
        frontBarrier.SetActive(true);
        rearBarrier.SetActive(false);

        greenLights.SetActive(false);
        yellowLights.SetActive(false);
        redLights.SetActive(false);

        time = 0f;
        CancelInvoke("StartTimer");
        CancelInvoke("SpawnCars");

        Vector3 pos = new Vector3(startLocation.position.x + 20f, car.transform.position.y, startLocation.position.z);
        car.transform.position = pos;
        car.transform.rotation = startLocation.rotation;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        testInProgress = false;

        foreach(GameObject vehicle in vehicles) {
            Destroy(vehicle, 0f);
        }
    }
}
