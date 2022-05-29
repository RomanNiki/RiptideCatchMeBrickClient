using System.Collections.Generic;
using Projectiles;
using SharedLibrary;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "PrefabContainer/New PrefabContainer")]
public class PrefabContainer : ScriptableObject
{
    [FormerlySerializedAs("_localClientPrefab")] [SerializeField]
    private PlayerComponents.Player _localPlayerPrefab;

    public PlayerComponents.Player LocalPlayerPrefab => _localPlayerPrefab;

    [FormerlySerializedAs("_playerPrefab")] [FormerlySerializedAs("_clientPrefab")] [SerializeField]
    private PlayerComponents.Player _redPlayerPrefab;

    public PlayerComponents.Player RedPlayerPrefab => _redPlayerPrefab;

    [SerializeField] private PlayerComponents.Player _bluePlayerPrefab;
    public PlayerComponents.Player BluePlayerPrefab => _bluePlayerPrefab;

    [SerializeField] private List<Projectile> _projectiles;

    public Projectile GetProjectile(WeaponType type)
    {
        var value = (int) type;
        return _projectiles[value];
    }
}