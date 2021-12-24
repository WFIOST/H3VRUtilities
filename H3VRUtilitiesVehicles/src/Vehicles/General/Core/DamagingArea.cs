using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles.Core
{
	public class DamagingArea : MonoBehaviour
	{
		public VehicleControl vehicle;
		public float damageMult = 15f;
		public float sharpyness = 50f;
		private void OnCollisionEnter(Collision other)
		{
			if (vehicle.speed < 12f) return;
			IFVRDamageable component = other.gameObject.GetComponent<IFVRDamageable>();
			if (component != null)
			{
				Damage damage = new Damage();
				damage.Class = Damage.DamageClass.Environment;
				damage.Dam_Piercing = sharpyness;
				damage.Dam_Blunt = vehicle.speed * damageMult;
				damage.Dam_TotalKinetic = damage.Dam_Blunt + damage.Dam_Piercing;
				damage.point = other.contacts[0].point;
				damage.hitNormal = other.contacts[0].normal;
				damage.strikeDir = transform.forward;
				component.Damage(damage);
			}
		}
	}
}