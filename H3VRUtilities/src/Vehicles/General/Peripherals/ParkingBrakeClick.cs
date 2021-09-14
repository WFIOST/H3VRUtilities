using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class ParkingBrakeClick : FVRInteractiveObject
	{
		public VehicleControl vehicle;
		public Vector3 positionOff;
		public Vector3 positionOn;
		public Vector3 rotationOff;
		public Vector3 rotationOn;
		public bool isOn;
		public VehicleAudioSet audioSet;
		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			isOn = !isOn;
			if (isOn)
			{
				SM.PlayGenericSound(audioSet.HandbrakeUp, transform.position);
				transform.localPosition = positionOn;
				transform.localEulerAngles = rotationOn;
				vehicle.brake = true;
			}
			else
			{
				SM.PlayGenericSound(audioSet.HandbrakeDown, transform.position);
				transform.localPosition = positionOff;
				transform.localEulerAngles = rotationOff;
				vehicle.brake = false;
			}
		}
	}
}