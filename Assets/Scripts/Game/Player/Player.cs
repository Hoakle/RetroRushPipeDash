using HoakleEngine.Addons;
using HoakleEngine.Addons.Scripts;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.GameData;
using RetroRush.GameSave;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Player
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
        [SerializeField] private MeshTrail _MeshTrail = null;

        private IGameState _GameState;
        private ThirdPersonCameraControl _ThirdPersonCameraControl;
        private GlobalGameSave _GlobalGameSave;
        private AudioPlayer _AudioPlayer;
        
        private bool _IsCoinBonusActive;

        [Inject]
        public void Inject(
            IGameState gameState,
            ThirdPersonCameraControl thirdPersonCameraControl,
            GlobalGameSave globalGameSave,
            AudioPlayer audioPlayer)
        {
            _GameState = gameState;
            _ThirdPersonCameraControl = thirdPersonCameraControl;
            _GlobalGameSave = globalGameSave;
            _AudioPlayer = audioPlayer;

            _GameState.State
                .Where(state => state == State.PlayerDied)
                .Subscribe(_ => PlayerDied());
        }
        public override void OnReady()
        {
            transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            
            EventBus.Instance.Subscribe(EngineEventType.Continue, InitPlayer);
            EventBus.Instance.Subscribe(EngineEventType.BackToMenu, Dispose);
            
            _ThirdPersonCameraControl.AddTarget(transform);

            _PlayerInput.OnJump += TryJump;
            _PlayerInput.OnSlide += TrySlide;

            InitPlayer();
            _JumpCount = 0;

            base.OnReady();
        }

        private bool _Jump;
        private float _JumpStart;
        private float _JumpThreshold = 0.1f;
        private int _JumpCount;

        public override void Dispose()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.Continue, InitPlayer);
            EventBus.Instance.UnSubscribe(EngineEventType.BackToMenu, Dispose);
            
            _GraphicsEngine.CameraControl.RemoveTarget(this.transform);
            _PlayerInput.OnJump -= TryJump;
            _PlayerInput.OnSlide -= TrySlide;
            
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
            _AudioPlayer.Play(AudioKeys.Jump);
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
                _Animator.SetBool(HASH_JUMP, false);
                _Jump = false;
                Slide();
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
            if(transform.position.y <= -5.5f && _GameState.State.Value == State.Start)
            {
                _GameState.SetState(State.PlayerDied);
            }
        }
        private bool IsGrounded()
        {
            var bounds = _Collider.bounds;
            //For BoxCollider detection
            //return Physics.CheckBox(bounds.center, new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), Quaternion.identity, LayerMask.GetMask("Default"));
            return Physics.CheckCapsule(bounds.center, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), _Collider.radius * 0.5f, LayerMask.GetMask("Ground"));
        }
        
        private void PlayerDied()
        {
            _Rigidbody.useGravity = false;
            _Rigidbody.velocity = Vector3.zero;
            _Collider.enabled = false;

            if(_JumpCount >= 15)
                _GlobalGameSave.CompleteMission(MissionType.BUNNY_UP);
        }

        private void InitPlayer()
        {
            _MeshTrail.IsActive = false;
            _Jump = false;
            _Rigidbody.useGravity = true;
            _Collider.enabled = true;
        }

        public void NotifyMovement(float speed)
        {
            _MeshTrail.Speed = speed;
        }
    }
}
