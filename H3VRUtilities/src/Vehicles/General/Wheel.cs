using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.Vehicles
{
	public class Wheel : VehicleDamagable
	{
		private WheelCollider _wheel;

		private float _wheelDefRadius;
		private float _wheelDefStiffnessForward;
		private float _wheelDefStiffnessSideways;
		public float wheelPoppedRadius;
		public float wheelPoppedDampening;

		[FormerlySerializedAs("PopSound")] public AudioEvent popSound;

		public void Start()
		{
			_wheel = GetComponent<WheelCollider>();
			_wheelDefRadius = _wheel.radius;
			_wheelDefStiffnessForward = _wheel.forwardFriction.stiffness;
			_wheelDefStiffnessSideways = _wheel.sidewaysFriction.stiffness;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void ONDeath()
		{
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
			float num2 = num / 343f;
			SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.GenericLongRange, popSound, base.transform.position, popSound.VolumeRange, popSound.PitchRange, num2 + 0.04f);
			dead = true;
		}

		public override void WhileDead()
		{
			_wheel.wheelDampingRate = wheelPoppedDampening;
			_wheel.radius = Mathf.Lerp(wheelPoppedRadius, _wheel.radius, 0.9f);

			WheelFrictionCurve fric = _wheel.forwardFriction;
			fric.stiffness = _wheelDefStiffnessForward * 1.75f;
			_wheel.forwardFriction = fric;

			fric = _wheel.sidewaysFriction;
			fric.stiffness = _wheelDefStiffnessSideways * 0.5f;
			_wheel.sidewaysFriction = fric;
		}

		public override void Damage(Damage dam)
		{
			float damageTaken = GETDamage(dam);
			health -= damageTaken;
		}
	}
}
