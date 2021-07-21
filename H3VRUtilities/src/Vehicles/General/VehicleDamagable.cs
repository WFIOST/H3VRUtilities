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
		private float _prevhealth;

		public virtual void FixedUpdate()
		{
			if (health < 0)
			{
				if (!dead)
				{
					ONDeath();
					dead = true;
				}
				WhileDead();
			}
			else
			{
				if (dead)
				{
					ONUndeath();
					dead = false;
				}
				WhileUndead();
			}
			if (health < minHealth)
			{
				health = minHealth;
			}

			if (health != _prevhealth)
				ONHealthChange();
			_prevhealth = health;
		}

		public virtual void ONHealthChange()
		{

		}

		public bool HpLessThan(float num)
		{
			if (health < num)
			{
				return true;
			}
			return false;
		}

		public bool HpLessThanPercent(float num)
		{
			if(health < num * maxHealth)
			{
				return true;
			}
			return false;
		}

		public virtual void ONDeath()
		{

		}

		public virtual void WhileDead()
		{

		}

		public virtual void WhileUndead()
		{

		}

		public virtual void ONUndeath()
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

		public float GETDamage(Damage dmg)
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
