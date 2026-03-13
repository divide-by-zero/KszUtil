#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KszUtil
{
    /// <summary>
    ///     定数を管理するクラスを生成するクラス
    /// </summary>
    public static class ConstantsClassCreator
    {
        //無効な文字を管理する配列
        private static readonly string[] INVALUD_CHARS =
        {
            " ", "!", "\"", "#", "$", "%", "&", "\'", "(", ")", "-", "=", "^", "~", "\\", "|", "[", "{", "@", "`", "]", "}", ":", "*", ";", "+", "/", "?", ".", ">", ",", "<",
        };

        /// <summary>
        ///     定数を管理するクラスを自動生成する
        /// </summary>
        public static void Create(string nameSpace, string className, string classInfo, List<string> valueList, string path)
        {
            if (Application.isPlaying) return;

            //コメント文とクラス名を入力
            var builder = new StringBuilder();
            var tab = 0;

            AppendLine(builder, $"namespace {nameSpace}", tab);
            AppendLine(builder, "{", tab++);
            AppendLine(builder, "/// <summary>", tab);
            AppendLine(builder, $"/// {classInfo} ", tab);
            AppendLine(builder, "/// </summary> ", tab);
            AppendLine(builder, $"public enum {className}", tab);
            AppendLine(builder, "{", tab++);

            //入力された定数とその値のペアを書き出していく
            foreach (var key in valueList)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                //数字だけのkeyだったらスルー
                if (Regex.IsMatch(key, @"^[0-9]+$"))
                {
                    continue;
                }

                //上記で判定した型と定数名を入力
                AppendLine(builder, $@"  {RemoveInvalidChars(key)} ,", tab);
            }

            AppendLine(builder, "}", --tab);

            AppendLine(builder, $"public static class {className}Extension", tab);
            AppendLine(builder, "{", tab++);
            AppendLine(builder, $"public static string ToPath(this {className} evalue)", tab);
            AppendLine(builder, "{", tab++);

            AppendLine(builder, "switch(evalue)", tab);
            AppendLine(builder, "{", tab++);

            //入力された定数とその値のペアを書き出していく
            foreach (var key in valueList)
            {
                if (string.IsNullOrEmpty(key)) continue;
                //数字だけのkeyだったらスルー
                if (Regex.IsMatch(key, @"^[0-9]+$")) continue;
                AppendLine(builder, $"case {className}.{RemoveInvalidChars(key)} : return \"{key}\";", tab);
            }

            AppendLine(builder, "}", --tab);
            AppendLine(builder, "return null;", tab);
            AppendLine(builder, "}", --tab);
            AppendLine(builder, "}", --tab);
            AppendLine(builder, "}", --tab);

            //書き出し、ファイル名はクラス名.cs
            var exportPath = Path.Combine(path, className + ".cs");

            //書き出し先のディレクトリが無ければ作成
            var directoryName = Path.GetDirectoryName(exportPath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(exportPath, builder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

            //            Debug.Log(className + ".cs" + "の作成が完了しました");
        }

        public static void AppendLine(StringBuilder builder, string str, int tab)
        {
            builder.Append(new string('\t', tab)).AppendLine(str);
        }

        /// <summary>
        ///     無効な文字を削除
        /// </summary>
        private static string RemoveInvalidChars(string str)
        {
            Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, "_"));
            return str;
        }
    }
}
#endif
