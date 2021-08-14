using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class VehicleDamagable : MonoBehaviour, IFVRDamageable
	{
		public float health;
		public float maxHealth;
		public float minHealth;
		public float explosionMult;
		public float piercingMult;
		public float cuttingMult;
		public float thermalMult;
		public float bluntMult;
		public float environmentMult;
		public Vehicle vehicle;
		public bool dead;
		private float prevhealth;

		public virtual void FixedUpdate()
		{
			if (health < 0)
			{
				if (!dead)
				{
					onDeath();
					dead = true;
				}
				whileDead();
			}
			else
			{
				if (dead)
				{
					onUndeath();
					dead = false;
				}
				whileUndead();
			}
			if (health < minHealth)
			{
				health = minHealth;
			}

			if (health != prevhealth)
				onHealthChange();
			prevhealth = health;
		}

		public virtual void onHealthChange()
		{

		}

		public bool HPLessThan(float num)
		{
			if (health < num)
			{
				return true;
			}
			return false;
		}

		public bool HPLessThanPercent(float num)
		{
			if(health < num * maxHealth)
			{
				return true;
			}
			return false;
		}

		public virtual void onDeath()
		{

		}

		public virtual void whileDead()
		{

		}

		public virtual void whileUndead()
		{

		}

		public virtual void onUndeath()
		{

		}

		public virtual void HealPercent(float percentHeal)
		{
			Heal(percentHeal * maxHealth);
			Debug.Log("percenthealing for " + percentHeal);
		}

		public virtual void Heal(float heal)
		{
			health += heal;
			Debug.Log("Healed for " + heal);
		}

		public virtual void Damage(Damage dmg)
		{

		}

		public float getDamage(Damage dmg)
		{
			float takenDamage = dmg.Dam_Blunt * bluntMult;
			takenDamage += dmg.Dam_Piercing * piercingMult;
			takenDamage += dmg.Dam_Cutting * cuttingMult;
			takenDamage += dmg.Dam_Thermal * thermalMult;
			if (dmg.Class == FistVR.Damage.DamageClass.Explosive) takenDamage *= explosionMult;
			return takenDamage;
		}
	}
}
