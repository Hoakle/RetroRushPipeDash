using System;
using System.Collections;
using HoakleEngine.Core.Communication;
using Unity.VisualScripting;
using UnityEngine;
using EventBus = HoakleEngine.Core.Communication.EventBus;

namespace RetroRush.Game.PlayerNS
{
    public class Shield : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _Mesh = null;
        [SerializeField] private string _DissolvePower = null;
        [SerializeField] private float _Speed = 1f;
        [SerializeField] private Material _Material = null;

        private Coroutine _Coroutine = null;
        private WaitForEndOfFrame _Waiter = new WaitForEndOfFrame();
        private WaitForSeconds _WarningWaiter = new WaitForSeconds(0.25f);
        private void Start()
        {
            EventBus.Instance.Subscribe(EngineEventType.ShieldFadeOutWarning, DisplayWarning);
            EventBus.Instance.Subscribe(EngineEventType.StartBoost, ActiveShield);
            EventBus.Instance.Subscribe(EngineEventType.ShieldFadeOut, UnactiveShield);
            EventBus.Instance.Subscribe(EngineEventType.Shield, ActiveShield);
            EventBus.Instance.Subscribe(EngineEventType.Continue, ActiveShield);
        }

        private void OnDestroy()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.ShieldFadeOutWarning, DisplayWarning);
            EventBus.Instance.UnSubscribe(EngineEventType.StartBoost, ActiveShield);
            EventBus.Instance.UnSubscribe(EngineEventType.ShieldFadeOut, UnactiveShield);
            EventBus.Instance.UnSubscribe(EngineEventType.Shield, ActiveShield);
            EventBus.Instance.UnSubscribe(EngineEventType.Continue, ActiveShield);
        }

        public void ActiveShield()
        {
            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            
            _Coroutine = StartCoroutine(ShieldCreation());
        }

        public void UnactiveShield()
        {
            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            
            _Coroutine = StartCoroutine(ShieldDissolve());
        }
        
        private IEnumerator ShieldDissolve()
        {
            _Mesh.sharedMaterial.SetFloat(_DissolvePower, 1);
            _Material.SetFloat("_Alpha", 1);
            yield return _Waiter;
            
            float power = _Mesh.sharedMaterial.GetFloat(_DissolvePower);
            while (power > 0)
            {
                power = Mathf.Max(power - _Speed * Time.deltaTime, 0);
                _Mesh.sharedMaterial.SetFloat(_DissolvePower, power);
                yield return _Waiter;
            }
        }
        
        private IEnumerator ShieldCreation()
        {
            _Material.SetFloat("_Alpha", 1);
            _Mesh.sharedMaterial.SetFloat(_DissolvePower, 0);
            float power = _Mesh.sharedMaterial.GetFloat(_DissolvePower);
            while (power < 1)
            {
                power = Mathf.Min(power + _Speed * Time.deltaTime, 1);
                _Mesh.sharedMaterial.SetFloat(_DissolvePower, power);
                yield return _Waiter;
            }
        }

        private void DisplayWarning()
        {
            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            
            _Coroutine = StartCoroutine(WarningCoroutine());
        }

        private IEnumerator WarningCoroutine()
        {
            while (true)
            {
                yield return _WarningWaiter;
                _Mesh.sharedMaterial.SetFloat(_DissolvePower, 0);
                _Material.SetFloat("_Alpha", 0);
                yield return _WarningWaiter;
                _Mesh.sharedMaterial.SetFloat(_DissolvePower, 1);
                _Material.SetFloat("_Alpha", 1);
            }
        }
    }
}
