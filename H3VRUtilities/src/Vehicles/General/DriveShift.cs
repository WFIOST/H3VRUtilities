using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.Vehicles
{
	public class DriveShift : FVRInteractiveObject
	{
		public Vehicle vehicle;

		[FormerlySerializedAs("RotPositions")] 
		public List<float> rotPositions;
		
		[FormerlySerializedAs("DriveShiftPos")] 
		public List<DriveShiftPosition> driveShiftPosition;

		[FormerlySerializedAs("shiftpos")] 
		public Text shiftPosition;

		public enum DriveShiftPosition
		{
			Park,
			Reverse,
			Neutral,
			Drive
		}
		public int currentPosition;

		void Start()
		{
			vehicle.SetDriveShift(driveShiftPosition[currentPosition]);
			transform.localEulerAngles = new Vector3(rotPositions[currentPosition], 0, 0);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			shiftPosition.text = driveShiftPosition[currentPosition].ToString();
			base.UpdateInteraction(hand);
			//drive shift, looked towards hand rotation
			transform.LookAt(hand.transform, this.transform.up);

			if (transform.localEulerAngles.x > 300) {
				transform.localEulerAngles = new Vector3(rotPositions[currentPosition], 0, 0);
				return;
			}

			//if its not top
			if (currentPosition != driveShiftPosition.Count - 1)
			{
				//if it's closer to the drive shift one up than the current
				if (transform.localEulerAngles.x > rotPositions[currentPosition + 1])
				{
					currentPosition++;
					vehicle.SetDriveShift(driveShiftPosition[currentPosition]);
					SM.PlayGenericSound(vehicle.audioSet.handbrakeDown, transform.position);
				}
			}


			//if its not last place
			if (currentPosition != 0)
			{
				//if it's closer to the drive shfit one below than the one current
				if (transform.localEulerAngles.x < rotPositions[currentPosition - 1])
				{
					currentPosition--;
					vehicle.SetDriveShift(driveShiftPosition[currentPosition]);
					SM.PlayGenericSound(vehicle.audioSet.handbrakeUp, transform.position);
				}
			}
			transform.localEulerAngles = new Vector3(rotPositions[currentPosition], 0, 0);
		}
	}
}
