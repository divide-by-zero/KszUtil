using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace KszUtil
{
    public class MyUtil
    {
        /// <summary>
        /// aとbを入れ替え
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Swap<T>(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        //渡された重み付け配列からIndexを得る
        public static int GetRandomIndex(params int[] weightTable)
        {
            var totalWeight = weightTable.Sum();
            var value = Random.Range(1, totalWeight + 1);
            var retIndex = -1;
            for (var i = 0; i < weightTable.Length; ++i)
            {
                if (weightTable[i] >= value)
                {
                    retIndex = i;
                    break;
                }

                value -= weightTable[i];
            }

            return retIndex;
        }

        private static float sTime;

        public static void TimeMeasureStart()
        {
            sTime = Time.realtimeSinceStartup;
        }

        public static float TimeMeasureEnd(bool isDebugLog = false)
        {
            var t = sTime;
            sTime = Time.realtimeSinceStartup;
            if (isDebugLog)
            {
                Debug.Log("time:" + (sTime - t));
                sTime = Time.realtimeSinceStartup;
            }

            return sTime - t;
        }

        /// <summary>
        /// TimeFact秒で値をmag倍にするための内分点を返却
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="TimeFact"></param>
        /// <returns></returns>
        public static float DeltaT(float mag = 0.1f, float TimeFact = 0.5f)
        {
            return 1 - Mathf.Pow(mag, Time.deltaTime / TimeFact);
        }

        public static IDisposable SlowMotion(float speed, float inDuration, float outDuration)
        {
            var oldSpeed = Time.timeScale;
            var tweener = DOVirtual.Float(oldSpeed, speed, inDuration, value => Time.timeScale = value);

            return Disposable.Create(() =>
            {
                tweener.Kill();
                DOVirtual.Float(Time.timeScale, oldSpeed, outDuration, value => Time.timeScale = value);
            });
        }

        //角度を-180～180に正規化
        public static float NormalizeAngle(float angle)
        {
            return (angle + 180) % 360 - 180;
        }
    }

    public static class LinqExtensions
    {
        public static T RandomAt<T>(this IEnumerable<T> ie)
        {
            var cnt = ie?.Count();
            if (cnt.HasValue == false) return default(T);
            return ie.ElementAt(Random.Range(0, cnt.Value));
        }

        public static T RandomAt<T>(this IEnumerable<T> ie, IEnumerable<T> exclude)
        {
            if (ie.Any() == false) return default(T);
            var exceptList = ie.Except(exclude);
            return exceptList.ElementAt(Random.Range(0, exceptList.Count()));
        }

        public static int FirstIndex<T>(this IEnumerable<T> ie, Func<T, bool> predicateFunc)
        {
            return ie.Select((tData, index) => new
            {
                tData,
                index
            }).First(arg => predicateFunc(arg.tData)).index;
        }

        public static int? FirstIndexOrNull<T>(this IEnumerable<T> ie, Func<T, bool> predicateFunc)
        {
            return ie.Select((tData, index) => new
            {
                tData,
                index
            }).FirstOrDefault(arg => predicateFunc(arg.tData))?.index;
        }
    }

    public static class ColorExtensions
    {
        public static Color ToDark(this Color col, float mag)
        {
            return new Color(Mathf.Clamp(col.r * mag, 0, 1), Mathf.Clamp(col.g * mag, 0, 1), Mathf.Clamp(col.b * mag, 0, 1));
        }

        public static Color ToAlphaDisable(this Color col)
        {
            return new Color(col.r, col.g, col.b);
        }

        public static Color SetAlpha(this Color col, float alpha)
        {
            return new Color(col.r, col.g, col.b, alpha);
        }

        /// <summary>
        /// 渡された色が背景として、見やすい色(白or黒)を返却する
        /// </summary>
        /// <param name="bgColor"></param>
        /// <returns>見やすい色</returns>
        public static Color GetEnableColor(this Color bgColor)
        {
            var grayScale = (bgColor.r * 299 + bgColor.g * 587 + bgColor.b * 114) / 1000.0f;
            return grayScale > 0.5f ? Color.black : Color.white;
        }
    }

    public static class AlphaExtension
    {
        public static void SetAlpha(this Graphic src, float alpha)
        {
            var color = src.color;
            color.a = alpha;
            src.color = color;
        }
    }

    public static class DictionaryExtensions
    {
        /// <summary>
        /// 値を取得、keyがなければデフォルト値を設定し、デフォルト値を取得
        /// </summary>
        public static TV GetOrDefault<TK, TV>(this Dictionary<TK, TV> dic, TK key, TV defaultValue = default(TV))
        {
            TV result;
            return dic.TryGetValue(key, out result) ? result : defaultValue;
        }
    }

    public static class StackExtensions
    {
        /// <summary>
        /// 値を取得、keyがなければデフォルト値を設定し、デフォルト値を取得
        /// </summary>
        public static TV PeekOrDefault<TV>(this Stack<TV> stack, TV defaultValue = default(TV))
        {
            if (stack.Count == 0) return defaultValue;
            return stack.Peek();
        }

        public static TV PopOrDefault<TV>(this Stack<TV> stack, TV defaultValue = default(TV))
        {
            if (stack.Count == 0) return defaultValue;
            return stack.Pop();
        }
    }

    public static class ImageExtension
    {
        public static Image SetColor(this Image image, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            var c = image.color;
            if (r.HasValue) c.r = r.Value;
            if (g.HasValue) c.g = g.Value;
            if (b.HasValue) c.b = b.Value;
            if (a.HasValue) c.a = a.Value;
            image.color = c;
            return image;
        }

        public static Image AddColor(this Image image, float r = 0.0f, float g = 0.0f, float b = 0.0f, float a = 0.0f)
        {
            var c = image.color;
            c.r += r;
            c.g += g;
            c.b += b;
            c.a += a;
            image.color = c;
            return image;
        }

        public static SpriteRenderer SetColor(this SpriteRenderer sprite, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            var c = sprite.color;
            if (r.HasValue) c.r = r.Value;
            if (g.HasValue) c.g = g.Value;
            if (b.HasValue) c.b = b.Value;
            if (a.HasValue) c.a = a.Value;
            sprite.color = c;
            return sprite;
        }

        public static SpriteRenderer AddColor(this SpriteRenderer sprite, float r = 0.0f, float g = 0.0f, float b = 0.0f, float a = 0.0f)
        {
            var c = sprite.color;
            c.r += r;
            c.g += g;
            c.b += b;
            c.a += a;
            sprite.color = c;
            return sprite;
        }
    }

    public static class ObjectUtil
    {
        public static int ParseInt(this object value, int defaultValue = default(int))
        {
            return int.TryParse(value.ToString(), out var result) ? result : defaultValue;
        }

        public static long ParseLong(this object value, long defaultValue = default)
        {
            return long.TryParse(value.ToString(), out var result) ? result : defaultValue;
        }

        public static float ParseFloat(this object value, float defaultValue = default)
        {
            return float.TryParse(value.ToString(), out var result) ? result : defaultValue;
        }

        public static T ParseEnum<T>(this object value, T defaultValue = default(T)) where T : struct
        {
            return Enum.TryParse(value.ToString(), out T result) ? result : defaultValue;
        }

        public static int? ParseIntNullable(this object value)
        {
            return int.TryParse(value.ToString(), out var result) ? (int?)result : null;
        }

        public static bool ParseBool(this object value, bool defaultValue = default(bool))
        {
            //0がfalse 1がtrueという風潮があるので
            var i = ParseIntNullable(value);
            if (i.HasValue) return i != 0;
            return bool.TryParse(value.ToString(), out var result) ? result : defaultValue;
        }

        public static TimeSpan ParseTime(this object value, TimeSpan defaultValue = default(TimeSpan))
        {
            return TimeSpan.TryParse(value.ToString(), out var result) ? result : defaultValue;
        }

        public static TimeSpan? ParseTimeNullable(this object value)
        {
            if (value == "0") return null; //0 だけ入っている場合は時刻じゃない
            return TimeSpan.TryParse(value.ToString(), out var result) ? (TimeSpan?)result : null;
        }

        public static DateTime ParseDate(this object value, DateTime defaultValue = default(DateTime))
        {
            return DateTime.TryParse(value.ToString(), out var result) ? result : defaultValue;
        }

        public static DateTime ParseExactDate(this string value, string format, DateTime defaultValue = default(DateTime))
        {
            return DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result) ? result : defaultValue;
        }

        public static DateTime? ParseExactDateNullable(this string value, string format)
        {
            return DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result) ? (DateTime?)result : null;
        }

        public static int ParseInt<T>(this Dictionary<T, object> dic, T key, int defaultValue = default(int))
        {
            return dic.ContainsKey(key) ? ParseInt(dic[key], defaultValue) : defaultValue;
        }

        public static long ParseLong<T>(this Dictionary<T, object> dic, T key, long defaultValue = default)
        {
            return dic.ContainsKey(key) ? ParseLong(dic[key], defaultValue) : defaultValue;
        }

        public static float ParseFloat<T>(this Dictionary<T, object> dic, T key, float defaultValue = default)
        {
            return dic.ContainsKey(key) ? ParseFloat(dic[key], defaultValue) : defaultValue;
        }

        public static int? ParseIntNullable<T>(this Dictionary<T, object> dic, T key)
        {
            return dic.ContainsKey(key) ? ParseIntNullable(dic[key]) : null;
        }

        public static bool ParseBool<T>(this Dictionary<T, object> dic, T key, bool defaultValue = default(bool))
        {
            return dic.ContainsKey(key) ? ParseBool(dic[key], defaultValue) : defaultValue;
        }

        public static TimeSpan ParseTime<T>(this Dictionary<T, object> dic, T key, TimeSpan defaultValue = default(TimeSpan))
        {
            return dic.ContainsKey(key) ? ParseTime(dic[key], defaultValue) : defaultValue;
        }

        public static TimeSpan? ParseTimeNullable<T>(this Dictionary<T, object> dic, T key)
        {
            return dic.ContainsKey(key) ? dic.ParseString(key).ParseTimeNullable() : null;
        }

        public static DateTime ParseDate<T>(this Dictionary<T, object> dic, T key, DateTime defaultValue = default(DateTime))
        {
            return !dic.ContainsKey(key) ? defaultValue : ParseDate(dic[key], defaultValue);
        }

        public static string ParseString<T>(this Dictionary<T, object> dic, T key, string defaultValue = default(string))
        {
            return dic.ContainsKey(key) ? dic[key].ToString() : defaultValue;
        }
    }

    public static class CommonUtil
    {
        public static int ParseInt(this string value, int defaultValue = default(int))
        {
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        public static Color ParseColor(this string value, Color defaultValue = default(Color))
        {
            return ColorUtility.TryParseHtmlString(value, out var result) ? result : defaultValue;
        }

        public static T ParseEnum<T>(this string value, T defaultValue = default(T)) where T : struct
        {
            T result;
            return Enum.TryParse(value, out result) ? result : defaultValue;
        }

        public static int? ParseIntNullable(this string value)
        {
            int result;
            return int.TryParse(value, out result) ? (int?)result : null;
        }

        public static bool ParseBool(this string value, bool defaultValue = default(bool))
        {
            //0がfalse 1がtrueという風潮があるので
            var i = ParseIntNullable(value);
            if (i.HasValue) return i != 0;
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }

        public static TimeSpan ParseTime(this string value, TimeSpan defaultValue = default(TimeSpan))
        {
            TimeSpan result;
            return TimeSpan.TryParse(value, out result) ? result : defaultValue;
        }

        public static TimeSpan? ParseTimeNullable(this string value)
        {
            if (value == "0") return null; //0 だけ入っている場合は時刻じゃない
            TimeSpan result;
            return TimeSpan.TryParse(value, out result) ? (TimeSpan?)result : null;
        }

        public static DateTime ParseDate(this string value, DateTime defaultValue = default(DateTime))
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : defaultValue;
        }

        public static DateTime ParseExactDate(this string value, string format, DateTime defaultValue = default(DateTime))
        {
            DateTime result;
            return DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out result) ? result : defaultValue;
        }

        public static DateTime? ParseExactDateNullable(this string value, string format)
        {
            DateTime result;
            return DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out result) ? (DateTime?)result : null;
        }

        public static int ParseInt<T>(this Dictionary<T, string> dic, T key, int defaultValue = default(int))
        {
            return dic.ContainsKey(key) ? ParseInt(dic[key], defaultValue) : defaultValue;
        }

        public static int? ParseIntNullable<T>(this Dictionary<T, string> dic, T key)
        {
            return dic.ContainsKey(key) ? ParseIntNullable(dic[key]) : null;
        }

        public static bool ParseBool<T>(this Dictionary<T, string> dic, T key, bool defaultValue = default(bool))
        {
            return dic.ContainsKey(key) ? ParseBool(dic[key], defaultValue) : defaultValue;
        }

        public static TimeSpan ParseTime<T>(this Dictionary<T, string> dic, T key, TimeSpan defaultValue = default(TimeSpan))
        {
            return dic.ContainsKey(key) ? ParseTime(dic[key], defaultValue) : defaultValue;
        }

        public static TimeSpan? ParseTimeNullable<T>(this Dictionary<T, string> dic, T key)
        {
            return dic.ContainsKey(key) ? dic.ParseString(key).ParseTimeNullable() : null;
        }

        public static DateTime ParseDate<T>(this Dictionary<T, string> dic, T key, DateTime defaultValue = default(DateTime))
        {
            return !dic.ContainsKey(key) ? defaultValue : ParseDate(dic[key], defaultValue);
        }

        public static string ParseString<T>(this Dictionary<T, string> dic, T key, string defaultValue = default(string))
        {
            return dic.ContainsKey(key) ? dic[key] : defaultValue;
        }
    }

    public static class BetweenExtensions
    {
        public static bool IsBetween(this int a, int b, int c)
        {
            return (a - b) * (a - c) <= 0;
        }

        public static bool IsBetween(this float a, float b, float c)
        {
            return (a - b) * (a - c) <= 0;
        }

        public static bool IsBetween(this double a, double b, double c)
        {
            return (a - b) * (a - c) <= 0;
        }
    }

    public static class ComponentExtension
    {
        public static T AddOrCreateComponent<T>(this Component component) where T : Component
        {
            if (component.TryGetComponent(out T tComponent))
            {
                return tComponent;
            }

            return component.gameObject.AddComponent<T>();
        }

        public static T AddOrCreateComponent<T>(this GameObject component) where T : Component
        {
            if (component.TryGetComponent(out T tComponent))
            {
                return tComponent;
            }

            return component.gameObject.AddComponent<T>();
        }
    }
}