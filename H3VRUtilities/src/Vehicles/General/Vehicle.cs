using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace H3VRUtils.Vehicles
{
	class Vehicle : MonoBehaviour
	{
		private Rigidbody rb;
		[Tooltip("The maximum acheivable speed of the car. Not used yet.")]
		public float maxSpeed;
		[Tooltip("The maximum acheivable acceleration of the car.")]
		public float maxAcceleration;
		[Tooltip("The maximum acheivable rotation of the car.")]
		public float maxRotation;
		[Tooltip("The maximum acheivable breaking force of the car.")]
		public float maxDampBreak;

		[Tooltip("The passive damping force on parking gear.")]
		public float dampParkingGear;
		[Tooltip("The passive damping force on neutral gear.")]
		public float dampNeutralGear;
		[Tooltip("The passive damping force on drive gear.")]
		public float dampDriveGear;

		public List<WheelInfo> TireGroups;

		public bool debug;
		public DriveShift.DriveShiftPosition ShiftPos;
		public float Rotation;
		public float Acceleration;
		public float downPressure;
		public float dPmult;
		public float sidePushBack;
		public float BrakingForce;

		public VehicleAudioSet AudioSet;

		public GameObject springloc;
		public float springlocHeight;

		public GameObject spedometerNeedle;
		public float spedometerMaxSpeed;
		public float spedometerMaxRotation;
		public WheelCollider spedometerMeasurer;
		public enum MeasurementSystems
		{
			Imperial,
			Metric
		}
		public MeasurementSystems MeasurementType;

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

		void Start()
		{
			rb = GetComponent<Rigidbody>();
			idleAudioSource = AudioCentre.AddComponent<AudioSource>();
			idleAudioSource = setAudioSource(idleAudioSource);
			idleAudioSource.clip = AudioSet.RevLoop.Clips[0];

			/*interimAudioSource = AudioCentre.AddComponent<AudioSource>();
			interimAudioSource = setAudioSource(interimAudioSource);
			interimAudioSource.clip = AudioSet.RevLoop.Clips[0];*/

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

		//applies rotation of wheel colliders to their childs (which should be the mesh)
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
			var _cir = Mathf.PI * 2 * spedometerMeasurer.radius; //get circumference in metres
			var _rpm = spedometerMeasurer.rpm; //yoink rpm
			var _mpm = _rpm * _cir; //metres per minute
			var _kmh = (_mpm / 1000) * 60; //metres per minute / 1000 to km per minute, * 60 to km per hour
			if (mt == MeasurementSystems.Imperial)
			{
				_kmh *= 0.6213712f; //miles per hour
			} //yes, all the fucking measurements measure in mph if you select this, cry about it
			return _kmh;
		}

		public void Update()
		{
			//anti-tipping. i'm not even sure what it does myself lol
			springloc.transform.position = new Vector3(transform.position.x, transform.position.y + springlocHeight, transform.position.z);

			var speed = GetSpeed();

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
			var voldistmult = Mathf.InverseLerp(maxDistListen, 0, distFromPlayer); //volume mult based on distance
			var volspeedmult = Mathf.Lerp(VolIdle, VolMaxSpeed, Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(GetSpeed()))); //volume mult based on speed
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
			//get speed effect
			float pitchspeedmult = Mathf.Lerp(PitchIdle, PitchMaxSpeed, Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(GetSpeed())));
			return pitchspeedmult * pitchdiffmult;
		}

		public void FixedUpdate()
		{
			//get player distance
			prevDistFromPlayer = distFromPlayer;
			distFromPlayer = Vector3.Distance(transform.position, GM.CurrentPlayerBody.transform.position);

			//enable complete kinematic locking
			//this is mainly to allow machine guns to lock to it
			//it's been removed because locking physics surprisingly fucks with physics
			/*if (spedometerMeasurer != null)
			{
				if (   spedometerMeasurer.rpm * 60 < 0.15 //basically stopped
					&& ShiftPos == DriveShift.DriveShiftPosition.Park //in park
					&& spedometerMeasurer.isGrounded //make sure no floaty
					)
				{
					rb.isKinematic = true;
				}
				else
				{
					rb.isKinematic = false;
				}
			}*/

			//ensure that the rotation isn't out of bounds. i would use mathf.clamp but i'm too paranoid
			if (Rotation > maxRotation)
			{
				Rotation = maxRotation;
			}
			else if (Rotation < -maxRotation)
			{
				Rotation = -maxRotation;
			}

			//get applied dampening and acceleration
			float AppliedDampening = 0f;
			float AppliedAcceleration = Acceleration;
			switch (ShiftPos)
			{
				case DriveShift.DriveShiftPosition.Park:
					AppliedDampening = dampParkingGear;
					break;
				case DriveShift.DriveShiftPosition.Reverse:
					AppliedDampening = dampDriveGear;
					AppliedAcceleration = -AppliedAcceleration;
					break;
				case DriveShift.DriveShiftPosition.Neutral:
					AppliedDampening = dampNeutralGear;
					AppliedAcceleration = 0f;
					break;
				case DriveShift.DriveShiftPosition.Drive:
					AppliedDampening = dampDriveGear;
					break;
			}
			AppliedDampening = AppliedDampening + BrakingForce;


			//i feel like this should be in update but idc
			foreach (var tiregroup in TireGroups)
			{
				foreach (var tire in tiregroup.Tires)
				{
					if (tiregroup.drives)
					{
						if (!isEngineForcedShutOff)
						{ //apply acceleration unless engine off
							tire.motorTorque = AppliedAcceleration;
						}

						tire.wheelDampingRate = AppliedDampening;

						//brakes if moving in reverse and in drive
						if (spedometerMeasurer.rpm < 0 && ShiftPos == DriveShift.DriveShiftPosition.Drive)
						{
							tire.wheelDampingRate = AppliedDampening + maxDampBreak;
						}
					}
					if (tiregroup.steers)
					{
						tire.steerAngle = Rotation;
					}

					if (tiregroup.locks && ShiftPos == DriveShift.DriveShiftPosition.Park)
					{
						tire.wheelDampingRate = AppliedDampening;
					}
					else
					{
						tire.wheelDampingRate = 0;
					}

					ApplyLocalPositionToVisuals(tire);
				}
			}

			//attempted anti-tipping methods. not even sure if it works tbh
			rb.AddRelativeForce(0, -(downPressure + dPmult * Acceleration), 0);
			rb.AddRelativeTorque(0, 0, -(transform.localEulerAngles.z * sidePushBack));
		}

		public void setRotation(float rotation)
		{
			if (debug) return;
			Rotation = rotation;
		}

		public void setDriveShift(DriveShift.DriveShiftPosition dsp)
		{
			if (debug) return;
			if (ShiftPos == DriveShift.DriveShiftPosition.Park && dsp != DriveShift.DriveShiftPosition.Park)
			{
				SM.PlayGenericSound(AudioSet.VehicleStart, transform.position);
				idleAudioSource.Play();
			}
			if (ShiftPos != DriveShift.DriveShiftPosition.Park && dsp == DriveShift.DriveShiftPosition.Park)
			{
				SM.PlayGenericSound(AudioSet.VehicleStop, transform.position);
				idleAudioSource.Stop();
			}
			ShiftPos = dsp;
		}

		public void setAcceleration(float acceleration)
		{
			if (debug) return;
			Acceleration = acceleration;
		}

		public void setBraking(float braking)
		{
			if (debug) return;
			BrakingForce = braking;
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

	[System.Serializable]
	public class WheelInfo
	{
		public List<WheelCollider> Tires;
		public bool drives;
		public bool steers;
		public bool locks;
	}
}
