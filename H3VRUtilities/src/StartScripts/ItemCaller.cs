using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace H3VRUtils.QOL
{
	public class ItemCaller : MonoBehaviour
	{
		[Tooltip("This deletes the gameobject this script is on after death.")]
		public bool deleteGameObjectAfterSpawn;
		public bool spawnKinematicLocked;
		public List<ItemCallerSet> sets = new List<ItemCallerSet>() { new ItemCallerSet() {primaryItemID = "MyItemId", backupID = "My Backup ID if Primary ID fails to spawn"}};
		public void Start()
		{
			foreach (var set in sets)
			{
				GameObject go = new GameObject();
				go.transform.parent = transform;
				go.transform.position = transform.position;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition += set.offset;
				Vector3 wPos = go.transform.position;
				Destroy(go);
				FVRObject obj = null;
				try
				{
					obj = IM.OD[set.primaryItemID];
					var res = Instantiate(obj.GetGameObject(), wPos, transform.rotation);
					if (spawnKinematicLocked)
						res.GetComponent<Rigidbody>().isKinematic = true;
				}
				catch //if it fails to spawn the primary ID
				{
					Debug.Log($"Item ID {set.primaryItemID} not found; attempting to spawn backupID");
					obj = IM.OD[set.backupID];
					Instantiate(obj.GetGameObject(), wPos, transform.rotation);
				}
				if(deleteGameObjectAfterSpawn) Destroy(this.gameObject);
				else Destroy(this);
				
			}
		}
	}

	[Serializable]
	public class ItemCallerSet
	{
		[Tooltip("Primary ID. this is the item the object will attempt to spawn.")]
		public string primaryItemID;
		[Tooltip("If your item fails to spawn, it will spawn the backup ID.")]
		public string backupID;
		[Tooltip("Offset from object position it will spawn from in local world space.")]
		public Vector3 offset;
	}
}