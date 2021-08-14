using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class Axle : MonoBehaviour
	{
		public Vehicle vehicle;
		[Header("Wheels")]
		public Wheel leftWheel;
		public Wheel rightWheel;
		public bool affectedByAcceleration;
		
		[Header("Suspension and Anti Roll")]
		public float suspensionDist = 0.2f;
		public float suspensionStrength = 300f;
		public float antiRollBarRate = 300f;

		[Header("Brakes")]
		public bool affectedByHandbrake;
		public float handbrakeTorque;
		public bool affectedByBrake;
		public float brakeTorque;

		[Header("Moving Values (no touchy)")] 
		public float forwardThrust;

		public void Update()
		{
			forwardThrust = vehicle.transmissionTorque * vehicle.torqueToImpulse;
		}
	}
}