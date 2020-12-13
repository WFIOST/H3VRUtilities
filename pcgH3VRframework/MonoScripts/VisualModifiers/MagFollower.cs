using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	public class MagFollower : MonoBehaviour
	{
		public FVRFireArmMagazine magazine;

		public GameObject follower;

		public int StartAtRoundCount;
		public int StopAtRoundCount;

		[Header("Translation Mag Follower")]
		public bool UsesOneRoundPos;
		[Tooltip("The position where the follower should be when the magazine is empty.")]
		public GameObject StartPos;
		[Tooltip("The position where the follower should be when the magazine has one round left.")]
		public GameObject OneRoundPos;
		[Tooltip("The position where the follower should be when the magazine is full.")]
		public GameObject StopPos;

		[Header("Rotation Mag Follower")]
		[Tooltip("The rotation where the follower should be when the magazine is empty.")]
		public float RotStartPos;
		[Tooltip("The rotation where the follower should be when the magazine has one round left.")]
		public float RotOneRoundPos;
		[Tooltip("The rotation where the follower should be when the magazine is full.")]
		public float RotStopPos;

		public void FixedUpdate()
		{
			Transform _b = StopPos.transform;
			int _c = StopAtRoundCount;
			if (UsesOneRoundPos) { _b = OneRoundPos.transform; _c++; }

			follower.transform.position = Vector3.Lerp(StartPos.transform.position, _b.position, Mathf.InverseLerp((float)StartAtRoundCount, (float)_c, magazine.m_numRounds));

			if (magazine.m_numRounds == 0)
			{
				follower.transform.position = StopPos.transform.position;
			}
		}
	}
}
