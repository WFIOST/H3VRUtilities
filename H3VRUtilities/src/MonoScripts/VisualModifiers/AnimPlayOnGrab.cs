using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class AnimPlayOnGrab : MonoBehaviour
	{
		public Animation animation;
		[FormerlySerializedAs("Object")] public FVRPhysicalObject @object;
		[FormerlySerializedAs("AnimNameOnGrab")] public string animNameOnGrab;
		[FormerlySerializedAs("AnimNameOnLetGo")] public string animNameOnLetGo;

		private bool _isGrabbed;

		public void Update()
		{
			if (animation.isPlaying) return;
			if (@object.m_hand != null) //held
			{
				if (!_isGrabbed) //was not held before
				{
					animation.Play(animNameOnGrab);
					_isGrabbed = true;
				}
			}
			else //is not held
			{
				if (_isGrabbed) //was held before
				{
					animation.Play(animNameOnLetGo);
					_isGrabbed = false;
				}
			}
		}
	}
}
