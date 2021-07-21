using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.Vehicles
{
	[CreateAssetMenu(fileName = "New Vehicle Audio Set", menuName = "Vehicles/AudioSet", order = 0)]
	public class VehicleAudioSet : ScriptableObject
	{
		private static AudioEvent _defaultAe = new AudioEvent()
		{
			PitchRange = new Vector2(0.98f, 1.04f),
			VolumeRange = new Vector2(0.98f, 1.04f),
			ClipLengthRange = new Vector2(1,1)
		};
		[FormerlySerializedAs("VehicleStart")] public AudioEvent vehicleStart;
		[FormerlySerializedAs("VehicleIdle")] public AudioEvent vehicleIdle;
		[FormerlySerializedAs("VehicleStop")] public AudioEvent vehicleStop;

		[FormerlySerializedAs("HandbrakeUp")] public AudioEvent handbrakeUp;
		[FormerlySerializedAs("HandbrakeDown")] public AudioEvent handbrakeDown;

		[FormerlySerializedAs("ShiftDownGear")] public AudioEvent shiftDownGear;
		[FormerlySerializedAs("RevLoop")] public AudioEvent revLoop;
		[FormerlySerializedAs("ShiftUpGear")] public AudioEvent shiftUpGear;

		[FormerlySerializedAs("Brake")] public AudioEvent brake;
		[FormerlySerializedAs("BrakeLong")] public AudioEvent brakeLong;

		[FormerlySerializedAs("PedalSwitchSound")] public AudioEvent pedalSwitchSound;
	}
}
