using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class DriveShift : FVRInteractiveObject
	{
		public Vehicle vehicle;

		public List<float> RotPositions;
		public List<DriveShiftPosition> DriveShiftPos;

		public Text shiftpos;

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
			vehicle.setDriveShift(DriveShiftPos[currentPosition]);
			transform.localEulerAngles = new Vector3(RotPositions[currentPosition], 0, 0);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			shiftpos.text = DriveShiftPos[currentPosition].ToString();
			base.UpdateInteraction(hand);
			//drive shift, looked towards hand rotation
			transform.LookAt(hand.transform, this.transform.up);

			if (transform.localEulerAngles.x > 300) {
				transform.localEulerAngles = new Vector3(RotPositions[currentPosition], 0, 0);
				return;
			}

			//if its not top
			if (currentPosition != DriveShiftPos.Count - 1)
			{
				//if it's closer to the drive shift one up than the current
				if (transform.localEulerAngles.x > RotPositions[currentPosition + 1])
				{
					currentPosition++;
					vehicle.setDriveShift(DriveShiftPos[currentPosition]);
					SM.PlayGenericSound(vehicle.AudioSet.HandbrakeDown, transform.position);
				}
			}


			//if its not last place
			if (currentPosition != 0)
			{
				//if it's closer to the drive shfit one below than the one current
				if (transform.localEulerAngles.x < RotPositions[currentPosition - 1])
				{
					currentPosition--;
					vehicle.setDriveShift(DriveShiftPos[currentPosition]);
					SM.PlayGenericSound(vehicle.AudioSet.HandbrakeUp, transform.position);
				}
			}
			transform.localEulerAngles = new Vector3(RotPositions[currentPosition], 0, 0);
		}
	}
}
