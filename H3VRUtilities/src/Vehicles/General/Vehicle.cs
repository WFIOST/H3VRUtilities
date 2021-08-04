using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace H3VRUtils.Vehicles
{
	public class Vehicle : MonoBehaviour
	{
		//TODO: please change all timestep values based on Time.fixedDeltaTime please; don't just keep using * 0.02
		private Rigidbody _rigidBody;
		[Header("Debug Values")]
		public bool debug;
		public Text RPMtext;
		
		[Header("Core Vehicle Values")]
		public float maxRotation;
		public float maxBreakingForce;
		public List<GearInfo> Gears;
		public float Rotation;
		public float Acceleration;
		[Tooltip("Measured in Torque, not RPM.")]
		public float brakingForce;
		public SpringJoint springloc;
		public float springlocheight;

		[Header("Engine Details")]
		public float maxHorsePower;
		[Tooltip("In max RPM increase per second.")]
		public float rpmIncreaseMult = 1500;
		public float rpmIdle = 1000;
		public float rpmMaxHorsepower = 7000;
		public float rpmMax = 9000;
		public float rpmNatDecrease = 0.3f;

		[Header("Wheels And Suspension")]
		public List<WheelInfo> wheels;
		public float wheelDiameter;
		public float floorDist = 0.3f;

		[Header("Measurement Tools")]
		public GameObject spedometerNeedle;
		public float spedometerMaxSpeed;
		public float spedometerMaxRotation;
		public enum MeasurementSystems { Imperial, Metric }
		public MeasurementSystems MeasurementType;


		[Header("Audio")]
		public VehicleAudioSet AudioSet;
		public GameObject AudioCentre;
		public float PitchIdle;
		public float PitchMaxSpeed;
		public float VolIdle;
		public float VolMaxSpeed;
		private AudioSource idleAudioSource;
		//private AudioSource interimAudioSource;
		private AudioSource brakeAudioSource;
		public float maxDistListen = 50f;

		private float distFromPlayer;
		private float prevDistFromPlayer;
		private bool isEngineForcedShutOff;
		
		[Header("Fine Tunement")]
		public float velocitySlerpSpeed = 0.025f;
		public float dragRetention = 0.975f;
		public float torqueToImpulse;
		public float rotationToTorque;
		public float engineToTransmissionInfluence = 0.9f;
		public float transmissionToEngineInfluence = 0.9f;
		public float groundToTransmissionInfluence = 0.9f;

		[Header("Viewable Values")]
		public int currentGear;
		public float engineRpm;
		public float engineHorsePower;
		public float engineTorque;
		
		public float transmissionRpm;
		//public float transmissionHorsePower;
		public float transmissionTorque;

		
		public float distmov;
		
		public float totalSpeedKMH;
		public float engineVol;
		public float enginePitch;
		public bool isOnGround;
		public float angularDrag;
		public bool isSlowerThanExpected;



		void Start()
		{
			_rigidBody = GetComponent<Rigidbody>();
			idleAudioSource = AudioCentre.AddComponent<AudioSource>();
			idleAudioSource = setAudioSource(idleAudioSource);
			idleAudioSource.clip = AudioSet.RevLoop.Clips[0];
			idleAudioSource.Play();
			
			brakeAudioSource = AudioCentre.AddComponent<AudioSource>();
			brakeAudioSource = setAudioSource(brakeAudioSource);
			brakeAudioSource.clip = AudioSet.Brake.Clips[0];
		}
		AudioSource setAudioSource(AudioSource audsrc)
		{
			audsrc.loop = true;
			audsrc.playOnAwake = false;
			audsrc.minDistance = 0.1f;
			return audsrc;
		}
		public void ApplyLocalPositionToVisuals(WheelCollider collider)
		{
			//not yoinked from unity docs
			if (collider.transform.childCount == 0)
			{
				return;
			}

			Transform visualWheel = collider.transform.GetChild(0);

			Vector3 position;
			Quaternion rotation;
			collider.GetWorldPose(out position, out rotation);

			visualWheel.transform.position = position;
			visualWheel.transform.rotation = rotation;
		}
		public float GetSpeed()
		{
			return GetSpeed(MeasurementType);
		}
		public float GetSpeed(MeasurementSystems mt)
		{
			/*//lol cry about the commenting
			var _cir = Mathf.PI * 2 * spedometerMeasurer.radius; //get circumference in metres
			var _rpm = spedometerMeasurer.rpm; //yoink rpm
			var _mpm = _rpm * _cir; //metres per minute
			var _kmh = (_mpm / 1000) * 60; //metres per minute / 1000 to km per minute, * 60 to km per hour*/
			var _kmh = _rigidBody.velocity.magnitude * 3.6f; //mps to kmh is 1:3.6 ratio
			if (mt == MeasurementSystems.Imperial)
			{
				_kmh *= 0.6213712f; //miles per hour
			} //yes, all the fucking measurements measure in mph if you select this, cry about it*/
			return _kmh;
		}
		public void Update()
		{
			var speed = GetSpeed();
			
			totalSpeedKMH = speed;
			
			//spedometer
			try
			{
				spedometerNeedle.transform.localEulerAngles = new Vector3(
					0,
					Mathf.Lerp(0, spedometerMaxRotation, //get the rotation of needle based on percentage dist in spedometer
					Mathf.InverseLerp(0, spedometerMaxSpeed, speed) //get percentage dist in spedometer
					),
					0);
			}
			catch { }

			//engine rev pitch
			idleAudioSource.pitch = GetEnginePitch();
			idleAudioSource.volume = GetEngineVolume();
		}
		public float GetEngineVolume()
		{
			//volume mult based on distance
			var voldistmult = Mathf.InverseLerp(maxDistListen, 0, distFromPlayer);
			//volume mult based on speed
			var volspeedmult = Mathf.Lerp(VolIdle, VolMaxSpeed, Mathf.InverseLerp(0, rpmMax, engineRpm));
			var voloverride = 1f;
			if (isEngineForcedShutOff) voloverride = 0f; //if engine is off multiply engine volume by 0
			return voldistmult * volspeedmult * voloverride;
		}
		public float GetEnginePitch()
		{
			//get doppler effect
			float diff = (distFromPlayer - prevDistFromPlayer) * 50f; //difference between two frames in m/s
			diff = diff * 3.6f; //conversion from ms to kmh
			float doppleraffectmult = 0.4f;
			float pitchdiffmult = (diff * -0.005f * doppleraffectmult) + 1f; // * 0.05 ensures a max of +/- 100% pitch at 200kmh; the - inverts the result
			pitchdiffmult = Mathf.Clamp(pitchdiffmult, 0.05f, 2f);
			//end get doppler effect
			//get RPM effect
			float pitchrpmmult = Mathf.Lerp(PitchIdle, PitchMaxSpeed, Mathf.InverseLerp(0, rpmMax, engineRpm));
			return pitchrpmmult * pitchdiffmult;
		}
		private void UpdatePlayerDist()
		{
			//mainly for doppler effect
			prevDistFromPlayer = distFromPlayer;
			distFromPlayer = Vector3.Distance(transform.position, GM.CurrentPlayerBody.transform.position);
		}
		
		/// <summary>
		/// Called by FixedUpdate. do NOT call this in update lol
		/// </summary>
		private void UpdateEngine()
		{
			//lower/increase engine RPM based on transmission
			float gearRpmThroughput = transmissionRpm * Gears[currentGear].etwGearRatio;
			engineRpm = Mathf.Lerp(engineRpm, gearRpmThroughput, transmissionToEngineInfluence * 0.02f);
			
			//increase Engine RPM
			if (Acceleration >= 0) {
				engineRpm += Acceleration * (rpmIncreaseMult / 50);
			}
			
			//natural engine RPM decrease
			if (Acceleration < 0.05 && engineRpm >= rpmIdle)
				engineRpm = Mathf.Lerp(rpmIdle, engineRpm, 1 - (rpmNatDecrease * 0.02f));

			//clamp engine RPM
			engineRpm = Mathf.Clamp(engineRpm, 0, rpmMax);
			//get current horsepower output
			engineHorsePower = maxHorsePower * Mathf.InverseLerp(0, rpmMaxHorsepower, engineRpm);
			//get engine torque
			//engineTorque = engineHorsePower / (engineRpm / 5252); //fuck this is ftlb not Nm
			engineTorque = 7127 * engineHorsePower / engineRpm; //HP and RPM to Nm Torque
		}
		private void UpdateTransmission()
		{
			//lower/increase transmission RPM based on engine
			float gearRpmThroughput = engineRpm / Gears[currentGear].etwGearRatio;
			transmissionRpm = Mathf.Lerp(transmissionRpm, gearRpmThroughput, engineToTransmissionInfluence * 0.02f);
			
			//lower transmission RPM based on actual speed
			float wheelCircumference = Mathf.PI * wheelDiameter;
			//TODO: see first todo
			//60 seconds in a min * 50 steps in a sec = 3000
			distmov = (transmissionRpm / 3000) * wheelCircumference;
			if (_rigidBody.velocity.magnitude * 0.02 < distmov)
			{
				float newDistmov = Mathf.Lerp(_rigidBody.velocity.magnitude, distmov, 1 - (groundToTransmissionInfluence * 0.02f));
				transmissionRpm = newDistmov / wheelCircumference * 3000;
			}
			
			//activate breaks
			if (Acceleration < 0) {
				transmissionTorque += -(Acceleration * (brakingForce / 50));
			}
			transmissionTorque = 7127 * engineHorsePower / transmissionRpm;
		}
		public void FixedUpdate()
		{
			UpdatePlayerDist();
			UpdateEngine();
			UpdateTransmission();
			
			//ensure that the rotation isn't out of bounds. i would use mathf.clamp but i'm too paranoid
			//please check if we can mathf.clamp this sometime
			if (Rotation > maxRotation) {
				Rotation = maxRotation;
			} else if (Rotation < -maxRotation) {
				Rotation = -maxRotation;
			}
			
			//debug bits
			if (RPMtext != null)
			{
				RPMtext.text = ((int)engineRpm).ToString(CultureInfo.InvariantCulture);
			}
			
			//get the amount travelled (should be anyway) every fixedTimeStep-th of a second (50th of a second)
			float wheelCircumference = Mathf.PI * wheelDiameter;
			//TODO: see first todo
			//60 seconds in a min * 50 steps in a sec = 3000
			//this is the distance moved every 
			distmov = (transmissionRpm / 3000) * wheelCircumference;


			//vehicle movement handler deets
			//TODO: rewrite this sometime please; i cannot imagine this being terribly efficient
			//move forward + make sure is on ground
			foreach (var wheelGroup in wheels)
			{
				foreach (var wheel in wheelGroup.Tires)
				{
					//check if on ground
					RaycastHit hit;
					if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out hit, floorDist)){
						isOnGround = true;
					} else { isOnGround = false; }

					//check if slower than wheel rpm should say tis
					if (_rigidBody.velocity.magnitude / 50 < distmov){
						isSlowerThanExpected = true;
					} else { isSlowerThanExpected = false; }

					if(isOnGround && isSlowerThanExpected)
					{
						//accelerate driving wheels
						if (wheelGroup.drives)
						{
							var vec3 = transform.forward * (transmissionTorque * torqueToImpulse);
							//_rigidBody.AddForceAtPosition(vec3, wheel.transform.position, ForceMode.Impulse);
							_rigidBody.AddForce(vec3, ForceMode.Impulse);
						}
					}
					else if(isOnGround && !isSlowerThanExpected)
					{
						_rigidBody.velocity = _rigidBody.velocity * dragRetention;
					}

					//wheel rotation bits
					var rotvec = new Vector3();
					//step to minute ratio is 3000:1
					rotvec.x = transmissionRpm / 3000; //rots per step
					rotvec.x = rotvec.x * 360; //rots to degrees
					rotvec.x = -rotvec.x;
					wheel.transform.GetChild(0).transform.Rotate(rotvec);
				}
			}

			if (isOnGround)
			{
				//steering
				float rot = Rotation * _rigidBody.velocity.magnitude / 50;
				Quaternion q = Quaternion.AngleAxis(rot, transform.up);
				var targetRot = _rigidBody.rotation * q;
				_rigidBody.MoveRotation(targetRot);

				/*float rot = Rotation * _rigidBody.velocity.magnitude * 0.02f * rotationToTorque;
				_rigidBody.AddRelativeTorque(new Vector3(0, rot, 0), ForceMode.Impulse);*/

				//slerping forward momentum
				_rigidBody.velocity = Vector3.SlerpUnclamped(_rigidBody.velocity, transform.forward * _rigidBody.velocity.magnitude, velocitySlerpSpeed);
				if(_rigidBody.angularDrag == 0 && angularDrag != 0)
				{
					_rigidBody.angularDrag = angularDrag;
				}
			}
			else //if NOT on ground
			{
				//unlock angular drag
				_rigidBody.angularDrag = 0;
			}
		}

		public void setRotation(float rotation)
		{
			if (debug) return;
			Rotation = rotation;
		}

		public void setDriveShift(int Gear)
		{
			if (debug) return;
			currentGear = Gear;
		}

		public void setAcceleration(float acceleration)
		{
			if (debug) return;
			Acceleration = acceleration;
		}

		public void setBraking(float braking)
		{
			if (debug) return;
			brakingForce = braking;
		}

		public void ToggleEngine(bool set)
		{
			if (!set) //if engine is set off
			{
				isEngineForcedShutOff = true;
			}
			else //if engine is set on
			{
				isEngineForcedShutOff = false;
			}
		}
	}

	[Serializable]
	public class GearInfo
	{
		public float etwGearRatio = 3;
		public DriveShift.DriveShiftPosition ShiftPos;
	}
	
	[Serializable]
	public class WheelInfo
	{
		public List<GameObject> Tires;
		public bool drives;
		public bool steers;
		public bool locks;
	}
}
