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
					if (TestTouchpadDir(grenade.m_hand, dir, true))
					{
						foreach (var ring in grenade.m_rings)
						{
							ring.PopOutRoutine();
						}
						if(kickOutLever) grenade.ReleaseLever();
					}
				}
			}
		}

		public static bool TestTouchpadDir(FVRViveHand hand, Vector2 dir, bool triggerpressed = false)
		{
			if (dir == Vector2.zero && hand.Input.TriggerDown && !triggerpressed) return true;
			if (dir == Vector2.zero && hand.Input.TriggerPressed && triggerpressed) return true;
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown &&
			    hand.Input.TouchpadAxes.magnitude > 0.2f) return true;
			return false;
		}
	}
}