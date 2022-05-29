using System;
using System.Collections.Generic;
using Multiplayer;
using PlayerComponents;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private PrefabContainer _prefabContainer;
        public static readonly Dictionary<ushort, Projectile> Projectiles = new Dictionary<ushort, Projectile>();
        private static PrefabContainer _staticContainer;

        private void Start()
        {
            _staticContainer = _prefabContainer;
        }

        public static void Spawn(ushort id, WeaponType type, ushort shooterId, Vector3 position)
        {
            try
            {
                PlayerSpawner.Players[shooterId].Shooter.Shoot();
                var projectile =
                    Instantiate(_staticContainer.GetProjectile(type), position, Quaternion.identity);

                projectile.name = $"Projectile {id}";
                projectile.Init(id, position);

                Projectiles.Add(id, projectile);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        [MessageHandler((ushort) ServerToClientId.ProjectileSpawned)]
        private static void ProjectileSpawned(Message message)
        {
            Spawn(message.GetUShort(), (WeaponType) message.GetByte(), message.GetUShort(), message.GetVector3());
        }

        [MessageHandler((ushort) ServerToClientId.ProjectileMovement)]
        private static void ProjectileMovement(Message message)
        {
            if (Projectiles.TryGetValue(message.GetUShort(), out var projectile))
            {
                if (projectile != null)
                {
                    projectile.Move(message.GetUShort(), message.GetVector3());
                    var rotation = message.GetQuaternion();
                    projectile.transform.rotation = rotation;
                }
            }
        }

        [MessageHandler((ushort) ServerToClientId.ProjectileCollided)]
        private static void ProjectileCollided(Message message)
        {
            if (Projectiles.TryGetValue(message.GetUShort(), out var projectile))
                Destroy(projectile.gameObject);
        }
    }
}