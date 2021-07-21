using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace H3VRUtils.Vehicles
{
	public class Vehicle : MonoBehaviour
	{
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

		[FormerlySerializedAs("TireGroups")] public List<WheelInfo> tireGroups;

		public bool debug;
		[FormerlySerializedAs("ShiftPos")] public DriveShift.DriveShiftPosition shiftPos;
		[FormerlySerializedAs("Rotation")] public float rotation;
		[FormerlySerializedAs("Acceleration")] public float acceleration;
		public float sidePushBack;
		[FormerlySerializedAs("BrakingForce")] public float brakingForce;

		[FormerlySerializedAs("AudioSet")] public VehicleAudioSet audioSet;

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
		[FormerlySerializedAs("MeasurementType")] public MeasurementSystems measurementType;

		[FormerlySerializedAs("AudioCentre")] public GameObject audioCentre;
		[FormerlySerializedAs("PitchIdle")] public float pitchIdle;
		[FormerlySerializedAs("PitchMaxSpeed")] public float pitchMaxSpeed;
		[FormerlySerializedAs("VolIdle")] public float volIdle;
		[FormerlySerializedAs("VolMaxSpeed")] public float volMaxSpeed;
		public float maxDistListen = 50f;
		
		private AudioSource _idleAudioSource;
		//private AudioSource interimAudioSource;
		private AudioSource _brakeAudioSource;
		private Rigidbody _rb;
		private float _distFromPlayer;
		private float _prevDistFromPlayer;
		private bool _isEngineForcedShutOff;

		void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_idleAudioSource = audioCentre.AddComponent<AudioSource>();
			_idleAudioSource = SetAudioSource(_idleAudioSource);
			_idleAudioSource.clip = audioSet.revLoop.Clips[0];

			/*interimAudioSource = AudioCentre.AddComponent<AudioSource>();
			interimAudioSource = setAudioSource(interimAudioSource);
			interimAudioSource.clip = AudioSet.RevLoop.Clips[0];*/

			_brakeAudioSource = audioCentre.AddComponent<AudioSource>();
			_brakeAudioSource = SetAudioSource(_brakeAudioSource);
			_brakeAudioSource.clip = audioSet.brake.Clips[0];
		}

		AudioSource SetAudioSource(AudioSource audsrc)
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
			return GetSpeed(measurementType);
		}

		public float GetSpeed(MeasurementSystems mt)
		{
			float cir = Mathf.PI * 2 * spedometerMeasurer.radius; //get circumference in metres
			float rpm = spedometerMeasurer.rpm; //yoink rpm
			float mpm = rpm * cir; //metres per minute
			float kmh = (mpm / 1000) * 60; //metres per minute / 1000 to km per minute, * 60 to km per hour
			if (mt == MeasurementSystems.Imperial)
			{
				kmh *= 0.6213712f; //miles per hour
			} //yes, all the fucking measurements measure in mph if you select this, cry about it
			return kmh;
		}

		public void Update()
		{
			//anti-tipping. i'm not even sure what it does myself lol
			springloc.transform.position = new Vector3(transform.position.x, transform.position.y + springlocHeight, transform.position.z);

			float speed = GetSpeed();

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


			_idleAudioSource.pitch = GetEnginePitch();

			_idleAudioSource.volume = GetEngineVolume();
		}

		public float GetEngineVolume()
		{
			float voldistmult = Mathf.InverseLerp(maxDistListen, 0, _distFromPlayer); //volume mult based on distance
			float volspeedmult = Mathf.Lerp(volIdle, volMaxSpeed, Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(GetSpeed()))); //volume mult based on speed
			float voloverride = 1f;
			if (_isEngineForcedShutOff) voloverride = 0f; //if engine is off multiply engine volume by 0
			return voldistmult * volspeedmult * voloverride;
		}


		public float GetEnginePitch()
		{
			//get doppler effect
			float diff = (_distFromPlayer - _prevDistFromPlayer) * 50f; //difference between two frames in m/s
			diff = diff * 3.6f; //conversion from ms to kmh
			float doppleraffectmult = 0.4f;
			float pitchdiffmult = (diff * -0.005f * doppleraffectmult) + 1f; // * 0.05 ensures a max of +/- 100% pitch at 200kmh; the - inverts the result
			pitchdiffmult = Mathf.Clamp(pitchdiffmult, 0.05f, 2f);
			//get speed effect
			float pitchspeedmult = Mathf.Lerp(pitchIdle, pitchMaxSpeed, Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(GetSpeed())));
			return pitchspeedmult * pitchdiffmult;
		}

		public void FixedUpdate()
		{
			//get player distance
			_prevDistFromPlayer = _distFromPlayer;
			_distFromPlayer = Vector3.Distance(transform.position, GM.CurrentPlayerBody.transform.position);

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
			if (rotation > maxRotation)
			{
				rotation = maxRotation;
			}
			else if (rotation < -maxRotation)
			{
				rotation = -maxRotation;
			}

			//get applied dampening and acceleration
			float appliedDampening = 0f;
			float appliedAcceleration = acceleration;
			switch (shiftPos)
			{
				case DriveShift.DriveShiftPosition.Park:
					appliedDampening = dampParkingGear;
					break;
				case DriveShift.DriveShiftPosition.Reverse:
					appliedDampening = dampDriveGear;
					appliedAcceleration = -appliedAcceleration;
					break;
				case DriveShift.DriveShiftPosition.Neutral:
					appliedDampening = dampNeutralGear;
					appliedAcceleration = 0f;
					break;
				case DriveShift.DriveShiftPosition.Drive:
					appliedDampening = dampDriveGear;
					break;
			}
			appliedDampening = appliedDampening + brakingForce;


			//i feel like this should be in update but idc
			foreach (WheelInfo tiregroup in tireGroups)
			{
				foreach (WheelCollider tire in tiregroup.tires)
				{
					if (tiregroup.drives)
					{
						if (!_isEngineForcedShutOff)
						{ //apply acceleration unless engine off
							tire.motorTorque = appliedAcceleration;
						}

						tire.wheelDampingRate = appliedDampening;

						//brakes if moving in reverse and in drive
						if (spedometerMeasurer.rpm < 0 && shiftPos == DriveShift.DriveShiftPosition.Drive)
						{
							tire.wheelDampingRate = appliedDampening + maxDampBreak;
						}
					}
					if (tiregroup.steers)
					{
						tire.steerAngle = rotation;
					}

					if (tiregroup.locks && shiftPos == DriveShift.DriveShiftPosition.Park)
					{
						tire.wheelDampingRate = appliedDampening;
					}
					else
					{
						tire.wheelDampingRate = 0;
					}

					ApplyLocalPositionToVisuals(tire);
				}
			}
		}

		public void SetRotation(float rotation)
		{
			if (debug) return;
			this.rotation = rotation;
		}

		public void SetDriveShift(DriveShift.DriveShiftPosition dsp)
		{
			if (debug) return;
			if (shiftPos == DriveShift.DriveShiftPosition.Park && dsp != DriveShift.DriveShiftPosition.Park)
			{
				if (_isEngineForcedShutOff) SM.PlayGenericSound(audioSet.vehicleStart, transform.position);
				_idleAudioSource.Play();
			}
			if (shiftPos != DriveShift.DriveShiftPosition.Park && dsp == DriveShift.DriveShiftPosition.Park)
			{
				if (_isEngineForcedShutOff) SM.PlayGenericSound(audioSet.vehicleStop, transform.position);
				_idleAudioSource.Stop();
			}
			shiftPos = dsp;
		}

		public void SetAcceleration(float acceleration)
		{
			if (debug) return;
			this.acceleration = acceleration;
		}

		public void SetBraking(float braking)
		{
			if (debug) return;
			brakingForce = braking;
		}

		public void ToggleEngine(bool set)
		{
			if (!set) //if engine is set off
			{
				_isEngineForcedShutOff = true;
			}
			else //if engine is set on
			{
				_isEngineForcedShutOff = false;
			}
		}
	}

	[System.Serializable]
	public class WheelInfo
	{
		[FormerlySerializedAs("Tires")] public List<WheelCollider> tires;
		public bool drives;
		public bool steers;
		public bool locks;
	}
}
