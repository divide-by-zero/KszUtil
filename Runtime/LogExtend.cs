using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KszUtil
{
    public static class LogExtend
    {
        public static void Log(string message = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            Debug.Log($"{file}:{line}-{member}\t{message}");
        }

        public static void LogDump(object obj,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            Debug.Log($"{file}:{line}-{member}\n{string.Join("\n", obj.GetType().GetProperties().Where(x => x.CanRead && x.CanWrite).ToDictionary(pi => pi.Name, pi => pi.GetValue(obj)).Select(pair => $"\t{pair.Key}:{pair.Value}"))}");
        }
    }
}
