using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class WallTest : MonoBehaviour
{
    public GameObject car;
    public BasicVehControls carScript;
    public GameObject greenLights, yellowLights, redLights;
    public Transform startLocation;
    public float timeLimit = 60f;
    public GameObject frontBarrier, rearBarrier;
    float time;
    public TMP_Text timeRemaining;
    public GameObject text1, text2, text3, textGo;
    bool testInProgress;
    public GameObject[] walls = new GameObject[6];
    public GameObject[] brickWalls = new GameObject[6];
    public GameObject wallPrefab;

    void Start() {
        time = 0f;
        timeRemaining.text = "";
        testInProgress = false;

        rearBarrier.SetActive(false);
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        textGo.SetActive(false);

        greenLights.SetActive(false);
        yellowLights.SetActive(false);
        redLights.SetActive(false);

        SetBarriers();
    }

    void Update() {
        if(car.transform.position.x < -80f && !testInProgress) {
            StartTest();
        }
        if(testInProgress && Input.GetKeyDown(KeyCode.R)) {
            Reset();
        }
    }

    void StartTest() {
        rearBarrier.SetActive(true);
        frontBarrier.SetActive(false);
        carScript.enabled = false;

        InvokeRepeating("StartTimer", 0f, .01f);
        testInProgress = true;

        Vector3 pos = new Vector3(startLocation.position.x, car.transform.position.y, startLocation.position.z);
        car.transform.position = pos;
        car.transform.rotation = startLocation.rotation;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void SetBarriers() {
        for(int i = 0; i < 6; i += 2) {
            if(UnityEngine.Random.Range(0f, 2f) > 1f) {
                walls[i].GetComponent<BoxCollider>().enabled = true;
                walls[i + 1].GetComponent<BoxCollider>().enabled = false;
            }
            else {
                walls[i].GetComponent<BoxCollider>().enabled = false;
                walls[i + 1].GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    void ResetWalls() {
        foreach(GameObject wall in brickWalls) {
            Instantiate(wallPrefab, wall.transform.position, wall.transform.rotation);
            Destroy(wall);
        }
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

            if(car.transform.position.x <= -239f) {
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
            if(car.transform.position.x <= -239f) {
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

        Vector3 pos = new Vector3(startLocation.position.x + 20f, car.transform.position.y, startLocation.position.z);
        car.transform.position = pos;
        car.transform.rotation = startLocation.rotation;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        testInProgress = false;

        SetBarriers();
        ResetWalls();
    }
}
