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

		public float currentRollingResistance;

		public float currentSuspensionTravel;
		public bool isOnGround;

		public void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			RaycastHit hit;
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
			Vector3 right = transform.TransformDirection(transform.localRotation * Vector3.right);
			float torque = GetResistanceTorque();
			_rigidbody.AddForce(torque * -right);
			Debug.DrawRay(transform.position, torque * -right / 1000, Color.red);
		}
		private void ApplyForwardForce()
		{
			if (wheelAxle.affectedByAcceleration)
			{
				Vector3 right = transform.TransformDirection(transform.localRotation * Vector3.right);
				_rigidbody.AddForce(right * wheelAxle.forwardThrust);
				Debug.DrawRay(transform.position, (right * wheelAxle.forwardThrust / 1000f), Color.green);
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
			//TODO: REPLACE ACCELERATION WITH BRAKE
			float torque = 0;
			if (wheelAxle.vehicle.Acceleration < 0 && wheelAxle.affectedByBrake)
				torque = wheelAxle.brakeTorque * Mathf.Abs(wheelAxle.vehicle.Acceleration);

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
