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
		public void Start()
		{
			//we do a slightly unavoidable amount of trolling
			var objs = FindObjectsOfType<Sosig>();
			Debug.Log("Found " + objs.Length + " sosigs!");
			for (int i = 0; i < objs.Length; i++)
			{
				Debug.Log("Sosig found: " + objs[i].name);
			}
			if (objs.Length == 0) return;
			
			//this and the for basically just yoinks the closest sosig
			GameObject curobj = null;
			float curobjdist = 999999;
			for (int i = 0; i < objs.Length; i++)
			{
				float distance = Vector3.Distance(this.transform.position, objs[i].gameObject.transform.position);
				if(distance < curobjdist)
				{
					Debug.Log("Current Sosig: " + objs[i].name);
					curobj = objs[i].gameObject;
					curobjdist = distance;
				}
			}

			if (curobj == null) { Debug.LogError("The nearest sosig is over a full 1,000 kms away, what the fuck are you doing?"); return; }

			//look at sosig
			transform.LookAt(curobj.transform);
			Debug.Log("looking at " + curobj.transform);
			//do the troll
			rigidbody.AddForce(new Vector3(0, 0, speed));
			Debug.Log("setting velocity to " + speed);
		}
	}
}
