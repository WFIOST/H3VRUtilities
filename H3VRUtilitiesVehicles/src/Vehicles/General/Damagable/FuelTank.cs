using System;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class FuelTank : VehicleDamagable
	{
		public float currentFuel;
		public float maxFuel;
		public float fuelUsagePer1000Rpm = 0.01f;
		public float leakMult;
		public GameObject explosionEffect;
		public bool BlowsOnDeath;
		public AudioSource leakSound;

		void FixedUpdate()
		{
			base.FixedUpdate();
			float krpm = vehicle.motorRPM / 1000;
			float fuelUsage = krpm * (fuelUsagePer1000Rpm / 3000);
			currentFuel -= fuelUsage;
			//TODO: Fix math
			float inlerp = Mathf.InverseLerp(maxHealth * 0.5f, 0, health);
			currentFuel -= (inlerp * leakMult / 50);
			if (leakSound != null) leakSound.volume = inlerp;
			if (currentFuel < 0) currentFuel = 0;
			if (currentFuel == 0) vehicle.TurnOffEngine(false);
		}

		public override void whileDead()
		{
			base.whileDead();
			
		}

		public override void onDeath()
		{
			base.onDeath();
			if (BlowsOnDeath)
			{
				Instantiate(explosionEffect, transform.position, transform.rotation);
				currentFuel = 0;
			}
		}

		public float AddFuel(float fuelAdded)
		{
			currentFuel += fuelAdded;
			float returnAmt = maxFuel - currentFuel;
			if (returnAmt <= 0) return 0;
			return returnAmt;
		}
	}
}