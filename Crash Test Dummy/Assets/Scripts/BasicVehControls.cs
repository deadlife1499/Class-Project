using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicVehControls : MonoBehaviour {
	[Space(20)]
	//[Header("Camera")]

	public bool doDynamicFOV = false;
	public Camera camera;
	[Range(0, 119)]
	public float minFOV = 55;
	[Range(1, 120)]
	public float maxFOV = 90;

    //[Header("Car Setup")]
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90;
    [Range(10, 120)]
    public int maxReverseSpeed = 45;
    [Range(1, 10)]
    public int accelerationMultiplier = 2; 
    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27; 
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;
    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350;
    [Range(1, 10)]
    public int decelerationMultiplier = 2;
    [Space(10)]
    public Vector3 bodyMassCenter;

	[Space(20)]
	//[Header("Wheels")]
	[Space(10)]
	public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

	[Space(20)]
    //[Header("EFFECTS")]
    [Space(10)]
    public bool useEffects = false;
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

	[Space(20)]
    //[Header("Sounds")]
    [Space(10)]
    public bool useSounds = false;
    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;
    float initialCarEngineSoundPitch;

	public float carSpeed;

	Rigidbody rb;
    float steeringAxis;
    float throttleAxis;
    float localVelocityZ;
    bool deceleratingCar;

	void Start() {
		rb = gameObject.GetComponent<Rigidbody>();
      	rb.centerOfMass = bodyMassCenter;
		camera.fieldOfView = minFOV;

		if(carEngineSound != null){
          	initialCarEngineSoundPitch = carEngineSound.pitch;
        }

		if(useSounds){
          	InvokeRepeating("CarSounds", 0f, 0.1f);
        }
		else if(!useSounds){
          	if(carEngineSound != null){
            	carEngineSound.Stop();
          	}
          	if(tireScreechSound != null){
            	tireScreechSound.Stop();
          	}
        }
	}

	void Update() {
		carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
      	localVelocityZ = transform.InverseTransformDirection(rb.velocity).z;

        if(Input.GetKey(KeyCode.W)){
          	CancelInvoke("DecelerateCar");
          	deceleratingCar = false;
          	GoForward();
        }
        if(Input.GetKey(KeyCode.S)){
          	CancelInvoke("DecelerateCar");
          	deceleratingCar = false;
          	GoReverse();
        }

        if(Input.GetKey(KeyCode.A)){
          	TurnLeft();
        }
        if(Input.GetKey(KeyCode.D)){
          	TurnRight();
        }
        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
          	ThrottleOff();
        }
        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
          	InvokeRepeating("DecelerateCar", 0f, 0.1f);
          	deceleratingCar = true;
        }
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f){
          	ResetSteeringAngle();
        }

		AnimateWheelMeshes();
		if(doDynamicFOV && maxFOV > minFOV) {
			SetFOV();
		}
	}

	void SetFOV() {
		float oldFOV = camera.fieldOfView;
		float multiplier = carSpeed * 20f / maxSpeed;
		float diffFOV = maxFOV - minFOV;
		float newFOV = multiplier * diffFOV + minFOV;

		if(newFOV > oldFOV) {
			camera.fieldOfView += .04f;
		}
		else if(newFOV < oldFOV) {
			camera.fieldOfView -= .1f;
		}
	}

	void CarSounds(){
      	if(useSounds){
          	if(carEngineSound != null){
            	float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(rb.velocity.magnitude) / 25f);
            	carEngineSound.pitch = engineSoundPitch;
          	}
      	}else if(!useSounds){
        	if(carEngineSound != null && carEngineSound.isPlaying){
          		carEngineSound.Stop();
        	}
      	}
    }

	void TurnLeft(){
      	steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      	if(steeringAxis < -1f){
        	steeringAxis = -1f;
      	}

      	var steeringAngle = steeringAxis * maxSteeringAngle;
      	frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      	frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

	void TurnRight(){
      	steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      	if(steeringAxis > 1f){
        	steeringAxis = 1f;
      	}
      	var steeringAngle = steeringAxis * maxSteeringAngle;
      	frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      	frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

	void ResetSteeringAngle(){
      	if(steeringAxis < 0f){
        	steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      	}else if(steeringAxis > 0f){
        	steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      	}
      	if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f){
        	steeringAxis = 0f;
      	}

      	var steeringAngle = steeringAxis * maxSteeringAngle;
      	frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      	frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

	void AnimateWheelMeshes(){
        Quaternion FLWRotation;
        Vector3 FLWPosition;
        frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
        frontLeftMesh.transform.position = FLWPosition;
        frontLeftMesh.transform.rotation = FLWRotation;

        Quaternion FRWRotation;
        Vector3 FRWPosition;
        frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
        frontRightMesh.transform.position = FRWPosition;
        frontRightMesh.transform.rotation = FRWRotation;

        Quaternion RLWRotation;
        Vector3 RLWPosition;
        rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
        rearLeftMesh.transform.position = RLWPosition;
        rearLeftMesh.transform.rotation = RLWRotation;

        Quaternion RRWRotation;
        Vector3 RRWPosition;
        rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
        rearRightMesh.transform.position = RRWPosition;
        rearRightMesh.transform.rotation = RRWRotation;
    }

	void GoForward(){
      	throttleAxis = throttleAxis + (Time.deltaTime * 3f);
      	if(throttleAxis > 1f){
        	throttleAxis = 1f;
      	}

      	if(localVelocityZ < -1f){
        	Brakes();
      	}else{
        	if(Mathf.RoundToInt(carSpeed) < maxSpeed){
          		frontLeftCollider.brakeTorque = 0;
          		frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          		frontRightCollider.brakeTorque = 0;
          		frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          		rearLeftCollider.brakeTorque = 0;
          		rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          		rearRightCollider.brakeTorque = 0;
          		rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        	}else {
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          		rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      	}
    }

	void GoReverse(){
      	throttleAxis = throttleAxis - (Time.deltaTime * 3f);
      	if(throttleAxis < -1f){
        	throttleAxis = -1f;
      	}

      	if(localVelocityZ > 1f){
        	Brakes();
      	}else{
        	if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
          		frontLeftCollider.brakeTorque = 0;
          		frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          		frontRightCollider.brakeTorque = 0;
          		frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          		rearLeftCollider.brakeTorque = 0;
          		rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          		rearRightCollider.brakeTorque = 0;
          		rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        	}else {
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          		rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      	}
    }

	void ThrottleOff(){
      	frontLeftCollider.motorTorque = 0;
      	frontRightCollider.motorTorque = 0;
      	rearLeftCollider.motorTorque = 0;
      	rearRightCollider.motorTorque = 0;
    }

	void DecelerateCar(){
      	if(throttleAxis != 0f){
        	if(throttleAxis > 0f){
          		throttleAxis = throttleAxis - (Time.deltaTime * 10f);
        	}else if(throttleAxis < 0f){
            	throttleAxis = throttleAxis + (Time.deltaTime * 10f);
        	}
        	if(Mathf.Abs(throttleAxis) < 0.15f){
          		throttleAxis = 0f;
        	}
      	}
      	rb.velocity = rb.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
      	frontLeftCollider.motorTorque = 0;
      	frontRightCollider.motorTorque = 0;
      	rearLeftCollider.motorTorque = 0;
      	rearRightCollider.motorTorque = 0;
      	if(rb.velocity.magnitude < 0.25f){
        	rb.velocity = Vector3.zero;
        	CancelInvoke("DecelerateCar");
      	}
    }

	void Brakes(){
      	frontLeftCollider.brakeTorque = brakeForce;
      	frontRightCollider.brakeTorque = brakeForce;
      	rearLeftCollider.brakeTorque = brakeForce;
      	rearRightCollider.brakeTorque = brakeForce;
    }
}