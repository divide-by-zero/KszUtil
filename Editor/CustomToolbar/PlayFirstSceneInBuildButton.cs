using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace KszUtil.Editor.CustomToolbar
{
    public static class PlayFirstSceneInBuildButton
    {
        private const string MenuItemName = "Edit/Play First Scene %@";
        private const int MenuItemPriority = 159;
        private const string HierarchyIndexOfTargetSceneIntKey = "HierarchyIndexOfTargetScene";
        private const string ShouldResetPlayModeStartSceneBoolKey = "ShouldResetPlayModeStartScene";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.playModeStartScene = null;
        }

        [MainToolbarElement(
            "HighKing/PlayFirstScene",
            defaultDockPosition = MainToolbarDockPosition.Middle,
            defaultDockIndex = 100
        )]
        public static MainToolbarElement CreateButton()
        {
            var icon = (Texture2D)EditorGUIUtility.Load("d_PlayButton@2x");
            var content = new MainToolbarContent(icon, tooltip: "Play the first scene in build.");
            return new MainToolbarButton(content, PlayFirstSceneInBuild);
        }

        private static string GetFirstScenePath()
        {
            return EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled)?.path;
        }

        [MenuItem(MenuItemName, true)]
        private static bool Validate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return false;
            if (string.IsNullOrEmpty(GetFirstScenePath())) return false;

            return true;
        }

        [MenuItem(MenuItemName, false, MenuItemPriority)]
        private static void PlayFirstSceneInBuild()
        {
            if (Validate() == false) return;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false) return;

            var firstScenePath = GetFirstScenePath();
            var firstSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScenePath);
            var sceneSetups = EditorSceneManager.GetSceneManagerSetup();
            var firstSceneSetup = sceneSetups.FirstOrDefault(s => s.path == firstScenePath);

            // NOTE: 対象のシーンがHierarchyに存在＆ロードされていない場合は閉じる（その状態でHierarchyに存在しているとうまく動かない）
            if ((firstSceneSetup != null) && (firstSceneSetup?.isLoaded == false))
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(firstScenePath);
                EditorSceneManager.CloseScene(scene, true);
                var hierarchyIndex = sceneSetups.ToList().IndexOf(firstSceneSetup);

                // NOTE: 再生終了後に対象のシーンをHierarchyに復元するためにインデックスを保存
                SessionState.SetInt(HierarchyIndexOfTargetSceneIntKey, hierarchyIndex);
            }

            SessionState.SetBool(ShouldResetPlayModeStartSceneBoolKey, true);
            EditorSceneManager.playModeStartScene = firstSceneAsset;
            EditorApplication.isPlaying = true;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode) return;

            if (SessionState.GetBool(ShouldResetPlayModeStartSceneBoolKey, false))
            {
                SessionState.EraseBool(ShouldResetPlayModeStartSceneBoolKey);
                EditorSceneManager.playModeStartScene = null;
            }

            // NOTE: 必要であれば対象のシーンをHierarchyに復元（特別な場合を除き自動で復元される）
            var hierarchyIndex = SessionState.GetInt(HierarchyIndexOfTargetSceneIntKey, -1);
            if (hierarchyIndex < 0) return;

            SessionState.EraseInt(HierarchyIndexOfTargetSceneIntKey);

            var firstScenePath = GetFirstScenePath();
            var firstScene = EditorSceneManager.OpenScene(firstScenePath, OpenSceneMode.AdditiveWithoutLoading);
            var destScenePath = EditorSceneManager.GetSceneManagerSetup()[hierarchyIndex].path;
            var destScene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(destScenePath);

            EditorSceneManager.MoveSceneBefore(firstScene, destScene);
        }
    }
}