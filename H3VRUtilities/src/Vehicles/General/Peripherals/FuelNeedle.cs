using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class FuelNeedle : MonoBehaviour
	{
		public FuelTank tank;
		public GameObject needle;
		public Vector3 needleNoFuel;
		public Vector3 needleMaxFuel;

		public void Update()
		{
			var fuel = tank.currentFuel;
			var inlerp = Mathf.InverseLerp(0, tank.maxFuel, fuel); //get lerp point between no and max speed;
			needle.transform.localEulerAngles = Vector3.Lerp(needleNoFuel, needleMaxFuel, inlerp);
		}
	}
}