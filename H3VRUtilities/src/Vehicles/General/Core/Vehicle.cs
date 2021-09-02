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
		public Engine engine;
		[Header("Debug Values")]
		public bool debug;
		public Text RPMtext;
		
		[Header("Core Vehicle Values")]
		public float Rotation;
		public float brakingForce;
		public float Throttle;

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
		/*public float engineToTransmissionInfluence = 0.9f;
		public float transmissionToEngineInfluence = 0.9f;
		public float groundToTransmissionInfluence = 0.9f;*/

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
		public bool isHandbrakeOn;



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
			//idleAudioSource.pitch = GetEnginePitch();
			//idleAudioSource.volume = GetEngineVolume();
		}

		private void UpdatePlayerDist()
		{
			//mainly for doppler effect
			prevDistFromPlayer = distFromPlayer;
			distFromPlayer = Vector3.Distance(transform.position, GM.CurrentPlayerBody.transform.position);
		}
		
		public void FixedUpdate()
		{
			//UpdatePlayerDist();

			//debug bits
			if (RPMtext != null)
			{
				RPMtext.text = ((int)engineRpm).ToString(CultureInfo.InvariantCulture);
			}
			
			//get the amount travelled (should be anyway) every fixedTimeStep-th of a second (50th of a second)
			//float wheelCircumference = Mathf.PI * wheelDiameter;
			//TODO: see first todo
			//60 seconds in a min * 50 steps in a sec = 3000
			//this is the distance moved every 
			//distmov = (transmissionRpm / 3000) * wheelCircumference;
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

		public void setThrottle(float throttle)
		{
			if (debug) return;
			Throttle = throttle;
		}

		public void setBraking(float braking)
		{
			if (debug) return;
			brakingForce = braking;
		}
	}
}
