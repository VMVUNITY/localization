using System;
using Localization.Enums;
using Localization.ResourceProviders;
using UnityEngine;
using Zenject;

namespace Localization.Installers {
    public class LocalizationInstaller : MonoInstaller<LocalizationInstaller> {
        public LocalizationResourceLocation location;

        public override void InstallBindings() {
            InstallProvidersForLocation();

            Container.BindInterfacesAndSelfTo<LocalizationController>().AsSingle().NonLazy();
        }

        private void InstallProvidersForLocation() {
            switch (location) {
                case LocalizationResourceLocation.Addressables:
                    Container.BindInterfacesAndSelfTo<AddressablesResourceProvider<string, TextAsset>>()
                        .AsSingle()
                        .WithConcreteId("locTextProvider")
                        .WhenInjectedInto<LocalizationController>();

                    Container.BindInterfacesAndSelfTo<AddressablesResourceProvider<string, Sprite>>()
                        .AsSingle()
                        .WithConcreteId("locSpriteProvider")
                        .WhenInjectedInto<LocalizationController>();
                    break;
                case LocalizationResourceLocation.ResourcesFolder:
                    Container.BindInterfacesAndSelfTo<DefaultResourceProvider<TextAsset>>()
                        .AsSingle()
                        .WithConcreteId("locTextProvider")
                        .WhenInjectedInto<LocalizationController>();

                    Container.Bind<string>().FromInstance("LocalizationAtlas")
                        .AsSingle()
                        .WhenInjectedInto<SpriteAtlasResourceProvider>();

                    Container.BindInterfacesAndSelfTo<SpriteAtlasResourceProvider>()
                        .AsSingle()
                        .WithConcreteId("locSpriteProvider")
                        .WhenInjectedInto<LocalizationController>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}