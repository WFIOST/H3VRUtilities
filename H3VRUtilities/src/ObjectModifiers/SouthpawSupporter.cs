using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils.QOL
{
	public class SouthpawSupporter : MonoBehaviour
	{
		//i don't believe southpaws deserve rights but apparently i've been asked to make this too many times
		[Tooltip("If true, automatically generates LeftHand and LeftHand_Touch.m")]
		public bool AutoSwapRot = true;
		public Transform LeftHand;
		public Transform LeftHand_Touch;
		private Transform RightHand;
		private Transform RightHand_Touch;
		private FVRPhysicalObject _physicalObject;

		public void Start()
		{
			_physicalObject = GetComponent<FVRPhysicalObject>();
			RightHand = _physicalObject.PoseOverride;
			RightHand_Touch = _physicalObject.PoseOverride_Touch;
			if (AutoSwapRot)
			{
				LeftHand = setupEmpty(_physicalObject.PoseOverride).transform;
				LeftHand_Touch = setupEmpty(_physicalObject.PoseOverride_Touch).transform;
				var pos = LeftHand.position;
				var rot = LeftHand.localEulerAngles;
				pos.x = -pos.x;
				LeftHand.localPosition = pos;
				rot.y = -rot.y; rot.z = -rot.z;
				LeftHand.localEulerAngles = rot;
				
				pos = LeftHand_Touch.position;
				rot = LeftHand_Touch.localEulerAngles;
				pos.x = -pos.x;
				LeftHand_Touch.localPosition = pos;
				rot.y = -rot.y; rot.z = -rot.z;
				LeftHand_Touch.localEulerAngles = rot;
			}
		}

		public static GameObject setupEmpty(Transform tf)
		{
			var go = new GameObject();
			go.transform.parent = tf;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = Vector3.zero;
			go.transform.localScale = Vector3.zero;
			go.transform.parent = tf.parent;
			return go;
		}
		
		public void Update()
		{
			if (_physicalObject.IsHeld)
			{
				if (_physicalObject.m_hand != null)
				{
					if (!_physicalObject.m_hand.IsThisTheRightHand)
					{
						_physicalObject.PoseOverride = LeftHand;
						_physicalObject.PoseOverride_Touch = LeftHand_Touch;
					}
					else
					{
						_physicalObject.PoseOverride = RightHand;
						_physicalObject.PoseOverride = RightHand_Touch;
					}
				}
			}
		}
	}
}