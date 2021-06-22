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
		public float acceleration;
		public float rotSpeed;
		public FVRViveHand hand;
		public Transform SitPos;

		public List<Wheel> PoweredWheels;
		//public List<Wheel> RotatingWheels;

		[HideInInspector]
		public DriveShift.DriveShiftPosition ShiftPos;

		void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void Update()
		{
			foreach (var wheel in PoweredWheels)
			{
				wheel.ShiftPos = ShiftPos;
			}


			if (hand == null) { return; }
			hand.MovementManager.transform.position = SitPos.transform.position;
			//hand.MovementManager.transform.rotation = SitPos.transform.rotation;
		}

		public void FixedUpdate()
		{

		}

		public void Rotate(float rotation)
		{
			//rb.AddTorque(0, rotation, 0);

			foreach (var wheel in PoweredWheels)
			{
				wheel.rotAmt = rotation;
			}
		}

		public void Accelerate(float accelerationPercent)
		{
			foreach (var wheel in PoweredWheels)
			{
				wheel.applyforce = accelerationPercent * acceleration;
			}
		}
	}
}
