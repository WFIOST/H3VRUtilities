using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils.Vehicles
{
	public class EngineDamagable : VehicleDamagable
	{
		public GameObject particleSystemCentre;
		[FormerlySerializedAs("SmokeParticleHPThreshold")] public float smokeParticleHpThreshold;
		public GameObject particleSmokePrefab;
		public GameObject particleFirePrefab;
		public GameObject explosionPrefab;
		public GameObject fixedMesh;
		public GameObject damagedMesh;
		public GameObject destroyedMesh;

		private ParticleSystem _particleSmoke;
		private ParticleSystem _particleFire;

		public void Start()
		{
			GameObject particleSmokeGO = Instantiate(particleSmokePrefab, particleSystemCentre.transform);
			_particleSmoke = particleSmokeGO.GetComponent<ParticleSystem>();
			_particleSmoke.Stop();
			GameObject particleFireGO = Instantiate(particleFirePrefab, particleSystemCentre.transform);
			_particleFire = particleFireGO.GetComponent<ParticleSystem>();
			_particleFire.Stop();
		}

		public override void ONHealthChange()
		{
			if (HpLessThanPercent(smokeParticleHpThreshold))
			{
				if (!_particleSmoke.IsAlive())
				{
					_particleSmoke.Play();
				}
			}
			else
			{
				_particleSmoke.Stop();
			}


			if (health < 0)
			{
				fixedMesh.SetActive(false);
				damagedMesh.SetActive(false);
				destroyedMesh.SetActive(true);
			}
			else if(HpLessThanPercent(smokeParticleHpThreshold))
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

		public override void ONDeath()
		{
			_particleFire.Play();
			if(explosionPrefab != null)
			{
				Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
			}
			vehicle.ToggleEngine(false);
		}

		public override void WhileDead()
		{

		}

		public override void WhileUndead()
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

		public override void ONUndeath()
		{
			_particleFire.Stop();
			vehicle.ToggleEngine(true);
		}

		public override void Damage(Damage dmg)
		{
			float takenDamage = GETDamage(dmg);
			Debug.Log("Engine taken " + takenDamage + " of type " + dmg.Class.ToString());
			health -= takenDamage;
		}
	}
}
