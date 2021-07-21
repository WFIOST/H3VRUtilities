using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	public class LockToHead : MonoBehaviour
	{
		public Vector3 posOffset;
		public void Update()
		{
			this.transform.position = GM.CurrentPlayerBody.Head.transform.position;
			transform.localPosition += posOffset;
			this.transform.rotation = GM.CurrentPlayerBody.Head.transform.rotation;
		}
	}
}
