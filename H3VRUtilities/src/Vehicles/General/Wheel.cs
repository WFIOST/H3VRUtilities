using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class Wheel : MonoBehaviour, IFVRDamageable
	{
		public Vehicle vehicle;

		private WheelCollider wheel;

		public float wheelMaxHP;
		public float wheelHP;
		private bool dead;

		private float wheelDefRadius;
		public float wheelPoppedRadius;
		public float wheelPoppedDampening;

		public AudioEvent PopSound;

		public void Start()
		{
			wheel = GetComponent<WheelCollider>();
			wheelDefRadius = wheel.radius;
		}

		public void FixedUpdate()
		{
			if (wheelHP < 0)
			{
				if (!dead)
				{
					float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
					float num2 = num / 343f;
					SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.GenericLongRange, PopSound, base.transform.position, PopSound.VolumeRange, PopSound.PitchRange, num2 + 0.04f);
					dead = true;
				}
				wheel.wheelDampingRate = wheelPoppedDampening;
				wheel.radius = Mathf.Lerp(wheelPoppedRadius, wheel.radius, 0.5f);
			}
		}

		public void Damage(Damage dam)
		{
			wheelHP -= dam.Dam_TotalKinetic;
		}
	}
}
