using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using RetroRush.Game.Player;
using RetroRush.GameSave;
using UnityEngine;

namespace RetroRush.Game.PlayerNS
{
    public class Player : GraphicalObjectRepresentation<PlayerData>
    {
        private const string KEY_JUMP = "Jump";
        private static readonly int HASH_JUMP = Animator.StringToHash(KEY_JUMP);
        
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private PlayerInput _PlayerInput = null;
        [SerializeField] private Rigidbody _Rigidbody = null;
        [SerializeField] private CapsuleCollider _Collider = null;
        
        [SerializeField] private ParticleSystem _BonusSpeedFx = null;
        [SerializeField] private MagnetForceField _MagnetForceField = null;

        private GlobalGameSave _GlobalGameSave;
        public override void OnReady()
        {
            _GlobalGameSave = _GraphicsEngine.GetEngine<GameEngine>().GameSave.GetSave<GlobalGameSave>();
            
            EventBus.Instance.Subscribe(EngineEventType.SpeedBonus, ActiveBonusSpeedParticulSystem);
            EventBus.Instance.Subscribe(EngineEventType.SpeedBonusFadeOut, DeactiveBonusSpeedParticulSystem);
            EventBus.Instance.Subscribe(EngineEventType.Coin, CollectCoin);
            EventBus.Instance.Subscribe(EngineEventType.GameOver, Dispose);
            _GraphicsEngine.CameraControl.AddTarget(this.transform);
            
            _PlayerInput.OnJump += TryJump;
            _Jump = false;
            
            base.OnReady();
        }
        
        private bool _Jump;
        private float _JumpStart;
        private float _JumpThreshold = 0.1f;

        public override void Dispose()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.SpeedBonus, ActiveBonusSpeedParticulSystem);
            EventBus.Instance.UnSubscribe(EngineEventType.SpeedBonusFadeOut, DeactiveBonusSpeedParticulSystem);
            EventBus.Instance.UnSubscribe(EngineEventType.Coin, CollectCoin);
            EventBus.Instance.UnSubscribe(EngineEventType.GameOver, Dispose);
            _GraphicsEngine.CameraControl.RemoveTarget(this.transform);
            _PlayerInput.OnJump -= TryJump;
            base.Dispose();
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
            _Rigidbody.AddForce(Vector3.up * 7, ForceMode.Impulse);
            _Animator.SetBool(HASH_JUMP, true);
            _Jump = true;
            _JumpStart = Time.time;
        }

        private void Move()
        {
            var position = transform.position;
            position = new Vector3(position.x, position.y, position.z);
            transform.position = position;
        }

        private void CheckEnd()
        {
            if(transform.position.y <= -6.5f)
            {
                EventBus.Instance.Publish(EngineEventType.GameOver);
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
            _BonusSpeedFx.Play();
        }

        public void DeactiveBonusSpeedParticulSystem()
        {
            _BonusSpeedFx.Stop();
        }

        private void CollectCoin()
        {
            _GlobalGameSave.Wallet.Add(CurrencyType.Coin, 1);
            Data.CollectedCoin++;
        }
    }
}
