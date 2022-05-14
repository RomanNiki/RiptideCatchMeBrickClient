﻿using UnityEngine;

public class Player : MonoBehaviour
{
    public ushort Id { get; private set; }
    public string UserName { get; private set; }
    public bool IsLocal { get; private set; }

    public void Init(ushort id, string userName, bool isLocal)
    {
        Id = id;
        UserName = userName;
        IsLocal = isLocal;
    }
    
    private void OnDestroy()
    {
        PlayerSpawner.Players.Remove(Id);
    }
}