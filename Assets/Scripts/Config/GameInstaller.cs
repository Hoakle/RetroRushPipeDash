using System;
using HoakleEngine;
using HoakleEngine.Core.Config.Ads;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using HoakleEngine.Core.Services.AdsServices;
using HoakleEngine.Core.Services.GameSaveService;
using HoakleEngine.Core.Services.MiscService;
using HoakleEngine.Core.Services.PlayServices;
using RetroRush.Config;
using RetroRush.Game.Economics;
using RetroRush.Game.Gameplay;
using RetroRush.GameData;
using RetroRush.GameSave;
using UnityEngine;
using Zenject;

namespace RetroRush
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameplayConfigData _GameplayConfigData;
        [SerializeField] private LevelConfigData _LevelConfigData;
        [SerializeField] private AdsServicesConfigData _AdsServicesConfigData;
        [SerializeField] private LocalizationDataBase _LocalizationDataBase;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CameraProvider>().AsSingle();
            
            InstallConfig();
            InstallGameSave();
            InstallServices();

            Container.BindInterfacesTo<LocalizationProvider>().FromSubContainerResolve().ByMethod(subContaienr =>
            {
                subContaienr.Install<LocalizationInstaller>();
                subContaienr.BindInstance(_LocalizationDataBase).AsSingle();
            }).AsSingle().NonLazy();
        }

        private void InstallConfig()
        {
            Container.Bind<GameplayConfigData>().FromInstance(_GameplayConfigData).AsSingle();
            Container.Bind<LevelConfigData>().FromInstance(_LevelConfigData).AsSingle();
            Container.Bind<AdsServicesConfigData>().FromInstance(_AdsServicesConfigData).AsSingle();
            Container.Bind<UpgradeConfigData>()
                .WithId(GameIdentifier.MagnetConfig)
                .FromResolveGetter<GameplayConfigData>(config => config.GetUpgradeConfig(PickableType.Magnet));
            Container.Bind<UpgradeConfigData>()
                .WithId(GameIdentifier.ShieldConfig)
                .FromResolveGetter<GameplayConfigData>(config => config.GetUpgradeConfig(PickableType.Shield));
            Container.Bind<UpgradeConfigData>()
                .WithId(GameIdentifier.CoinFactorConfig)
                .FromResolveGetter<GameplayConfigData>(config => config.GetUpgradeConfig(PickableType.CoinFactor));
            Container.Bind<UpgradeConfigData>()
                .WithId(GameIdentifier.SpeedBoostConfig)
                .FromResolveGetter<GameplayConfigData>(config => config.GetUpgradeConfig(PickableType.SpeedBoost));
            Container.Bind<UpgradeConfigData>()
                .WithId(GameIdentifier.StartBoostConfig)
                .FromResolveGetter<GameplayConfigData>(config => config.GetUpgradeConfig(PickableType.StartBoost));
        }

        private void InstallGameSave()
        {
            InitGameSave();
            InitCurrencyHandler();

            Container.BindInterfacesAndSelfTo<GlobalGameSave>().AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsGameSave>().AsSingle();
            Container.BindInterfacesAndSelfTo<ProgressionHandler>().AsSingle().WithArguments("ProgressionHandler");
            Container.Bind<UpgradeData>()
                .WithId(GameIdentifier.MagnetData)
                .FromResolveGetter<GlobalGameSave>(save => save.GetUpgrade(PickableType.Magnet));
            Container.Bind<UpgradeData>()
                .WithId(GameIdentifier.ShieldData)
                .FromResolveGetter<GlobalGameSave>(save => save.GetUpgrade(PickableType.Shield));
            Container.Bind<UpgradeData>()
                .WithId(GameIdentifier.CoinFactorData)
                .FromResolveGetter<GlobalGameSave>(save => save.GetUpgrade(PickableType.CoinFactor));
            Container.Bind<UpgradeData>()
                .WithId(GameIdentifier.SpeedBoostData)
                .FromResolveGetter<GlobalGameSave>(save => save.GetUpgrade(PickableType.SpeedBoost));
            Container.Bind<UpgradeData>()
                .WithId(GameIdentifier.StartBoostData)
                .FromResolveGetter<GlobalGameSave>(save => save.GetUpgrade(PickableType.StartBoost));
        }
        
        protected void InitGameSave()
        {
            Container.BindInterfacesTo<PlayerPrefService>().AsSingle();
        }

        private void InitCurrencyHandler()
        {
            foreach (var type in Enum.GetValues(typeof(CurrencyType)))
            {
                Container.Bind<CurrencyHandler>()
                    .WithId((CurrencyType)type)
                    .AsSingle()
                    .WithArguments((CurrencyType)type)
                    .OnInstantiated<CurrencyHandler>((container, obj) => 
                        container.Container.Resolve<InitializableManager>()
                            .Add(obj)
                    );
            }
        }

        private void InstallServices()
        {
            Container.BindInterfacesTo<PlayServicesTP>().AsSingle();
            Container.BindInterfacesAndSelfTo<AdsThirdPartyService>().AsSingle();
            Container.BindInterfacesAndSelfTo<MiscThirdPartyService>().AsSingle();
        }
    }

    public static class GameIdentifier
    {
        public const string MagnetConfig = "GameplayConfig.Upgrade.Magnet";
        public const string ShieldConfig = "GameplayConfig.Upgrade.Shield";
        public const string CoinFactorConfig = "GameplayConfig.Upgrade.CoinFactor";
        public const string SpeedBoostConfig = "GameplayConfig.Upgrade.SpeedBoost";
        public const string StartBoostConfig = "GameplayConfig.Upgrade.StartBoost";

        public const string MagnetData = "GameSave.Upgrade.Magnet";
        public const string ShieldData = "GameSave.Upgrade.Shield";
        public const string CoinFactorData = "GameSave.Upgrade.CoinFactor";
        public const string SpeedBoostData = "GameSave.Upgrade.SpeedBoost";
        public const string StartBoostData = "GameSave.Upgrade.StartBoost";
    }
}
