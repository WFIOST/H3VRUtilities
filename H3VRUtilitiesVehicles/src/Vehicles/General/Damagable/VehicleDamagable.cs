using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	[System.Serializable]
	public class VehicleDamagableMult
	{
		public float projectileMult = 1;
		public float meleeMult = 1;
		public float explosionMult = 1;
		public float piercingMult = 1;
		public float cuttingMult = 1;
		public float thermalMult = 1;
		public float bluntMult = 1;
		public float totalKineticMult = 1;
	}
	public class VehicleDamagable : MonoBehaviour, IFVRDamageable
	{
		public float health;
		public float maxHealth;
		public float minHealth;
		public VehicleDamagableMult dmgMult;
		public VehicleControl vehicle;
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
			health -= getDamage(dmg);
		}

		public float getDamage(Damage dmg)
		{
			float strikeAngle = Vector3.Angle(dmg.strikeDir, -dmg.hitNormal);
			float strikeAngleMult = Mathf.Clamp((1f - strikeAngle / 90f) * 1.5f, 0.4f, 1.5f);
			dmg.Dam_Blunt *= strikeAngleMult;
			dmg.Dam_Cutting *= strikeAngleMult;
			dmg.Dam_Piercing *= strikeAngleMult;
			dmg.Dam_TotalKinetic *= strikeAngleMult;

			if (dmg.Class == FistVR.Damage.DamageClass.Projectile)
			{
				dmg.Dam_Blunt *= dmgMult.projectileMult;
				dmg.Dam_Cutting *= dmgMult.projectileMult;
				dmg.Dam_Piercing *= dmgMult.projectileMult;
				dmg.Dam_TotalKinetic *= dmgMult.projectileMult;
			}
			else if (dmg.Class == FistVR.Damage.DamageClass.Melee)
			{
				dmg.Dam_Blunt *= dmgMult.meleeMult;
				dmg.Dam_Cutting *= dmgMult.meleeMult;
				dmg.Dam_Piercing *= dmgMult.meleeMult;
				dmg.Dam_TotalKinetic *= dmgMult.meleeMult;
			}
			else if (dmg.Class == FistVR.Damage.DamageClass.Explosive)
			{
				dmg.Dam_Blunt *= dmgMult.explosionMult;
				dmg.Dam_Cutting *= dmgMult.explosionMult;
				dmg.Dam_Piercing *= dmgMult.explosionMult;
				dmg.Dam_TotalKinetic *= dmgMult.explosionMult;
			}
			dmg.Dam_Blunt *= dmgMult.bluntMult;
			dmg.Dam_Cutting *= dmgMult.cuttingMult;
			dmg.Dam_TotalKinetic *= dmgMult.totalKineticMult;
			dmg.Dam_Piercing *= dmgMult.piercingMult;
			dmg.Dam_Thermal *= dmgMult.thermalMult;

			float takenDamage = 0;
			takenDamage += dmg.Dam_Blunt;
			takenDamage += dmg.Dam_Cutting;
			takenDamage += dmg.Dam_Piercing;
			takenDamage += dmg.Dam_Piercing;
			takenDamage += dmg.Dam_Thermal;
			
			Debug.Log("Damage taken: " + takenDamage);
			
			
			return takenDamage;
		}
	}
}