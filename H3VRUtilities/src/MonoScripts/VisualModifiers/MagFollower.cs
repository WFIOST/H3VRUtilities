using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class MagFollower : MonoBehaviour
	{
		public FVRFireArmMagazine magazine;

		public GameObject follower;

		[FormerlySerializedAs("UsesOneRoundPos")] [Header("Translation Mag Follower")]
		public bool usesOneRoundPos;
		[FormerlySerializedAs("StartAtRoundCount")] public int startAtRoundCount;
		[FormerlySerializedAs("StopAtRoundCount")] public int stopAtRoundCount;
		[FormerlySerializedAs("StartPos")] [Tooltip("The position where the follower should be when the magazine is empty.")]
		public GameObject startPos;
		[FormerlySerializedAs("OneRoundPos")] [Tooltip("The position where the follower should be when the magazine has one round left.")]
		public GameObject oneRoundPos;
		[FormerlySerializedAs("StopPos")] [Tooltip("The position where the follower should be when the magazine is full.")]
		public GameObject stopPos;

		[FormerlySerializedAs("UsesIndivdualPointMagFollower")] [Header("Individual Point Mag Follower")]
		public bool usesIndivdualPointMagFollower;
		[FormerlySerializedAs("Positions")] [Tooltip("Top-to-bottom order, where the 0th position is when the magazine is empty. ")]
		public List<GameObject> positions;

		[FormerlySerializedAs("UsesIndividualMeshReplacement")] [Header("Individual Mesh Replacement")]
		public bool usesIndividualMeshReplacement;
		[FormerlySerializedAs("Meshes")] [Tooltip("Top-to-bottom order, where the 0th position is when the magazine is empty. ")]
		public List<Mesh> meshes;

		private int _magrounds;

		private MeshFilter _followerFilter;

		public void Update()
		{
			if (magazine.m_numRounds != _magrounds)
			{
				_magrounds = magazine.m_numRounds;
				UpdateDisp();
			}
		}

		public void Start()
		{
			_followerFilter = follower.GetComponent<MeshFilter>();
		}

		public void UpdateDisp()
		{

			if (usesIndivdualPointMagFollower)
			{
				if (positions.Count < magazine.m_numRounds)
				{
					return;
				}
				if (positions[magazine.m_numRounds] == null)
				{
					return;
				}
				follower.transform.position = positions[magazine.m_numRounds].transform.position;
				follower.transform.rotation = positions[magazine.m_numRounds].transform.rotation;
			}
			else if (usesIndividualMeshReplacement)
			{
				if (meshes.Count < magazine.m_numRounds)
				{
					return;
				}
				if (meshes[magazine.m_numRounds] is null)
				{
					return;
				}
				_followerFilter.mesh = meshes[magazine.m_numRounds];
				_followerFilter.mesh = meshes[magazine.m_numRounds];
			}
			else //if no other use
			{
				Transform b = stopPos.transform;
				int c = stopAtRoundCount;
				if (usesOneRoundPos) { b = oneRoundPos.transform; c++; }

				follower.transform.position = Vector3.Lerp(startPos.transform.position, b.position, Mathf.InverseLerp((float)startAtRoundCount, (float)c, magazine.m_numRounds));

				if (magazine.m_numRounds == 0)
				{
					follower.transform.position = stopPos.transform.position;
				}
			}
		}
	}
}
