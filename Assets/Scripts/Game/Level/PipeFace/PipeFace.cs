using System;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.Game.Gameplay;
using RetroRush.Game.Gameplay.Obstacle;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class PipeFace : GraphicalObjectRepresentation<PipeFaceData>
    {
        [SerializeField] private Transform _MeshTransform;
        [SerializeField] private MeshRenderer _MeshRenderer;
        [SerializeField] private Transform _PickableContainer;
        [SerializeField] private Material _BaseMaterial;
        [SerializeField] private Material _ShieldMaterial;
        [SerializeField] private Material _FinishMaterial;
        public override void OnReady()
        {
            Data.OnRemoveFace += Dispose;

            SetState();
            SetPositionAndRotation();
            SetPickable();

            base.OnReady();
        }

        private void SetState()
        {
            gameObject.SetActive(Data.Exist);
            if (Data.PickableType == PickableType.Finish)
            {
                _MeshRenderer.sharedMaterial = _FinishMaterial;
            }
            else if (Data.Exist)
                _MeshRenderer.sharedMaterial = _BaseMaterial;
            else
            {
                EventBus.Instance.Subscribe(EngineEventType.Shield, ActiveShield);
                EventBus.Instance.Subscribe(EngineEventType.Continue, ActiveShield);
                EventBus.Instance.Subscribe(EngineEventType.StartBoost, ActiveShield);
                EventBus.Instance.Subscribe(EngineEventType.ShieldFadeOut, UnActiveShield);
                _MeshRenderer.sharedMaterial = _ShieldMaterial;
            }
        }

        private void SetPositionAndRotation()
        {
            _MeshTransform.localScale = Data.LocalScale;
            var parentPos = transform.parent.position;
            transform.position = new Vector3(Data.Position.x, Data.Position.y,
                parentPos.z + (_MeshTransform.localScale.z * Data.Depth) + _MeshTransform.localScale.z / 2);
            transform.Rotate(0, 0, 90 - transform.eulerAngles.z);
        }

        private void SetPickable()
        {
            switch (Data.PickableType)
            {
                case PickableType.Coin:
                    SetPickable<Coin>();
                    break;
                case PickableType.SpeedBonus:
                    SetPickable<SpeedBoost>();
                    break;
                case PickableType.Magnet:
                    SetPickable<Magnet>();
                    break;
                case PickableType.Shield:
                    SetPickable<Shield>();
                    break;
                default:
                    RotateAroundParent();
                    break;
            }
        }

        private void SetPickable<T>() where T : Pickable
        {
            AddGOR<T, PickableType>(Data.PickableType.ToString(), Data.PickableType, null,
                pickable =>
                {
                    pickable.transform.parent = _PickableContainer;
                    pickable.transform.position = _PickableContainer.position;
                    RotateAroundParent();
                });
        }

        public void SetObstacle()
        {
            AddGOR<Lazer, ObstacleType>(PrefabKeys.LAZER, ObstacleType.Lazer, null, lazer =>
            {
                lazer.transform.localScale = new Vector3(0.2f,
                    (float)Math.Sqrt(Math.Pow(transform.position.y - Data.LinkedFace.Position.y, 2) +
                                     Math.Pow(transform.position.x - Data.LinkedFace.Position.x, 2)) / 2f, 0.2f);

                lazer.transform.SetParent(transform, false);

                lazer.transform.position = new Vector3((transform.position.x + Data.LinkedFace.Position.x) / 2,
                    (transform.position.y + Data.LinkedFace.Position.y) / 2, transform.position.z);
                lazer.transform.LookAt(transform.position);
                lazer.transform.Rotate(Vector3.right, 90);

            });
        }

        private void RotateAroundParent()
        {
            transform.RotateAround(transform.parent.position, Vector3.back,
                Data.RotateAround - transform.parent.eulerAngles.z);
            Data.Position = transform.position;
        }

        public void ActiveShield()
        {
            gameObject.SetActive(true);
        }

        private void UnActiveShield()
        {
            gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if(Data.PickableType != PickableType.Finish)
                return;

            if (other.CompareTag("Player"))
            {
                EventBus.Instance.Publish(EngineEventType.Win);
            }
        }
        
        public override void Dispose()
        {
            Data.OnRemoveFace -= Dispose;
            if (!Data.Exist)
            {
                EventBus.Instance.UnSubscribe(EngineEventType.Shield, ActiveShield);
                EventBus.Instance.UnSubscribe(EngineEventType.Continue, ActiveShield);
                EventBus.Instance.UnSubscribe(EngineEventType.StartBoost, ActiveShield);
                EventBus.Instance.UnSubscribe(EngineEventType.ShieldFadeOut, UnActiveShield);
                _MeshRenderer.sharedMaterial = _ShieldMaterial;
            }

            base.Dispose();
        }
    }
}
