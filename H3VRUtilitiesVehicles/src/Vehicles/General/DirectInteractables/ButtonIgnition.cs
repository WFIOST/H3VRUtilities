using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class ButtonIgnition : FVRInteractiveObject
	{
		public VehicleControl vehicle;
		public VehicleAudioSet audioSet;
		public float ignitionTime;
		private float m_it;
		public float failChance;
		public System.Random rand;

		public void Start()
		{
			rand = new System.Random();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			m_it = ignitionTime;
			if (!vehicle.isOn)
			{
				if (!vehicle.isForciblyOff)
				{
					SM.PlayGenericSound(audioSet.VehicleStart, transform.position);
				}
			}
			else
			{
				SM.PlayGenericSound(audioSet.VehicleStop, transform.position);
				m_it = 999999f; //takes 11 days to restart. if you manage to turn it off then turn it back on again you win
				vehicle.TurnOffEngine(false);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			m_it -= Time.fixedDeltaTime;
			if (m_it <= 0)
			{
				float fchance = rand.Next(0, 10000) / 100f;
				if (!(fchance <= failChance)) //fuck you, my code my logic
				{
					vehicle.TurnOnEngine(false);
				}
			}
		}
	}
}