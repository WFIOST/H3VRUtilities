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

		public void Update()
		{
			//anti-tipping
			springloc.transform.position = new Vector3(transform.position.x, transform.position.y + springlocHeight, transform.position.z);

			//measure speed
			var _cir = Mathf.PI * 2 * spedometerMeasurer.radius; //get circumference in metres
			var _rpm = spedometerMeasurer.rpm; //yoink rpm
			var _mpm = _rpm * _cir; //metres per minute
			var _kmh = (_mpm / 1000) * 60; //metres per minute / 1000 to km per minute, * 60 to km per hour
			if (MeasurementType == MeasurementSystems.Imperial)
			{
				_kmh *= 0.6213712f; //miles per hour
			}

			try
			{
				spedometerNeedle.transform.localEulerAngles = new Vector3(
					0,
					Mathf.Lerp(0, spedometerMaxRotation, //get the rotation of needle based on percentage dist in spedometer
					Mathf.InverseLerp(0, spedometerMaxSpeed, _kmh) //get percentage dist in spedometer
					),
					0);
			}
			catch { }

			idleAudioSource.pitch = Mathf.Lerp(PitchIdle, PitchMaxSpeed, Mathf.InverseLerp(0, maxSpeed, _kmh));
			idleAudioSource.volume = Mathf.Lerp(VolIdle, VolMaxSpeed, Mathf.InverseLerp(0, maxSpeed, _kmh));
		}

		public void FixedUpdate()
		{
			//enable complete kinematic locking
			//this is mainly to allow machine guns to lock to it
			if (spedometerMeasurer != null)
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
			}





			if (Rotation > maxRotation)
			{
				Rotation = maxRotation;
			}
			if (Rotation < -maxRotation)
			{
				Rotation = -maxRotation;
			}

			foreach (var tiregroup in TireGroups)
			{
				foreach (var tire in tiregroup.Tires)
				{
					tire.wheelDampingRate = BrakingForce;

					if (tiregroup.drives)
					{
						if (ShiftPos != DriveShift.DriveShiftPosition.Neutral)
						{
							var acc = Acceleration;
							if (ShiftPos == DriveShift.DriveShiftPosition.Reverse) acc = -acc;
							tire.motorTorque = acc;
							tire.wheelDampingRate += dampNeutralGear;
						}
						else
						{
							tire.wheelDampingRate += dampDriveGear;
						}
					}
					if (tiregroup.steers)
					{
						tire.steerAngle = Rotation;
					}
					if (tiregroup.locks && ShiftPos == DriveShift.DriveShiftPosition.Park)
					{
						tire.wheelDampingRate += dampParkingGear;
					}
					else
					{
						tire.brakeTorque = 0;
					}

					ApplyLocalPositionToVisuals(tire);
				}
			}
			rb.AddRelativeForce(0, -(downPressure + dPmult * Acceleration), 0);
			rb.AddRelativeTorque(0, 0, -(transform.localEulerAngles.z * sidePushBack));

			/*if (ShiftPos != DriveShift.DriveShiftPosition.Park)
			{
				SM.PlayGenericSound(AudioSet.VehicleIdle, transform.position);
			}*/
		}

		public void setRotation(float rotation)
		{
			if (debug) return;
			Rotation = rotation;
		}

		public void setDriveShift(DriveShift.DriveShiftPosition dsp)
		{
			if (debug) return;
			if(ShiftPos == DriveShift.DriveShiftPosition.Park && dsp != DriveShift.DriveShiftPosition.Park)
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
