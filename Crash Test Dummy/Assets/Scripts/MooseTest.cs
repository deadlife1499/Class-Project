using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MooseTest : MonoBehaviour
{
    public GameObject car;
    public BasicVehControls carScript;
    public GameObject greenLights, yellowLights, redLights;
    public Transform startLocation;
    public float timeLimit = 25f;
    public GameObject frontBarrier, rearBarrier;
    float time;
    public TMP_Text timeRemaining;
    public GameObject text1, text2, text3, textGo;
    bool testInProgress;

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
    }

    void Update() {
        if(car.transform.position.x < 304f && !testInProgress) {
            StartMooseTest();
        }
    }

    void StartMooseTest() {
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

            if(car.transform.position.x <= 193f) {
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
            if(car.transform.position.x <= 193f) {
                this.enabled = false;
            }
            else{
                Reset();
            }
        }
    }

    void Reset() {
        Vector3 pos = new Vector3(startLocation.position.x + 20f, car.transform.position.y, startLocation.position.z);
        car.transform.position = pos;
        car.transform.rotation = startLocation.rotation;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        testInProgress = false;
    }
}
