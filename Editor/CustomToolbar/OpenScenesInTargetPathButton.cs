using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KszUtil.Editor.CustomToolbar
{
    public static class OpenScenesInTargetPathButton
    {
        private static readonly string TargetPath = "Assets/Scenes";

        [MainToolbarElement(
            "HighKing/OpenAllScenes",
            defaultDockPosition = MainToolbarDockPosition.Middle,
            defaultDockIndex = 200
        )]
        public static MainToolbarElement CreateOpenAllButton()
        {
            var icon = (Texture2D)EditorGUIUtility.Load("d_SceneAsset Icon");
            var content = new MainToolbarContent(icon, tooltip: $"Open all scenes in \"{TargetPath}\".");
            return new MainToolbarButton(content, OpenAllScenes);
        }

        [MainToolbarElement(
            "HighKing/SceneDropdown",
            defaultDockPosition = MainToolbarDockPosition.Middle,
            defaultDockIndex = 201
        )]
        public static MainToolbarElement CreateDropdownButton()
        {
            var icon = (Texture2D)EditorGUIUtility.Load("d_dropdown@2x");
            var content = new MainToolbarContent(icon, tooltip: "Scene operations.");
            return new MainToolbarButton(content, ShowSceneMenu);
        }

        private static IEnumerable<string> GetTargetScenePaths()
        {
            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(Scene)}", new[] { TargetPath }))
            {
                yield return AssetDatabase.GUIDToAssetPath(guid);
            }
        }

        private static void OpenAllScenes()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            var targetScenePaths = GetTargetScenePaths();
            var setups = EditorSceneManager.GetSceneManagerSetup();

            foreach (var scenePath in targetScenePaths)
            {
                var setup = setups.FirstOrDefault(s => s.path == scenePath);
                if (setup != null) continue;

                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.AdditiveWithoutLoading);
            }
        }

        private static void ShowSceneMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            var scenePaths = GetTargetScenePaths();
            var setups = EditorSceneManager.GetSceneManagerSetup();
            var menu = new GenericMenu();

            foreach (var scenePath in scenePaths)
            {
                var sceneName = Path.GetFileName(scenePath);
                {
                    var content = new GUIContent($"{sceneName}/Open");

                    menu.AddItem(
                        content,
                        false,
                        () =>
                        {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false) return;

                            EditorSceneManager.OpenScene(scenePath);
                        }
                    );
                }
                {
                    var setup = setups.FirstOrDefault(s => s.path == scenePath);
                    var content = new GUIContent($"{sceneName}/Add");

                    if (setup == null)
                    {
                        menu.AddItem(
                            content,
                            false,
                            () => EditorSceneManager.OpenScene(scenePath, OpenSceneMode.AdditiveWithoutLoading)
                        );
                    }
                    else
                    {
                        menu.AddDisabledItem(content);
                    }
                }

                menu.AddSeparator($"{sceneName}/");

                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));
                    var content = new GUIContent($"{sceneName}/Ping");

                    menu.AddItem(
                        content,
                        false,
                        () => EditorGUIUtility.PingObject(sceneAsset)
                    );
                }
            }

            menu.AddSeparator("");

            {
                var content = new GUIContent("Clear");

                if (setups.Length <= 1)
                {
                    menu.AddDisabledItem(content);
                }
                else
                {
                    menu.AddItem(
                        content,
                        false,
                        () =>
                        {
                            var targetScenes = setups
                                .Where(s => s.isActive == false)
                                .Select(s => UnityEngine.SceneManagement.SceneManager.GetSceneByPath(s.path))
                                .ToArray();

                            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(targetScenes) == false) return;

                            foreach (var scene in targetScenes)
                            {
                                EditorSceneManager.CloseScene(scene, true);
                            }
                        }
                    );
                }
            }

            menu.ShowAsContext();
        }
    }
}