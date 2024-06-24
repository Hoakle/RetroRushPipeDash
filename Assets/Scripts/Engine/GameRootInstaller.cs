using HoakleEngine;
using HoakleEngine.Core;
using RetroRush.Game.Level;
using UnityEngine;
using Zenject;

namespace RetroRush.Engine
{
    public class GameRootInstaller : MonoInstaller
    {
        [SerializeField] private GameRoot _GameRoot;
        [SerializeField] private Transform _GameContainer = null;
        [SerializeField] private UnityEngine.Camera _Camera;
        public override void InstallBindings()
        {
            Container.Bind<Transform>().WithId(GameRootIdentifier.GameContainer).FromInstance(_GameContainer);

            Container.Bind<GameRoot>().FromInstance(_GameRoot).AsSingle();
            Container.BindInterfacesAndSelfTo<GameEngineImpl>().AsSingle();
            Container.BindInterfacesAndSelfTo<GraphicsEngineImpl>().AsSingle();
            Container.BindInterfacesAndSelfTo<GUIEngineImpl>().AsSingle();

            Container.Bind<StaticCameraControl>().AsSingle();
            Container.Bind<CameraSettingsData>().FromInstance(new CameraSettingsData( -7f, -2f, -3f)).AsSingle();
            Container.Bind<ThirdPersonCameraControl>().AsSingle();

            Container.Bind<LevelDesignData>().AsSingle();
        }
    }
}
