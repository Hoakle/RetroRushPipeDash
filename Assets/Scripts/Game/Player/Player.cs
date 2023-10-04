using System.Collections;
using System.Collections.Generic;
using HoakleEngine;
using HoakleEngine.Addons;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Engine;
using RetroRush.Game.Economics;
using RetroRush.Game.Gameplay;
using RetroRush.Game.PlayerNS;
using RetroRush.GameData;
using RetroRush.GameSave;
using UnityEngine;

namespace RetroRush.Game.PlayerNS
{
    public class Player : GraphicalObjectRepresentation<PlayerData>
    {
        private const string KEY_JUMP = "Jump";
        private const string KEY_SLIDE = "Slide";
        private static readonly int HASH_JUMP = Animator.StringToHash(KEY_JUMP);
        private static readonly int HASH_SLIDE = Animator.StringToHash(KEY_SLIDE);
        
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private PlayerInputV2 _PlayerInput = null;
        [SerializeField] private Rigidbody _Rigidbody = null;
        [SerializeField] private CapsuleCollider _Collider = null;
        [SerializeField] private Material _Material;
        [SerializeField] private string _DissolvePower;
        [SerializeField] private MeshTrail _MeshTrail = null;

        private GlobalGameSave _GlobalGameSave;
        private UpgradeData _CoinUpgrade;
        private UpgradeConfigData _CoinConfig;
        private bool _IsCoinBonusActive;

        private bool _IsGameOver;
        public override void OnReady()
        {
            _GlobalGameSave = _GraphicsEngine.GetEngine<GameEngine>().GameSave.GetSave<GlobalGameSave>();
            
            _CoinConfig = _GraphicsEngine.ConfigContainer.GetConfig<GameplayConfigData>().GetUpgradeConfig(PickableType.CoinFactor);
            _CoinUpgrade = _GlobalGameSave.Upgrades.Find(b => b.Type == PickableType.CoinFactor);
            
            EventBus.Instance.Subscribe(EngineEventType.SpeedBonus, ActiveBonusSpeedParticulSystem);
            EventBus.Instance.Subscribe(EngineEventType.StartBoost, ActiveStartBoost);
            EventBus.Instance.Subscribe(EngineEventType.SpeedBonusFadeOut, DeactiveBonusSpeedParticulSystem);
            EventBus.Instance.Subscribe(EngineEventType.CoinFactorStarted, ActiveCoinFactor);
            EventBus.Instance.Subscribe(EngineEventType.Coin, CollectCoin);
            EventBus.Instance.Subscribe(EngineEventType.PlayerDied, PlayerDied);
            EventBus.Instance.Subscribe(EngineEventType.GameOver, GameOver);

            _GraphicsEngine.OnCameraControlChange += AddPlayerAsTarget;
            _GraphicsEngine.CameraControl.AddTarget(this.transform);
            
            _PlayerInput.OnJump += TryJump;
            _PlayerInput.OnSlide += TrySlide;

            _IsGameOver = false;
            _Material.SetFloat(_DissolvePower, 1);
            _MeshTrail.IsActive = false;
            _Jump = false;
            _JumpCount = 0;
            
            base.OnReady();
        }
        
        private bool _Jump;
        private float _JumpStart;
        private float _JumpThreshold = 0.1f;
        private int _JumpCount;

        public override void Dispose()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.SpeedBonus, ActiveBonusSpeedParticulSystem);
            EventBus.Instance.UnSubscribe(EngineEventType.StartBoost, ActiveStartBoost);
            EventBus.Instance.UnSubscribe(EngineEventType.SpeedBonusFadeOut, DeactiveBonusSpeedParticulSystem);
            EventBus.Instance.UnSubscribe(EngineEventType.CoinFactorStarted, ActiveCoinFactor);
            EventBus.Instance.UnSubscribe(EngineEventType.Coin, CollectCoin);
            EventBus.Instance.UnSubscribe(EngineEventType.PlayerDied, PlayerDied);
            EventBus.Instance.UnSubscribe(EngineEventType.GameOver, GameOver);
            
