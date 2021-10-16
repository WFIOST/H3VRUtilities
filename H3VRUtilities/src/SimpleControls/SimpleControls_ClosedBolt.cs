using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	public class SimpleControls_ClosedBolt : MonoBehaviour
	{
		[HideInInspector]
		public ClosedBoltWeapon cbw;
		public void Start()
		{
			cbw = GetComponent<ClosedBoltWeapon>();
			if (cbw == null)
			{
				Debug.Log("SimpleControls_ClosedBolt_BoltRelease can't find the closed bolt weapon!");
				Destroy(this);
			}
		}
	}
}