using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace H3VRUtils.Vehicles
{
	[System.Serializable]
	public class DriveShiftNode
	{
		public Vector3 localposition;
		public Vector3 rotation;
		public int left = -1;
		public int up = -1;
		public int right = -1;
		public int down = -1;
		public int gear = 0;
	}
	public class DriveShift : FVRInteractiveObject
	{
		public VehicleControl vehicle;
		public Text gearText;
		public int currentNode;
		public List<DriveShiftNode> driveShiftNodes;
		public VehicleAudioSet audioSet;
		
		public void Update()
		{
			if (vehicle.carSetting.automaticGear)
			{
				if (vehicle.currentGear > 0 && vehicle.speed > 1)
				{
					gearText.text = vehicle.currentGear.ToString();
				}
				else if (vehicle.speed > 1)
				{
					gearText.text = "R";
				}
				else
				{
					gearText.text = "N";
				}
			}
			else
			{

				if (vehicle.NeutralGear)
				{
					gearText.text = "N";
				}
				else
				{
					if (vehicle.currentGear != 0)
					{
						gearText.text = vehicle.currentGear.ToString();
					}
					else
					{
						gearText.text = "R";
					}
				}

			}
		}

		void Start()
		{
			vehicle.ShiftTo(driveShiftNodes[currentNode].gear);
			transform.localPosition = driveShiftNodes[currentNode].localposition;
			transform.localEulerAngles = driveShiftNodes[currentNode].rotation;
		}
		
		public override void UpdateInteraction(FVRViveHand hand)
		{
			bool changed = false;
			if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				if (driveShiftNodes[currentNode].left != -1)
				{
					changed = true;
					currentNode = driveShiftNodes[currentNode].left;
				}
			}
			if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				if (driveShiftNodes[currentNode].up != -1)
				{
					changed = true;
					currentNode = driveShiftNodes[currentNode].up;
				}
			}
			if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				if (driveShiftNodes[currentNode].right != -1)
				{
					changed = true;
					currentNode = driveShiftNodes[currentNode].right;
				}
			}
			if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				if (driveShiftNodes[currentNode].down != -1)
				{
					changed = true;
					currentNode = driveShiftNodes[currentNode].down;
				}
			}

			if (changed)
			{
				vehicle.ShiftTo(driveShiftNodes[currentNode].gear);
				transform.localPosition = driveShiftNodes[currentNode].localposition;
				transform.localEulerAngles = driveShiftNodes[currentNode].rotation;
			}
		}
	}
}
