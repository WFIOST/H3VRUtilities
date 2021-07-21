using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class FollowDir : MonoBehaviour
	{
		public GameObject leader;
		public GameObject follower;
		[FormerlySerializedAs("FollowDirection")] public CullOnZLoc.DirType followDirection;

		public Vector3 followerpos;
		public Vector3 leaderpos;
		public Vector3 resultpos;

		void Update()
		{
			/*			float[] dir = new float[3];
			dir[0] = follower.transform.position.x;
			dir[1] = follower.transform.position.y;
			dir[2] = follower.transform.position.z;

			dir[(int)FollowDirection] = leader.transform.position[(int)FollowDirection];

			follower.transform.position = Vector3.Lerp(follower.transform.position, leader.transform.position, 1);

			follower.transform.position = new Vector3(dir[0], dir[1], dir[2]);

			followerpos = new Vector3(follower.transform.position.x, follower.transform.position.y, follower.transform.position.z);
			leaderpos = new Vector3(leader.transform.position.x, leader.transform.position.y, leader.transform.position.z);
			resultpos = new Vector3(dir[0], dir[1], dir[2]);*/
			follower.transform.position = leader.transform.position;
		}
	}
}
