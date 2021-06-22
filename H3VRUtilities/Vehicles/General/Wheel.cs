using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class Wheel : MonoBehaviour
	{
		public Vehicle vehicle;

		private Rigidbody rb;
		private HingeJoint hj;
		private HingeJoint hjp;

		[HideInInspector]
		public float applyforce;
		[HideInInspector]
		public float rotAmt;
		[HideInInspector]
		public DriveShift.DriveShiftPosition ShiftPos;

		public bool isBreaking;

		public bool doesRotate;

		public void Start()
		{
			rb = GetComponent<Rigidbody>();
			hj = GetComponent<HingeJoint>();
		}

		public void FixedUpdate()
		{
			var speed = vehicle.maxSpeed;
			var force = applyforce;
			if (ShiftPos == DriveShift.DriveShiftPosition.Neutral) force = 0;
			if (ShiftPos == DriveShift.DriveShiftPosition.Reverse) speed = -speed;
			if (ShiftPos == DriveShift.DriveShiftPosition.Park)
			{
				hj.motor = new JointMotor()
				{
					targetVelocity = 0,
					force = 99999999,
					freeSpin = false
				};
				if (doesRotate)
					transform.parent.transform.localEulerAngles = new Vector3(transform.parent.transform.localEulerAngles.x, rotAmt, transform.parent.transform.localEulerAngles.z);
				return;
			}
			

			if (applyforce > 0)
			{
				hj.motor = new JointMotor()
				{
					targetVelocity = speed,
					force = force,
					freeSpin = true
				};
			}
			else
			{
				hj.motor = new JointMotor()
				{
					targetVelocity = 0,
					force = 0,
					freeSpin = true
				};
			}
			if (doesRotate)
				transform.parent.transform.localEulerAngles = new Vector3(transform.parent.transform.localEulerAngles.x, rotAmt, transform.parent.transform.localEulerAngles.z);
		}
	}
}
