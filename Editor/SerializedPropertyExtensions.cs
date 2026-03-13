using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace KszUtil.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static string ToLabelString(this SerializedProperty serializedProperty, FieldInfo fieldInfo)
        {
            switch (serializedProperty.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return "";
                case SerializedPropertyType.Integer:
                    return serializedProperty.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return serializedProperty.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return serializedProperty.floatValue.ToString(CultureInfo.InvariantCulture);
                case SerializedPropertyType.String:
                    return serializedProperty.stringValue;
                case SerializedPropertyType.Color:
                    return serializedProperty.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return serializedProperty.objectReferenceValue.ToString();
                case SerializedPropertyType.LayerMask:
                    return "";
                case SerializedPropertyType.Enum:
                    string enumName = serializedProperty.enumNames.ElementAt(serializedProperty.enumValueIndex);

                    if (TryGetEnumDescription(fieldInfo, enumName, out var description))
                    {
                        return description;
                    }
                    else
                    {
                        return enumName;
                    }
                case SerializedPropertyType.Vector2:
                    return serializedProperty.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return serializedProperty.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return serializedProperty.vector4Value.ToString();
                case SerializedPropertyType.Rect:
                    return serializedProperty.rectValue.ToString();
                case SerializedPropertyType.ArraySize:
                    return serializedProperty.arraySize.ToString();
                case SerializedPropertyType.Character:
                    return "";
                case SerializedPropertyType.AnimationCurve:
                    return serializedProperty.animationCurveValue.ToString();
                case SerializedPropertyType.Bounds:
                    return serializedProperty.boundsValue.ToString();
                case SerializedPropertyType.Gradient:
                    return "";
                case SerializedPropertyType.Quaternion:
                    return serializedProperty.quaternionValue.ToString();
                case SerializedPropertyType.ExposedReference:
                    return "";
                case SerializedPropertyType.FixedBufferSize:
                    return serializedProperty.fixedBufferSize.ToString();
                case SerializedPropertyType.Vector2Int:
                    return serializedProperty.vector2IntValue.ToString();
                case SerializedPropertyType.Vector3Int:
                    return serializedProperty.vector3IntValue.ToString();
                case SerializedPropertyType.RectInt:
                    return serializedProperty.rectIntValue.ToString();
                case SerializedPropertyType.BoundsInt:
                    return serializedProperty.boundsIntValue.ToString();
                case SerializedPropertyType.ManagedReference:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// SerializedProperty から FieldInfo を取得する
        /// </summary>
        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            FieldInfo GetField(Type type, string path)
            {
                return type.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            }

            var splits = property.propertyPath.Split('.');
            var currentType = property.serializedObject.targetObject.GetType();
            var fieldInfo = default(FieldInfo);
            for (var i = 0; i < splits.Length; i++)
            {
                if (splits[i] == "Array")
                {
                    i += 2;
                    currentType = currentType.IsArray ? currentType.GetElementType() : currentType.GetGenericArguments()[0];
                }

                fieldInfo = GetField(currentType, splits[i]);
                currentType = fieldInfo.FieldType;
            }

            return fieldInfo;
        }

        /// <summary>
        /// Enumの[LabelText]Attributeの設定を取得する
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="enumName"></param>
        /// <param name="labelText"></param>
        /// <returns></returns>
        private static bool TryGetEnumDescription(FieldInfo fieldInfo, string enumName, out string labelText)
        {
            labelText = string.Empty;

            var valueType = fieldInfo.FieldType;
            if (valueType.IsGenericType == false)
            {
                return false;
            }

            var collectionGenericType = valueType.GetGenericArguments();
            if (collectionGenericType.Length <= 0)
            {
                return false;
            }

            var keyValuePairGenericType = collectionGenericType[0].GetGenericArguments();
            if (keyValuePairGenericType.Length <= 0)
            {
                return false;
            }

            var enumType = keyValuePairGenericType[0];
            var enumFieldInfo = enumType.GetField(enumName);
            // var attributes = (LabelTextAttribute[])enumFieldInfo.GetCustomAttributes(typeof(LabelTextAttribute), false);
            // labelText = attributes.Select(n => n.LabelText).FirstOrDefault();
            //
            // return !string.IsNullOrEmpty(labelText);
            return false;
        }
    }
}