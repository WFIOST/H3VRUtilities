using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class Wheel : VehicleDamagable
	{
		private WheelCollider wheel;

		private float wheelDefRadius;
		private float wheelDefStiffnessForward;
		private float wheelDefStiffnessSideways;
		public float wheelPoppedRadius;
		public float wheelPoppedDampening;

		public AudioEvent PopSound;

		public void Start()
		{
			wheel = GetComponent<WheelCollider>();
			wheelDefRadius = wheel.radius;
			wheelDefStiffnessForward = wheel.forwardFriction.stiffness;
			wheelDefStiffnessSideways = wheel.sidewaysFriction.stiffness;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void onDeath()
		{
			base.onDeath();
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
			float num2 = num / 343f;
			SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.GenericLongRange, PopSound, base.transform.position, PopSound.VolumeRange, PopSound.PitchRange, num2 + 0.04f);
			dead = true;
		}
		public override void whileDead()
		{
			base.whileDead();
			wheel.wheelDampingRate = wheelPoppedDampening;
			wheel.radius = Mathf.Lerp(wheelPoppedRadius, wheel.radius, 0.9f);

			var fric = wheel.forwardFriction;
			fric.stiffness = wheelDefStiffnessForward * 1.75f;
			wheel.forwardFriction = fric;

			fric = wheel.sidewaysFriction;
			fric.stiffness = wheelDefStiffnessSideways * 0.5f;
			wheel.sidewaysFriction = fric;
		}

		public override void Damage(Damage dam)
		{
			float damageTaken = getDamage(dam);
			health -= damageTaken;
		}
	}
}
