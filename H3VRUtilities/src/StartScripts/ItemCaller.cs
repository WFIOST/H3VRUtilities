using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace H3VRUtils.QOL
{
	public class ItemCaller : MonoBehaviour
	{
		public List<ItemCallerSet> sets;
		public void Start()
		{
			foreach (var set in sets)
			{
				FVRObject obj = null;
				try
				{
					obj = IM.OD[set.primaryItemID];
					Instantiate(obj.GetGameObject(), transform.position += set.offset, transform.rotation);
				}
				catch
				{
					Debug.Log($"Item ID {set.primaryItemID} not found; attempting to spawn backupID");
					obj = IM.OD[set.backupID];
					Instantiate(obj.GetGameObject(), transform.position += set.offset, transform.rotation);
				}
				Destroy(this);
			}
		}
	}

	[Serializable]
	public class ItemCallerSet
	{
		public string primaryItemID;
		[Tooltip("If your item fails to spawn, it will spawn the backup ID.")]
		public string backupID;
		[Tooltip("Offset from object position it will spawn from.")]
		public Vector3 offset;
	}
}