﻿using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.Tests;
using UnityEngine;
using Education.FeelPhysics.MyHolographicAcademy;

public class MagnetSpawner : MonoBehaviour {

    [SerializeField]
    private PrefabSpawnManager spawnManager;

    public TextMesh DebugLogText;

    private long userId;

    // Use this for initialization
    void Start () {
        //DebugLogText = GameObject.Find("Debug Log").GetComponent<TextMesh>();

        // SharingStage should be valid at this point, but we may not be connected.
        if (SharingStage.Instance.IsConnected)
        {
            Connected();
        }
        else
        {
            SharingStage.Instance.SharingManagerConnected += Connected;
            DebugLogText.text += "\n[MagnetMisc] Add event SharingManagerConnected";
        }

        /*
        // プレハブを取得
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Sharing Magnet");
        Vector3 position = new Vector3(0, 0, 2.0f);
        // プレハブからインスタンスを生成
        Instantiate(prefab, position, Quaternion.identity);
        */
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Connected(object sender = null, System.EventArgs e = null)
    {
        DebugLogText.text += "\n[Magnet] Connected";
        SharingStage.Instance.SharingManagerConnected -= Connected;

        SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession;
        DebugLogText.text += "\n[Magnet] Add event UserJoined";
        SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession;
    }

    /// <summary>
    /// Called when a new user is leaving the current session.
    /// </summary>
    /// <param name="user">User that left the current session.</param>
    private void UserLeftSession(User user)
    {
        DebugLogText.text += "\n[Magnet] UserLeftSession(User user) > user.GetID(): " + user.GetID().ToString();
        /*
        int userId = user.GetID();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            RemoveRemoteMagnet(remoteMagnets[userId].MagnetObject);
            remoteMagnets.Remove(userId);
        }
        */
    }

    /// <summary>
    /// Called when a user is joining the current session.
    /// </summary>
    /// <param name="user">User that joined the current session.</param>
    private void UserJoinedSession(User user)
    {
        userId = user.GetID();
        DebugLogText.text += "\n[Magnet] UserJoinedSession(User user) > user.GetID(): " +
            user.GetID().ToString();
        DebugLogText.text += "\n[Magnet] UserJoinedSession(User user) > " +
            "SharingStage.Instance.Manager.GetLocalUser().GetID(): " +
            SharingStage.Instance.Manager.GetLocalUser().GetID().ToString();
        if (user.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            //GetRemoteMagnetInfo(user.GetID());
        }

        CreateMagnet(userId);
    }

    private void CreateMagnet(long userId)
    {
        RemoteMagnetManager.RemoteMagnetInfo magnetInfo;

        if (!RemoteMagnetManager.Instance.RemoteMagnets.TryGetValue(userId, out magnetInfo))
        {
            Vector3 position = new Vector3(0, 0, 1.5f);
            Quaternion rotation = Quaternion.identity;
            var spawnedObject = new SyncSpawnMagnet();
            spawnManager.Spawn(spawnedObject, position, rotation, null, "SpawnedMagnet", false);

            // 生成した磁石にuserIdを紐付ける
            magnetInfo = new RemoteMagnetManager.RemoteMagnetInfo();
            magnetInfo.UserID = userId;
            magnetInfo.MagnetObject = GameObject.Find("SpawnedMagnet");

            RemoteMagnetManager.Instance.RemoteMagnets.Add(userId, magnetInfo);
        }

    }
}
