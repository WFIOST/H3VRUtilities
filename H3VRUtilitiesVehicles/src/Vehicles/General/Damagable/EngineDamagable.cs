using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	class EngineDamagable : VehicleDamagable
	{
		public GameObject particleSystemCentre;
		public GameObject explosionCentre;
		public float SmokeParticleHPThreshold;
		public GameObject particleSmokePrefab;
		public GameObject particleFirePrefab;
		public GameObject explosionPrefab;
		public GameObject fixedMesh;
		public GameObject damagedMesh;
		public GameObject destroyedMesh;
		public float explosionStrength = 200;
		
		private ParticleSystem particleSmoke;
		private ParticleSystem particleFire;

		private bool dontChangeMesh = false;
		
		public void Start()
		{
			if (fixedMesh == null || damagedMesh == null || destroyedMesh == null) dontChangeMesh = true;
			GameObject particleSmokeGO = Instantiate(particleSmokePrefab, particleSystemCentre.transform);
			particleSmoke = particleSmokeGO.GetComponent<ParticleSystem>();
			particleSmoke.Stop();
			GameObject particleFireGO = Instantiate(particleFirePrefab, particleSystemCentre.transform);
			particleFire = particleFireGO.GetComponent<ParticleSystem>();
			particleFire.Stop();
		}

		public override void onHealthChange()
		{
			if (HPLessThanPercent(SmokeParticleHPThreshold))
			{
				if (!particleSmoke.IsAlive())
				{
					particleSmoke.Play();
				}
			}
			else
			{
				particleSmoke.Stop();
			}

			if (!dontChangeMesh)
			{
				if (health < 0)
				{
					fixedMesh.SetActive(false);
					damagedMesh.SetActive(false);
					destroyedMesh.SetActive(true);
				}
				else if (HPLessThanPercent(SmokeParticleHPThreshold))
				{
					fixedMesh.SetActive(false);
					damagedMesh.SetActive(true);
					destroyedMesh.SetActive(false);
				}
				else
				{
					fixedMesh.SetActive(true);
					damagedMesh.SetActive(false);
					destroyedMesh.SetActive(false);
				}
			}
		}

		public override void onDeath()
		{
			particleFire.Play();
			if(explosionPrefab != null)
			{
				Instantiate(explosionPrefab, explosionCentre.transform.position, explosionCentre.transform.rotation);
				vehicle.GetRigidbody().AddForceAtPosition(new Vector3(0, explosionStrength, 0), transform.position);
			}
			vehicle.TurnOffEngine(true);
		}

		public override void whileDead()
		{
			vehicle.isForciblyOff = true;
		}

		public override void whileUndead()
		{

		}

		public override void Heal(float heal)
		{
			base.Heal(heal);
		}

		public override void HealPercent(float percentHeal)
		{
			base.HealPercent(percentHeal);
		}

		public override void onUndeath()
		{
			particleFire.Stop();
			vehicle.isForciblyOff = false;
		}

		public override void Damage(Damage dmg)
		{
			float takenDamage = getDamage(dmg);
			//Debug.Log("Engine taken " + takenDamage + " of type " + dmg.Class.ToString());
			health -= takenDamage;
		}
	}
}