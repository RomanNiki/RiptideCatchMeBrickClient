using System;
using UnityEngine;

namespace PlayerComponents
{
    [RequireComponent(typeof(Interpolator))]
    public class PlayerMover : MonoBehaviour
    {
        private Interpolator _interpolator;

        private void Start()
        {
            _interpolator = GetComponent<Interpolator>();
        }

        public void Move(ushort tick, Vector3 newPosition, Quaternion rotation, bool isTeleport)
        {
            try
            {
                _interpolator.NewUpdate(tick, newPosition, isTeleport);
            }
            catch (Exception e)
            {
               Debug.Log(e);
            }
            transform.rotation = rotation;
        }
    }
}