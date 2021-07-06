using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	// Token: 0x02000977 RID: 2423
	public class VehicleRepairTool : MonoBehaviour
	{
		public float percentHeal;
		// Token: 0x060033BF RID: 13247 RVA: 0x0016830C File Offset: 0x0016670C
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
				component.HealPercent(percentHeal);
			}
		}
	}
}