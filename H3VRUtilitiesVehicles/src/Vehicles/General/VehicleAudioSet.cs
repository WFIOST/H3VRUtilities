using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	[CreateAssetMenu(fileName = "New Vehicle Audio Set", menuName = "Vehicles/AudioSet", order = 0)]
	public class VehicleAudioSet : ScriptableObject
	{
		private static AudioEvent defaultAE = new AudioEvent()
		{
			PitchRange = new Vector2(0.98f, 1.04f),
			VolumeRange = new Vector2(0.98f, 1.04f),
			ClipLengthRange = new Vector2(1,1)
		};
		public AudioEvent VehicleStart;
		public AudioEvent VehicleIdle;
		public AudioEvent VehicleStop;

		public AudioEvent HandbrakeUp;
		public AudioEvent HandbrakeDown;

		public AudioEvent ShiftDownGear;
		public AudioEvent RevLoop;
		public AudioEvent ShiftUpGear;

		public AudioEvent Brake;
		public AudioEvent BrakeLong;

		public AudioEvent PedalSwitchSound;
	}
}
