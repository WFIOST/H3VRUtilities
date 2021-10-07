using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	public class RotateTowardsPlayer : MonoBehaviour
	{
		public bool rotateOnX;
		public bool rotateOnY;
		public bool rotateOnZ;
		public void Start()
		{
			var rot = gameObject.transform.localEulerAngles;
			gameObject.transform.LookAt(GM.CurrentPlayerRoot);
			if (rotateOnX) rot.x = gameObject.transform.localEulerAngles.x;
			if (rotateOnY) rot.y = gameObject.transform.localEulerAngles.y;
			if (rotateOnZ) rot.z = gameObject.transform.localEulerAngles.z;
			gameObject.transform.localEulerAngles = rot;
		}
	}
}