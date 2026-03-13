using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KszUtil
{
    public static class VectorExtension
    {
        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            return vectors.Sum() / vectors.Count();
        }

        public static Vector3 Sum(this IEnumerable<Vector3> vectors)
        {
            var sum = Vector3.zero;
            foreach (var v in vectors)
            {
                sum += v;
            }

            return sum;
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            return vectors.Sum() / vectors.Count();
        }

        public static Vector2 Sum(this IEnumerable<Vector2> vectors)
        {
            var sum = Vector2.zero;
            foreach (var v in vectors)
            {
                sum += v;
            }

            return sum;
        }
    }
}
