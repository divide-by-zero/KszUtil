using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KszUtil.Editor
{
    [CustomPropertyDrawer(typeof(SerializableKeyValuePair<,>), true)]
    public sealed class SerializableKeyValuePairPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private const string RegexPattern = "\\.keyValuePairs\\.Array\\.data\\[\\d+\\]$";

        private bool? isKeyEditable = default;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keySerializedProperty = property.FindPropertyRelative("key");
            var valueSerializedProperty = property.FindPropertyRelative("value");

            if (valueSerializedProperty.propertyType == SerializedPropertyType.Generic)
                // Value が class や struct で、AssetReference 系ではない場合、Foldout させる
            {
                var labelContent = GUIContent.none;
                if (!DrawKey(true))
                {
                    labelContent = new GUIContent(keySerializedProperty.ToLabelString(this.fieldInfo));
                }

                valueSerializedProperty.isExpanded = EditorGUI.Foldout(
                    new Rect(position)
                    {
                        height = EditorGUIUtility.singleLineHeight,
                    },
                    valueSerializedProperty.isExpanded,
                    labelContent,
                    true
                );
                EditorGUI.PropertyField(
                    new Rect(position),
                    valueSerializedProperty,
                    GUIContent.none,
                    true
                );
            }
            else
                // Value が単一の値の場合、ラベルの右に PropertyField を表示する
            {
                DrawKey();

                var indentedRect = EditorGUI.IndentedRect(position);
                var indentedWidth = indentedRect.x - position.x;
                var x = position.x + EditorGUIUtility.labelWidth - indentedWidth + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(
                    new Rect(position)
                    {
                        x = x,
                        width = EditorGUIUtility.currentViewWidth - x - EditorStyles.inspectorDefaultMargins.padding.left - EditorGUIUtility.standardVerticalSpacing,
                    },
                    valueSerializedProperty,
                    GUIContent.none,
                    true
                );
            }

            property.serializedObject.ApplyModifiedProperties();

            bool DrawKey(bool skipDrawLabelField = false)
            {
                if (!IsKeyEditable(property))
                {
                    if (skipDrawLabelField)
                    {
                        return false;
                    }

                    EditorGUI.LabelField(
                        new Rect(position)
                        {
                            width = EditorGUIUtility.labelWidth,
                        },
                        keySerializedProperty.ToLabelString(this.fieldInfo)
                    );
                    return false;
                }

                EditorGUI.PropertyField(
                    new Rect(position)
                    {
                        width = EditorGUIUtility.labelWidth,
                    },
                    keySerializedProperty,
                    GUIContent.none,
                    true
                );
                return true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
        }

        private bool IsKeyEditable(SerializedProperty property)
        {
            if (!isKeyEditable.HasValue)
            {
                // Key が Editable かどうかの判定は propertyPath の末尾から .keyValuePairs.Array.data[0] を取り除いた Path を
                // 親の SerializedObject から探して、それの型が SerializableDictionary<,> であるかどうか
                isKeyEditable =
                    property.serializedObject.FindProperty(Regex.Replace(property.propertyPath, RegexPattern, string.Empty)).type
                    ==
                    typeof(SerializableDictionary<,>).Name;
            }

            return isKeyEditable.Value;
        }
    }
}