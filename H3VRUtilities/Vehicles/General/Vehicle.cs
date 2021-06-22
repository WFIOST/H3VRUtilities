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
		public float maxSpeed;
		public float maxAcceleration;
		public float maxRotation;
		public float maxBrakingForce;
		public float parkingBrakeForce;
		public FVRViveHand hand;
		public Transform SitPos;

		public List<WheelInfo> TireGroups;

		public bool debug;
		public DriveShift.DriveShiftPosition ShiftPos;
		public float Rotation;
		public float Acceleration;
		public float downPressure;
		public float dPmult;
		public float sidePushBack;

		public float wheelDampNeutral;
		public float wheelDampDrive;

		public VehicleAudioSet AudioSet;

		void Start()
		{
			rb = GetComponent<Rigidbody>();
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
			if (hand != null)
			{
				hand.MovementManager.transform.position = SitPos.transform.position;
				//hand.MovementManager.transform.rotation = SitPos.transform.rotation;
				var rot = hand.MovementManager.transform.rotation;
				rot.x = SitPos.transform.rotation.x;
				rot.z = SitPos.transform.rotation.z;
				hand.MovementManager.transform.rotation = rot;
			}
			//my fucking head hurts when you do this dont do it
		}

		public void FixedUpdate()
		{
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
					if (tiregroup.drives)
					{
						if (ShiftPos != DriveShift.DriveShiftPosition.Neutral)
						{
							var acc = Acceleration;
							if (ShiftPos == DriveShift.DriveShiftPosition.Reverse) acc = -acc;
							tire.motorTorque = acc;
						}
					}
					if (tiregroup.steers)
					{
						tire.steerAngle = Rotation;
					}
					if (tiregroup.locks && ShiftPos == DriveShift.DriveShiftPosition.Park)
					{
						tire.brakeTorque = parkingBrakeForce;
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
			}
			if (ShiftPos != DriveShift.DriveShiftPosition.Park && dsp == DriveShift.DriveShiftPosition.Park)
			{
				SM.PlayGenericSound(AudioSet.VehicleStop, transform.position);
			}
			ShiftPos = dsp;	
		}

		public void setAcceleration(float acceleration)
		{
			if (debug) return;
			Acceleration = acceleration;
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
