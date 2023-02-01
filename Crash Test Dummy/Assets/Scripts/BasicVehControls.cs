using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicVehControls : MonoBehaviour {
	[Space(20)]
	//[Header("Camera")]

	[Range(0, 119)]
	public int minFOV = 55;
	[Range(0, minFOV)]
	public int maxFOV = 90;

    //[Header("Car Setup")]
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
    [Range(10, 120)]
    public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
    [Range(1, 10)]
    public int accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350; // The strength of the wheel brakes.
    [Range(1, 10)]
    public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
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

	float carSpeed;
    bool isDrifting;
    bool isTractionLocked;

	Rigidbody rb;
    float steeringAxis;
    float throttleAxis;
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;

	void Start() {
		carRigidbody = gameObject.GetComponent<Rigidbody>();
      	carRigidbody.centerOfMass = bodyMassCenter;

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
      	localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

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
	}

	public void CarSounds(){
      	if(useSounds){
        	try{
          		if(carEngineSound != null){
            		float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
            		carEngineSound.pitch = engineSoundPitch;
          		}
        	}catch(Exception ex){
          		Debug.LogWarning(ex);
        	}
      	}else if(!useSounds){
        	if(carEngineSound != null && carEngineSound.isPlaying){
          		carEngineSound.Stop();
        	}
      	}
    }

	public void TurnLeft(){
      	steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      	if(steeringAxis < -1f){
        	steeringAxis = -1f;
      	}

      	var steeringAngle = steeringAxis * maxSteeringAngle;
      	frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      	frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

	public void TurnRight(){
      	steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      	if(steeringAxis > 1f){
        	steeringAxis = 1f;
      	}
      	var steeringAngle = steeringAxis * maxSteeringAngle;
      	frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      	frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

	public void ResetSteeringAngle(){
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
      	try{
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
      	}catch(Exception ex){
        	Debug.LogWarning(ex);
      	}
    }
}