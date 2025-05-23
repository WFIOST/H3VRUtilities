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
		[Header("Only fill out one of the ones below.")]
		public FVRFireArmMagazine magazine;
		public FVRFireArmClip clip;
		
		public GameObject follower;
		
		[Header("Translation Mag Follower")]
		public bool UsesOneRoundPos;
		[Tooltip("The round count where the follower starts moving (e.g 20)")]
		public int StartAtRoundCount;
		[Tooltip("The round count where the follower stops moving (e.g 0)")]
		public int StopAtRoundCount;
		[Tooltip("The position where the follower should be when the magazine is full.")]
		public GameObject StartPos;
		[Tooltip("The position where the follower should be when the magazine has one round left.")]
		public GameObject OneRoundPos;
		[Tooltip("The position where the follower should be when the magazine is empty.")]
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

		public bool isClip;
		
		public void Update()
		{
			//i know this can use ternary. i'm not going to, because i dont like ternary.
			int rounds = 0;
			if (isClip) rounds = clip.m_numRounds;
			else rounds = magazine.m_numRounds;
			
			if (rounds != magrounds)
			{
				magrounds = rounds;
				UpdateDisp();
			}
		}
		
		public void Start()
		{
			if (clip != null)
			{
				if (magazine != null) { Debug.LogError("H3VR Utilities Follower has both a clip and mag inserted! What?"); }
				isClip = true;
			}
			followerFilter = follower.GetComponent<MeshFilter>();
			//fix if modder reverses vars by accident
			if (StopAtRoundCount > StartAtRoundCount)
			{
				var temp = StopAtRoundCount;
				StopAtRoundCount = StartAtRoundCount;
				StartAtRoundCount = temp;
			}
		}
		
		public void UpdateDisp()
		{

			if (UsesIndivdualPointMagFollower)
			{
				if (Positions.Count <= magazine.m_numRounds)
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
			else if (UsesIndividualMeshReplacement)
			{
				if (Meshes.Count <= magazine.m_numRounds)
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
			else //if no other use
			{
				Transform _b = StopPos.transform;
				int _c = StopAtRoundCount;
				if (UsesOneRoundPos) { _b = OneRoundPos.transform; _c++; }

				follower.transform.position = Vector3.Lerp(StartPos.transform.position, _b.position, Mathf.InverseLerp((float)StartAtRoundCount, (float)_c, magrounds));

				if (magrounds == 0)
				{
					follower.transform.position = StopPos.transform.position;
				}
			}
		}
	}
}
