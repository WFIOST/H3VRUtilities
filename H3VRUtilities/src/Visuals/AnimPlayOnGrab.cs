using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class AnimPlayOnGrab : MonoBehaviour
	{
		public Animation animation;
		public FVRPhysicalObject Object;
		public string AnimNameOnGrab;
		public string AnimNameOnLetGo;

		private bool isGrabbed;

		public void Update()
		{
			if (animation.isPlaying) return;
			if (Object.m_hand != null) //held
			{
				if (!isGrabbed) //was not held before
				{
					animation.Play(AnimNameOnGrab);
					isGrabbed = true;
				}
			}
			else //is not held
			{
				if (isGrabbed) //was held before
				{
					animation.Play(AnimNameOnLetGo);
					isGrabbed = false;
				}
			}
		}
	}
}
