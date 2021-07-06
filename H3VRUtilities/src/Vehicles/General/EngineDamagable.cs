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
		public float SmokeParticleHPThreshold;
		public GameObject particleSmokePrefab;
		public GameObject particleFirePrefab;
		public GameObject explosionPrefab;
		public GameObject fixedMesh;
		public GameObject damagedMesh;
		public GameObject destroyedMesh;

		private ParticleSystem particleSmoke;
		private ParticleSystem particleFire;

		public void Start()
		{
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


			if (health < 0)
			{
				fixedMesh.SetActive(false);
				damagedMesh.SetActive(false);
				destroyedMesh.SetActive(true);
			}
			else if(HPLessThanPercent(SmokeParticleHPThreshold))
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

		public override void onDeath()
		{
			particleFire.Play();
			if(explosionPrefab != null)
			{
				Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
			}
			vehicle.ToggleEngine(false);
		}

		public override void whileDead()
		{

		}

		public override void whileUndead()
		{

		}

		public override void onUndeath()
		{
			particleFire.Stop();
			vehicle.ToggleEngine(true);
		}

		public override void Damage(Damage dmg)
		{
			float takenDamage = getDamage(dmg);
			Debug.Log("Engine taken " + takenDamage + " of type " + dmg.Class.ToString());
			health -= takenDamage;
		}
	}
}
