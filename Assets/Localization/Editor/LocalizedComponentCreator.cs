using Localization.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Localization.Editor {
    public class LocalizedComponentCreator {
        private const string CREATED_TEXT_NAME = "LocalizedText(TMP)";
        private const string CREATED_IMAGE_NAME = "LocalizedImage";
        private const string CREATED_BUTTON_NAME = "Button(Localized)";

        [MenuItem("GameObject/UI/Localization/LocalizedText(TMP)")]
        private static void CreateLocalizedText(MenuCommand menuCommand) {
            var createdGameObject = new GameObject(CREATED_TEXT_NAME);

            var textComponent = createdGameObject.AddComponent<LocalizedText>();

            textComponent.rectTransform.anchorMin = Vector2.zero;
            textComponent.rectTransform.anchorMax = Vector2.zero;

            var autoInjecter = createdGameObject.AddComponent<ZenAutoInjecter>();
            autoInjecter.ContainerSource = ZenAutoInjecter.ContainerSources.ProjectContext;

            GameObjectUtility.SetParentAndAlign(createdGameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(createdGameObject, $"Create " + createdGameObject.name);

            Selection.activeGameObject = createdGameObject;
        }

        [MenuItem("GameObject/UI/Localization/LocalizedText(TMP)_Optimised")]
        private static void CreateOptimisedLocalizedText(MenuCommand menuCommand) {
            var createdGameObject = new GameObject(CREATED_TEXT_NAME);

            var textComponent = createdGameObject.AddComponent<LocalizedText>();

            textComponent.rectTransform.anchorMin = Vector2.zero;
            textComponent.rectTransform.anchorMax = Vector2.zero;

            textComponent.raycastTarget = false;
            textComponent.maskable = false;

            var autoInjecter = createdGameObject.AddComponent<ZenAutoInjecter>();
            autoInjecter.ContainerSource = ZenAutoInjecter.ContainerSources.ProjectContext;

            GameObjectUtility.SetParentAndAlign(createdGameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(createdGameObject, $"Create " + createdGameObject.name);

            Selection.activeGameObject = createdGameObject;
        }

        [MenuItem("GameObject/UI/Localization/LocalizedImage")]
        private static void CreateLocalizedImage(MenuCommand menuCommand) {
            var createdGameObject = new GameObject(CREATED_IMAGE_NAME);

            createdGameObject.AddComponent<LocalizedImage>();

            var autoInjecter = createdGameObject.AddComponent<ZenAutoInjecter>();
            autoInjecter.ContainerSource = ZenAutoInjecter.ContainerSources.ProjectContext;

            GameObjectUtility.SetParentAndAlign(createdGameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(createdGameObject, $"Create " + createdGameObject.name);

            Selection.activeGameObject = createdGameObject;
        }

        [MenuItem("GameObject/UI/Localization/LocalizedImage(Optimised)")]
        private static void CreateOptimisedLocalizedImage(MenuCommand menuCommand) {
            var createdGameObject = new GameObject(CREATED_IMAGE_NAME);

            var textComponent = createdGameObject.AddComponent<LocalizedImage>();

            textComponent.raycastTarget = false;
            textComponent.maskable = false;

            var autoInjecter = createdGameObject.AddComponent<ZenAutoInjecter>();
            autoInjecter.ContainerSource = ZenAutoInjecter.ContainerSources.ProjectContext;

            GameObjectUtility.SetParentAndAlign(createdGameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(createdGameObject, $"Create " + createdGameObject.name);

            Selection.activeGameObject = createdGameObject;
        }

        [MenuItem("GameObject/UI/Localization/Localized Button Text")]
        private static void CreateLocalizedButton(MenuCommand menuCommand) {
            var createdGameObject = new GameObject(CREATED_BUTTON_NAME);

            var imageComponent = createdGameObject.AddComponent<Image>();
            var buttonComponent = createdGameObject.AddComponent<Button>();

            var textComponent = new GameObject("Text").AddComponent<LocalizedText>();

            textComponent.transform.SetParent(createdGameObject.transform);

            textComponent.rectTransform.anchoredPosition = Vector2.zero;

            buttonComponent.targetGraphic = imageComponent;

            var autoInjecter = textComponent.gameObject.AddComponent<ZenAutoInjecter>();
            autoInjecter.ContainerSource = ZenAutoInjecter.ContainerSources.ProjectContext;

            GameObjectUtility.SetParentAndAlign(createdGameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(createdGameObject, $"Create " + createdGameObject.name);

            Selection.activeGameObject = createdGameObject;
        }

        [MenuItem("GameObject/UI/Localization/Localized Button Image")]
        private static void CreateLocalizedButtonImage(MenuCommand menuCommand) {
            var createdGameObject = new GameObject(CREATED_BUTTON_NAME);

            var imageComponent = createdGameObject.AddComponent<LocalizedImage>();
            var buttonComponent = createdGameObject.AddComponent<Button>();

            buttonComponent.targetGraphic = imageComponent;

            var autoInjecter = imageComponent.gameObject.AddComponent<ZenAutoInjecter>();
            autoInjecter.ContainerSource = ZenAutoInjecter.ContainerSources.ProjectContext;

            GameObjectUtility.SetParentAndAlign(createdGameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(createdGameObject, $"Create " + createdGameObject.name);

            Selection.activeGameObject = createdGameObject;
        }
    }
}