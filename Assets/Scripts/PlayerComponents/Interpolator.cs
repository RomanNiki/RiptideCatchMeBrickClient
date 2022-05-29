using System;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

namespace PlayerComponents
{
    public class Interpolator : MonoBehaviour
    {
        [SerializeField] private float _timeElapsed = 0f;
        [SerializeField] private float _timeToReachTarget = 0.05f;
        [SerializeField] private float _movementThreshold = 0.05f;

        private readonly List<TransformUpdate> _futureTransformUpdates = new List<TransformUpdate>();

        private float _squareMovementThreshold;
        private TransformUpdate _to;
        private TransformUpdate _from;
        private TransformUpdate _previous;

        private void Start()
        {
            _squareMovementThreshold = _movementThreshold * _movementThreshold;
            var _position = transform.position;
            _to = new TransformUpdate(Networking.ServerTick, _position);
            _from = new TransformUpdate(Networking.InterpolationTick, _position);
            _previous = new TransformUpdate(Networking.InterpolationTick, _position);
        }

        private void Update()
        {
            for (var i = 0; i < _futureTransformUpdates.Count; i++)
            {
                if (Networking.ServerTick >= _futureTransformUpdates[i].Tick)
                {
                    _previous = _to;
                    _to = _futureTransformUpdates[i];
                    _from = new TransformUpdate(Networking.InterpolationTick, transform.position);


                    _futureTransformUpdates.RemoveAt(i);
                    i--;
                    _timeElapsed = 0f;
                    _timeToReachTarget = (_to.Tick - _from.Tick) * Time.fixedDeltaTime;
                }
            }

            _timeElapsed += Time.deltaTime;
            InterpolatePosition(_timeElapsed / _timeToReachTarget);
        }

        private void InterpolatePosition(float lerpAmount)
        {
            if ((_to.Position - _previous.Position).sqrMagnitude < _squareMovementThreshold)
            {
                if (_to.Position != _from.Position)
                    transform.position = Vector3.Lerp(_from.Position, _to.Position, lerpAmount);

                return;
            }

            var newPosition = Vector3.Lerp(_from.Position, _to.Position, lerpAmount);
            transform.position = newPosition;
        }

        public void NewUpdate(ushort tick, Vector3 position, bool isTeleport)
        {
            try
            {
                if (tick <= Networking.InterpolationTick && !isTeleport)
                    return;

                for (var i = 0; i < _futureTransformUpdates.Count; i++)
                {
                    if (tick < _futureTransformUpdates[i].Tick)
                    {
                        _futureTransformUpdates.Insert(i, new TransformUpdate(tick, position));
                        return;
                    }
                }

                _futureTransformUpdates.Add(new TransformUpdate(tick, position));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}