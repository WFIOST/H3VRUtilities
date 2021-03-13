using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class VehicleMovementHandling : MonoBehaviour
	{
		public GameObject vehicle;
		private Rigidbody rb;
		public float force;
		public FVRViveHand hand;
		public Transform SitPos;

		void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void FixedUpdate()
		{
			if (hand.A_Button.active)
			{
				ApplyThrust();
			}
		}

		public void Update()
		{
			
		}


		public void ApplyThrust()
		{
			rb.AddForce(transform.forward * force);
		}
	}
}
