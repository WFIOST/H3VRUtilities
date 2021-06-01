using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;


namespace H3VRUtils.FVRInteractiveObjects
{
	class GoToNearestSosig : MonoBehaviour
	{
		public float speed;
		public Rigidbody rigidbody;
		[Tooltip("Change this if you want the object to throw itself to a different tag")]
		public string TagOverride;
		public void Start()
		{
			if (string.IsNullOrEmpty(TagOverride)) TagOverride = "AgentBody";
			//we do a slightly unavoidable amount of trolling
			GameObject[] objs = GameObject.FindGameObjectsWithTag(TagOverride);
			if (objs.Length == 0) return;
			
			//this and the for basically just yoinks the closest sosig
			GameObject curobj = null;
			float curobjdist = 999999;
			for (int i = 0; i < objs.Length; i++)
			{
				float distance = Vector3.Distance(this.transform.position, objs[i].transform.position);
				if(distance < curobjdist)
				{
					curobj = objs[i];
					curobjdist = distance;
				}
			}

			if (curobj == null) { Debug.LogError("The nearest sosig is over a full 1,000 kms away, what the fuck are you doing?"); return; }

			//look at sosig
			transform.LookAt(curobj.transform);
			//do the troll
			rigidbody.velocity = new Vector3(0,0,speed);
		}
	}
}
