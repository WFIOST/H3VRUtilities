using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	public class Wheel : VehicleDamagable
	{
		private Rigidbody _rigidbody;
		public Axle wheelAxle;
		public GameObject model;
		public AudioEvent PopSound;

		public float wheelRadius;
		public bool invertRotation;
		public float defaultRotation;
		public float minimumRotation;
		public float maxRotation;

		public float currentRollingResistance;

		public float currentSuspensionTravel;
		public bool isOnGround;
		public float currentRotation;

		public void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public float CalcWheelRotation()
		{
			var rot = wheelAxle.vehicle.Rotation; //get rotation
			float lerp;
			//wrap rot to minimum to maximum rotation (probably a nicer way, but this works)
			if (rot >= 0) lerp = Mathf.Lerp(defaultRotation, maxRotation, rot); //handler if rot is above 0
			else lerp = Mathf.Lerp(defaultRotation, minimumRotation, -rot); //handler if rot is below 0
			return lerp;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			RaycastHit hit;
			currentRotation = CalcWheelRotation();
			transform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);
			isOnGround =
				Physics.Raycast(
					transform.position,
					-gameObject.transform.up,
					out hit,
					wheelAxle.suspensionDist + wheelRadius);
			//don't move if it's not on the ground, lol
			if (isOnGround)
			{
				ApplySuspensionForce(hit);
				ApplyForwardForce();
				ApplyBrakeForce();
			}
		}

		private void ApplyBrakeForce()
		{
			//TODO: if brakes are too strong it can actually cause the car to go backwards, pls prevent this
			float torque = GetResistanceTorque();
			_rigidbody.AddForce(torque * transform.forward);
			Debug.DrawRay(transform.position, torque * transform.forward / 1000, Color.red);
		}
		private void ApplyForwardForce()
		{
			if (wheelAxle.affectedByAcceleration)
			{
				_rigidbody.AddForce(-transform.forward * wheelAxle.forwardThrust);
				Debug.DrawRay(transform.position, (-transform.forward * wheelAxle.forwardThrust / 1000f), Color.green);
			}
		}
		private void ApplySuspensionForce(RaycastHit hit)
		{
			//get the suspension strength
			var suspensionStrength = GetSuspensionForce(GetCompression());
			var totalSuspensionStrengthfloat = wheelAxle.suspensionStrength;
			totalSuspensionStrengthfloat += suspensionStrength;
			Vector3 totalSuspensionStrength = (hit.normal * totalSuspensionStrengthfloat);
			_rigidbody.AddForce(totalSuspensionStrength);
			Debug.DrawRay(transform.position, totalSuspensionStrength / 1000, Color.gray);
		}
		private float GetCompression()
		{
			var com = wheelAxle.suspensionDist - currentSuspensionTravel;
			com = Mathf.Clamp(com, 0f, wheelAxle.suspensionDist);
			return com;
		}
		private float GetSuspensionForce(float compressionApplied)
		{
			var sus = wheelAxle.suspensionStrength * compressionApplied + wheelAxle.antiRollBarRate;
			return sus; //sus
		}
		private float GetResistanceTorque()
		{
			float torque = 0;
			if (wheelAxle.affectedByBrake)
				torque = wheelAxle.brakeTorque * wheelAxle.vehicle.brakingForce;

			if (wheelAxle.vehicle.isHandbrakeOn && wheelAxle.affectedByHandbrake)
				torque += wheelAxle.handbrakeTorque;

			torque += currentRollingResistance;
			return torque;
		}


		public override void onDeath()
		{

		}
		public override void whileDead()
		{
			//dead bit
		}
		public override void Damage(Damage dam)
		{
			float damageTaken = getDamage(dam);
			health -= damageTaken;
		}
	}
}
