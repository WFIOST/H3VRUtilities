using UnityEngine;
using FistVR;
using H3VRUtils.MonoScripts.VisualModifiers;
using JetBrains.Annotations;

namespace H3VRUtils.FVRInteractiveObjects
{
	public class GrenadeOneHandArm : MonoBehaviour
	{
		public PinnedGrenade grenade;
		//public ManipulateObject MOoverride;
		[Tooltip("Trigger is a terrible idea for this. Please don't use trigger. I mean, i can't stop you, but i dont recommend it.")]
		public H3VRUtilsMagRelease.TouchpadDirType dirType;
		private Vector2 dir;
		public bool kickOutLever;

		public void Start()
		{
			dir = H3VRUtilsMagRelease.TouchpadDirTypeToVector2(dirType);
		}
		
		public void Update()
		{
			if (grenade.m_hand != null)
			{
				if (TestTouchpadDir(grenade.m_hand, dir))
				{
					foreach (var ring in grenade.m_rings)
					{
						ring.PopOutRoutine();
					}
					if (kickOutLever) grenade.ReleaseLever();
				}
			}
		}

		public static bool TestTouchpadDir(FVRViveHand hand, Vector2 dir, bool triggerpressed = false)
		{
			//trigger checks- check if has just been pressed
			if (dir == Vector2.zero && hand.Input.TriggerDown && !triggerpressed) return true;
			//trigger checks- check if is currently being pressed
			if (dir == Vector2.zero && hand.Input.TriggerPressed && triggerpressed) return true;
			//touchpad checks- just check the touchpad lol
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown &&
			    hand.Input.TouchpadAxes.magnitude > 0.2f) return true;
			return false;
		}
	}
}