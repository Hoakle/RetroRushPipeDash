using System.Collections;
using System.Collections.Generic;
using HoakleEngine.Core.Audio;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Player
{
    public class PlayerDissolve : MonoBehaviour
    {
        [SerializeField] private List<Material> _Materials;
        private const string _DissolvePower = "_DissolvePower";

        private IGameState _GameState;
        private AudioPlayer _AudioPlayer;
        
        [Inject]
        public void Inject(IGameState gameState,
            AudioPlayer audioPlayer)
        {
            _GameState = gameState;
            _AudioPlayer = audioPlayer;
            
            gameState.State
                .Where(state => state == State.Start)
                .Subscribe(_ => ResetMaterials());

            gameState.State
                .Where(state => state == State.PlayerDied)
                .Subscribe(_ => StartCoroutine(Dissolve()));
        }

        private void SetDissolvePower(float power)
        {
            foreach (var material in _Materials)
            {
                material.SetFloat(_DissolvePower, power);
            }
        }
        
        private void ResetMaterials()
        {
            SetDissolvePower(1);
        }
        
        private IEnumerator Dissolve()
        {
            yield return new WaitForEndOfFrame();
            
            _AudioPlayer.Play(AudioKeys.GameOver);
            float power = 1;
            while (power > 0)
            {
                power = Mathf.Max(power - 1f * Time.deltaTime, 0);
                SetDissolvePower(power);
                
                yield return new WaitForEndOfFrame();
            }
            
            _GameState.SetState(State.GameOver);
        }
    }
}
