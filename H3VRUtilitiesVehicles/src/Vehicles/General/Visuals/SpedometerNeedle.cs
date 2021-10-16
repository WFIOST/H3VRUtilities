using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class SpedometerNeedle : MonoBehaviour
	{
		public VehicleControl vehicle;
		public GameObject needle;
		public bool isImperial;
		public float maxSpeed;
		public Vector3 needleNoSpeed;
		public Vector3 needleMaxSpeed;

		public void Update()
		{
			var speed = Mathf.Abs(vehicle.speed);
			if (isImperial) speed *= 0.6213712f; //convert from kmh to mph
			var inlerp = Mathf.InverseLerp(0, maxSpeed, speed); //get lerp point between no and max speed;
			needle.transform.localEulerAngles = Vector3.Lerp(needleNoSpeed, needleMaxSpeed, inlerp);
		}
	}
}