﻿using System;
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

		[Header("Translation Mag Follower")]
		public bool UsesOneRoundPos;
		public int StartAtRoundCount;
		public int StopAtRoundCount;
		[Tooltip("The position where the follower should be when the magazine is empty.")]
		public GameObject StartPos;
		[Tooltip("The position where the follower should be when the magazine has one round left.")]
		public GameObject OneRoundPos;
		[Tooltip("The position where the follower should be when the magazine is full.")]
		public GameObject StopPos;

		[Header("Individual Point Mag Follower")]
		public bool UsesIndivdualPointMagFollower;
		[Tooltip("Top-to-bottom order, where the 0th position is when the magazine is empty. ")]
		public List<GameObject> Positions;

		[Header("Individual Mesh Replacement")]
		public bool UsesIndividualMeshReplacement;
		[Tooltip("Top-to-bottom order, where the 0th position is when the magazine is empty. ")]
		public List<Mesh> Meshes;

		private int magrounds;

		private MeshFilter followerFilter;

		public void Update()
		{
			if (magazine.m_numRounds != magrounds)
			{
				magrounds = magazine.m_numRounds;
				UpdateDisp();
			}
		}

		public void Start()
		{
			followerFilter = follower.GetComponent<MeshFilter>();
		}

		public void UpdateDisp()
		{
			if (UsesOneRoundPos)
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

			if (UsesIndivdualPointMagFollower)
			{
				if (Positions.Count < magazine.m_numRounds)
				{
					return;
				}
				if (Positions[magazine.m_numRounds] == null)
				{
					return;
				}
				follower.transform.position = Positions[magazine.m_numRounds].transform.position;
				follower.transform.rotation = Positions[magazine.m_numRounds].transform.rotation;
			}

			if (UsesIndivdualPointMagFollower)
			{
				if (Positions.Count < magazine.m_numRounds)
				{
					return;
				}
				if (Positions[magazine.m_numRounds] == null)
				{
					return;
				}
				follower.transform.position = Positions[magazine.m_numRounds].transform.position;
				follower.transform.rotation = Positions[magazine.m_numRounds].transform.rotation;
			}
			if (UsesIndividualMeshReplacement)
			{
				if (Meshes.Count < magazine.m_numRounds)
				{
					return;
				}
				if (Meshes[magazine.m_numRounds] == null)
				{
					return;
				}
				followerFilter.mesh = Meshes[magazine.m_numRounds];
				followerFilter.mesh = Meshes[magazine.m_numRounds];
			}
		}
	}
}
