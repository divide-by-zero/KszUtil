using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.Linq; // LINQ を使用するために必須

namespace KszUtil
{
    /// <summary>
    /// URP (Universal Render Pipeline) のランタイム操作に関するユーティリティクラス。
    /// RenderFeature の取得や変更をサポートします。
    /// 注意: このユーティリティは、操作対象の ScriptableRendererData のインデックスを把握している必要があります。
    /// </summary>
    public static class URPRuntimeUtils
    {
        /// <summary>
        /// 現在アクティブな UniversalRenderPipelineAsset を取得します。
        /// </summary>
        /// <returns>現在の UniversalRenderPipelineAsset。URP がアクティブでない場合は null。</returns>
        public static UniversalRenderPipelineAsset GetCurrentUrpAsset()
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null)
            {
                Debug.LogError("現在のレンダリングパイプラインはURPではありません。URPアセットが設定されているか確認してください。");
            }

            return urpAsset;
        }

        /// <summary>
        /// 指定されたURPアセットとレンダラーインデックスからScriptableRendererDataを取得します。
        /// </summary>
        /// <param name="urpAsset">対象の UniversalRenderPipelineAsset。</param>
        /// <param name="rendererDataIndex">
        /// URPアセットのrendererDataList内の、対象とするScriptableRendererDataのインデックス。
        /// </param>
        /// <returns>指定されたインデックスの ScriptableRendererData。見つからない、または不正な場合は null。</returns>
        private static ScriptableRendererData GetRendererDataAtIndex(UniversalRenderPipelineAsset urpAsset, int rendererDataIndex)
        {
            if (urpAsset == null)
            {
                // GetCurrentUrpAsset内でエラーログが出るため、ここでは重複を避ける
                return null;
            }

            // UniversalRenderPipelineAsset.rendererDataList は public ReadOnlySpan<ScriptableRendererData>
            var rendererDataList = urpAsset.rendererDataList;

            if (rendererDataIndex < 0 || rendererDataIndex >= rendererDataList.Length) // ReadOnlySpan は .Length
            {
                Debug.LogError($"指定されたレンダラーデータインデックス '{rendererDataIndex}' は不正です。URPアセット '{urpAsset.name}' の利用可能なインデックス範囲: 0 ～ {rendererDataList.Length - 1}。");
                return null;
            }

            var rendererData = rendererDataList[rendererDataIndex];
            if (rendererData == null)
            {
                Debug.LogError($"URPアセット '{urpAsset.name}' のインデックス '{rendererDataIndex}' にあるレンダラーデータがnullです。アセットの設定を確認してください。");
            }

            return rendererData;
        }

        /// <summary>
        /// 指定されたインデックスのScriptableRendererDataから、指定された型の最初のRenderFeatureを取得します。
        /// </summary>
        /// <typeparam name="TFeature">取得したいRenderFeatureの型。</typeparam>
        /// <param name="rendererDataIndex">操作対象のScriptableRendererDataの、URPアセット内でのインデックス。</param>
        /// <returns>見つかったTFeature型のRenderFeature。見つからない場合はnull。</returns>
        public static TFeature GetRenderFeature<TFeature>(int rendererDataIndex) where TFeature : ScriptableRendererFeature
        {
            var urpAsset = GetCurrentUrpAsset();
            if (urpAsset == null) return null;

            var rendererData = GetRendererDataAtIndex(urpAsset, rendererDataIndex);
            if (rendererData == null) return null; // エラーログはGetRendererDataAtIndex内で出力

            if (rendererData.rendererFeatures == null)
            {
                Debug.LogWarning($"ScriptableRendererData '{rendererData.name}' (インデックス: {rendererDataIndex}) の rendererFeatures リストがnullです。");
                return null;
            }

            return rendererData.rendererFeatures.OfType<TFeature>().FirstOrDefault();
        }

        /// <summary>
        /// 指定されたインデックスのScriptableRendererDataから、指定された型および名前を持つ最初のRenderFeatureを取得します。
        /// </summary>
        /// <typeparam name="TFeature">取得したいRenderFeatureの型。</typeparam>
        /// <param name="rendererDataIndex">操作対象のScriptableRendererDataの、URPアセット内でのインデックス。</param>
        /// <param name="featureName">検索するRenderFeatureの名前。nullまたは空文字列の場合、名前でのフィルタリングは行いません。</param>
        /// <returns>見つかったTFeature型のRenderFeature。見つからない場合はnull。</returns>
        public static TFeature GetRenderFeature<TFeature>(int rendererDataIndex, string featureName) where TFeature : ScriptableRendererFeature
        {
            var urpAsset = GetCurrentUrpAsset();
            if (urpAsset == null) return null;

            var rendererData = GetRendererDataAtIndex(urpAsset, rendererDataIndex);
            if (rendererData == null) return null;

            if (rendererData.rendererFeatures == null)
            {
                Debug.LogWarning($"ScriptableRendererData '{rendererData.name}' (インデックス: {rendererDataIndex}) の rendererFeatures リストがnullです。");
                return null;
            }

            return rendererData.rendererFeatures.OfType<TFeature>()
                .FirstOrDefault(feature => string.IsNullOrEmpty(featureName) || feature.name == featureName);
        }

        /// <summary>
        /// 指定されたインデックスのScriptableRendererDataから、指定された名前を持つ最初のRenderFeatureを取得します (型はScriptableRendererFeature)。
        /// </summary>
        /// <param name="rendererDataIndex">操作対象のScriptableRendererDataの、URPアセット内でのインデックス。</param>
        /// <param name="featureName">検索するRenderFeatureの名前。</param>
        /// <returns>見つかったScriptableRendererFeature。見つからない場合はnull。</returns>
        public static ScriptableRendererFeature GetRenderFeature(int rendererDataIndex, string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
            {
                Debug.LogError("RenderFeature 名が指定されていません。名前で検索する場合は有効な名前を指定してください。");
                return null;
            }

            var urpAsset = GetCurrentUrpAsset();
            if (urpAsset == null) return null;

            var rendererData = GetRendererDataAtIndex(urpAsset, rendererDataIndex);
            if (rendererData == null) return null;

            if (rendererData.rendererFeatures == null)
            {
                Debug.LogWarning($"ScriptableRendererData '{rendererData.name}' (インデックス: {rendererDataIndex}) の rendererFeatures リストがnullです。");
                return null;
            }

            return rendererData.rendererFeatures.FirstOrDefault(feature => feature.name == featureName);
        }

        /// <summary>
        /// 指定されたインデックスのScriptableRendererData内の、指定された型の最初のRenderFeatureに対してアクションを実行します。
        /// </summary>
        /// <typeparam name="TFeature">対象のRenderFeatureの型。</typeparam>
        /// <param name="rendererDataIndex">操作対象のScriptableRendererDataの、URPアセット内でのインデックス。</param>
        /// <param name="modificationAction">RenderFeatureに対して実行するアクション (ラムダ式またはメソッド)。</param>
        /// <returns>アクションが正常に実行された場合はtrue。RenderFeatureが見つからないか、アクションがnullの場合はfalse。</returns>
        public static bool TryModifyRenderFeature<TFeature>(int rendererDataIndex, Action<TFeature> modificationAction) where TFeature : ScriptableRendererFeature
        {
            if (modificationAction == null)
            {
                Debug.LogError("modificationAction (変更処理) が null です。実行する処理を指定してください。");
                return false;
            }

            TFeature feature = GetRenderFeature<TFeature>(rendererDataIndex);
            if (feature != null)
            {
                try
                {
                    modificationAction(feature);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"RenderFeature '{feature.name}' (型: {typeof(TFeature).Name}) の変更処理中にエラーが発生しました (対象RendererDataインデックス: {rendererDataIndex}): {ex.Message}\nスタックトレース: {ex.StackTrace}");
                    return false;
                }
            }
            else
            {
                // GetRenderFeature内で適切なログが出力されるため、ここでは簡潔に
                Debug.LogWarning($"型 '{typeof(TFeature).Name}' のRenderFeatureに対する変更処理を実行できませんでした (対象RendererDataインデックス: {rendererDataIndex})。Featureが見つかりません。");
                return false;
            }
        }

        /// <summary>
        /// 指定されたインデックスのScriptableRendererData内の、指定された型および名前を持つRenderFeatureに対してアクションを実行します。
        /// </summary>
        /// <typeparam name="TFeature">対象のRenderFeatureの型。</typeparam>
        /// <param name="rendererDataIndex">操作対象のScriptableRendererDataの、URPアセット内でのインデックス。</param>
        /// <param name="featureName">検索するRenderFeatureの名前。nullまたは空文字列の場合、名前でのフィルタリングは行いません。</param>
        /// <param name="modificationAction">RenderFeatureに対して実行するアクション (ラムダ式またはメソッド)。</param>
        /// <returns>アクションが正常に実行された場合はtrue。RenderFeatureが見つからないか、アクションがnullの場合はfalse。</returns>
        public static bool TryModifyRenderFeature<TFeature>(int rendererDataIndex, string featureName, Action<TFeature> modificationAction) where TFeature : ScriptableRendererFeature
        {
            if (modificationAction == null)
            {
                Debug.LogError("modificationAction (変更処理) が null です。実行する処理を指定してください。");
                return false;
            }

            TFeature feature = GetRenderFeature<TFeature>(rendererDataIndex, featureName);
            if (feature != null)
            {
                try
                {
                    modificationAction(feature);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"RenderFeature '{feature.name}' (型: {typeof(TFeature).Name}) の変更処理中にエラーが発生しました (対象RendererDataインデックス: {rendererDataIndex}, Feature名: {featureName}): {ex.Message}\nスタックトレース: {ex.StackTrace}");
                    return false;
                }
            }
            else
            {
                string searchCriteria = string.IsNullOrEmpty(featureName) ? $"型 '{typeof(TFeature).Name}'" : $"名前 '{featureName}' (型: '{typeof(TFeature).Name}')";
                Debug.LogWarning($"{searchCriteria} のRenderFeatureに対する変更処理を実行できませんでした (対象RendererDataインデックス: {rendererDataIndex})。Featureが見つかりません。");
                return false;
            }
        }
    }
}