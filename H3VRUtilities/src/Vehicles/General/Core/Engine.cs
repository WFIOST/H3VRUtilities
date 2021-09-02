using System;
using System.Collections.Generic;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	public class Engine : MonoBehaviour
	{
		public Vehicle vehicle;
		[Header("Gears")]
		public List<float> gears;
		public int currentGear;
		private int _neutralGear;
		
		[Header("Power")]
		public float maxHorsePower;
		public float currentHorsePower;
		public float currentTorque;
		public float naturalFriction = 0.2f;
		
		[Header("RPM")]
		public float currentRPM;
		public float RPMincrease;
		public float minRPM;
		public float maxRPM;
		
		public void Start()
		{
			//set neutral gear
			for(int i = 0; i < gears.Count; i++)
				if (gears[i] == 0) {
					_neutralGear = i;
					break; }
		}

		public void ShiftGear(bool up)
		{
			if (up)
				currentGear++;
			else
				currentGear--;
		}

		public float GetEngineHorsepowerOutput(float rpm)
		{
			//get current horsepower output
			return maxHorsePower * Mathf.InverseLerp(0, maxRPM, rpm);
		}
		
		public float GetEngineTorque(float rpm)
		{
			var hp = GetEngineHorsepowerOutput(rpm);
			return 7127 * hp / rpm; //HP and RPM to Nm Torque
		}

		public float GetFriction(float torque, float rpm)
		{
			return torque * naturalFriction * rpm / maxRPM;
		}

		public float GetRPMchange(float torque, float friction)
		{
			float newRPM = vehicle.Throttle * RPMincrease * torque * 0.1f;
			newRPM -= friction * 10;
			return newRPM;
		}

		public void StartEngine()
		{
			currentRPM = minRPM * 0.15f;
		}

		public void StopEngine()
		{
			
		}

		public void FixedUpdate()
		{
			currentRPM += GetRPMchange(currentTorque, GetFriction(currentTorque, currentRPM));
			currentRPM = Mathf.Clamp(currentRPM, minRPM, maxRPM);
			
			currentTorque = GetEngineTorque(currentRPM);
			currentHorsePower = GetEngineHorsepowerOutput(currentRPM);
		}
	}
}