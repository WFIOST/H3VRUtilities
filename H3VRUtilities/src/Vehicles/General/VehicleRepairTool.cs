using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	public class VehicleRepairTool : MonoBehaviour
	{
		public float percentHeal;
		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log("Hit object with a relative velocity magnitude of " + collision.relativeVelocity.magnitude);
			if (collision.relativeVelocity.magnitude < 2f)
			{
				return;
			}
			VehicleDamagable component = collision.gameObject.GetComponent<VehicleDamagable>();
			if (component != null)
			{
				Debug.Log("a real vehicledamagable");
				component.HealPercent(percentHeal);
			}
			else
			{
				Debug.Log("not a real vehicledamagable");
			}
		}
	}
}