            _GraphicsEngine.CameraControl.RemoveTarget(this.transform);
            _GraphicsEngine.OnCameraControlChange -= AddPlayerAsTarget;
            _PlayerInput.OnJump -= TryJump;
            _PlayerInput.OnSlide -= TrySlide;
            
            base.Dispose();
        }

        private void AddPlayerAsTarget()
        {
            _GraphicsEngine.CameraControl.AddTarget(this.transform);
        }
        private void Update()
        {
            Move();
            
            if (_Jump && IsGrounded() && _JumpStart < Time.time - _JumpThreshold)
            {
                _Animator.SetBool(HASH_JUMP, false);
                _Jump = false;
            }

            CheckEnd();
        }

        private void TryJump()
        {
            if(IsGrounded())
                Jump();
        }
        private void Jump()
        {
            AudioPlayer.Instance.Play(AudioKeys.Jump);
            _Rigidbody.AddForce(Vector3.up * 7, ForceMode.Impulse);
            _Animator.SetBool(HASH_JUMP, true);
            _Jump = true;
            _JumpStart = Time.time;
            _JumpCount++;
        }

        private void TrySlide()
        {
            if(IsGrounded())
                Slide();
            else
            {
                _Rigidbody.AddForce(Vector3.down * 7, ForceMode.Impulse);
            }
        }
        
        private void Slide()
        {
            _Animator.SetTrigger(HASH_SLIDE);
        }

        private void Move()
        {
            var position = transform.position;
            position = new Vector3(position.x, position.y, position.z);
            transform.position = position;
        }
        
        private void CheckEnd()
        {
            if(transform.position.y <= -5.5f && !_IsGameOver)
            {
                _IsGameOver = true;
                EventBus.Instance.Publish(EngineEventType.PlayerDied);
            }
        }
        private bool IsGrounded()
        {
            var bounds = _Collider.bounds;
            //For BoxCollider detection
            //return Physics.CheckBox(bounds.center, new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), Quaternion.identity, LayerMask.GetMask("Default"));
            return Physics.CheckCapsule(bounds.center, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), _Collider.radius * 0.5f, LayerMask.GetMask("Ground"));
        }

        public void ActiveBonusSpeedParticulSystem()
        {
            _MeshTrail.IsActive = true;
        }

        public void DeactiveBonusSpeedParticulSystem()
        {
            _MeshTrail.IsActive = false;
        }

        public void ActiveStartBoost()
        {
            _MeshTrail.IsActive = true;
        }
        private void ActiveCoinFactor()
        {
            _IsCoinBonusActive = true;
            _MeshTrail.IsActive = true;
        }
        private void CollectCoin()
        {
            float value = 1;
            if (_IsCoinBonusActive)
                value *= _CoinConfig.GetFactor(_CoinUpgrade.Level);
            
            _GlobalGameSave.Wallet.Add(CurrencyType.Coin, (long) value);
            Data.CollectedCoin++;
        }

        private Coroutine _DissolveCoroutine;
        private void PlayerDied()
        {
            _Rigidbody.useGravity = false;
            _Rigidbody.velocity = Vector3.zero;
            
            _DissolveCoroutine = StartCoroutine(PlayerDissolve());
            
            if(_JumpCount >= 15)
                _GraphicsEngine.GetEngine<GameEngineImpl>().CompleteMission(MissionType.BUNNY_UP);
        }

        private IEnumerator PlayerDissolve()
        {
            yield return new WaitForEndOfFrame();
            
            AudioPlayer.Instance.Play(AudioKeys.GameOver);
            float power = _Material.GetFloat(_DissolvePower);
            while (power > 0)
            {
                power = Mathf.Max(power - 1f * Time.deltaTime, 0);
                _Material.SetFloat(_DissolvePower, power);
                yield return new WaitForEndOfFrame();
            }
            
            _Rigidbody.useGravity = true;
            EventBus.Instance.Publish(EngineEventType.GameOver);
        }
        private void GameOver()
        {
            StopCoroutine(_DissolveCoroutine);
            Dispose();
        }

        public void NotifyMovement(float speed)
        {
            _MeshTrail.Speed = speed;
        }
    }
}
