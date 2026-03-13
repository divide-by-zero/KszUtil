using System.Collections.Generic;
using UnityEngine;

namespace KszUtil
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GridRenderer : MonoBehaviour
    {
        public enum Face
        {
            xy,
            zx,
            yz,
        };

        [SerializeField] private Material _material;
        public Vector2 gridSize = Vector2.one;
        public int size_x = 8;
        public int size_y = 8;
        public Color color = Color.white;
        public Color growColor = new Color(1, 1, 1, 0);
        public Face face = Face.xy;
        public float thickness = 1;
        public float growSize = 0;

        private Mesh mesh;

        public void OnValidate()
        {
            if (Application.isPlaying) return;
            if (gridSize.x <= 0) gridSize.x = 0.00001f;
            if (gridSize.y <= 0) gridSize.y = 0.00001f;
            if (size_x <= 0) size_x = 1;
            if (size_y <= 0) size_y = 1;
            if (thickness <= 0) thickness = 1;
            if (growSize <= 0) growSize = 0;
            GetComponent<MeshFilter>().mesh = (mesh == null) ? (mesh = new Mesh()) : mesh;
            mesh = ReGrid(mesh);
        }

        Mesh ReGrid(Mesh mesh)
        {
            GetComponent<MeshRenderer>().material = _material;
            mesh.Clear();

            var drawSize = size_x + size_y;
            var width = gridSize.x * size_x / 2.0f;
            var height = gridSize.y * size_y / 2.0f;
            var lineWidth = thickness / 2;
            var startPosition = new Vector2(-width, -height);
            var endPosition = new Vector2(width, height);
            var diff = gridSize;
            var resolution = (drawSize + 2) * 4 * (growSize > 0 ? 3 : 1);
            //最期の２辺を追加している

            var vertices = new List<Vector3>();
            var uvs = new Vector2[resolution];
            var lines = new int[resolution];
            var colors = new Color[resolution];

            //縦軸
            for (int i = 0; i <= size_x; ++i)
            {
                var v1 = new Vector3(startPosition.x + diff.x * i - lineWidth, startPosition.y - lineWidth, 0);
                var v2 = new Vector3(startPosition.x + diff.x * i + lineWidth, startPosition.y - lineWidth, 0);
                var v3 = new Vector3(startPosition.x + diff.x * i + lineWidth, endPosition.y + lineWidth, 0);
                var v4 = new Vector3(startPosition.x + diff.x * i - lineWidth, endPosition.y + lineWidth, 0);

                colors[vertices.Count + 0] = color;
                colors[vertices.Count + 1] = color;
                colors[vertices.Count + 2] = color;
                colors[vertices.Count + 3] = color;

                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);

                if (growSize > 0)
                {
                    var growDiff = new Vector3(growSize, 0, 0);

                    //grow追加
                    colors[vertices.Count + 0] = color;
                    colors[vertices.Count + 1] = growColor;
                    colors[vertices.Count + 2] = growColor;
                    colors[vertices.Count + 3] = color;

                    vertices.Add(v2);
                    vertices.Add(v2 + growDiff);
                    vertices.Add(v3 + growDiff);
                    vertices.Add(v3);

                    //grow追加
                    colors[vertices.Count + 0] = growColor;
                    colors[vertices.Count + 1] = color;
                    colors[vertices.Count + 2] = color;
                    colors[vertices.Count + 3] = growColor;

                    vertices.Add(v1 - growDiff);
                    vertices.Add(v1);
                    vertices.Add(v4);
                    vertices.Add(v4 - growDiff);
                }
            }

            //横軸
            for (int i = 0; i <= size_y; ++i)
            {
                var v1 = new Vector3(startPosition.x - lineWidth, endPosition.y - diff.y * i - lineWidth, 0);
                var v2 = new Vector3(endPosition.x + lineWidth, endPosition.y - diff.y * i - lineWidth, 0);
                var v3 = new Vector3(endPosition.x + lineWidth, endPosition.y - diff.y * i + lineWidth, 0);
                var v4 = new Vector3(startPosition.x - lineWidth, endPosition.y - diff.y * i + lineWidth, 0);

                colors[vertices.Count + 0] = color;
                colors[vertices.Count + 1] = color;
                colors[vertices.Count + 2] = color;
                colors[vertices.Count + 3] = color;

                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);

                if (growSize > 0)
                {
                    var growDiff = new Vector3(0, growSize, 0);

                    //grow追加
                    colors[vertices.Count + 0] = growColor;
                    colors[vertices.Count + 1] = growColor;
                    colors[vertices.Count + 2] = color;
                    colors[vertices.Count + 3] = color;

                    vertices.Add(v1 - growDiff);
                    vertices.Add(v2 - growDiff);
                    vertices.Add(v2);
                    vertices.Add(v1);

                    //grow追加
                    colors[vertices.Count + 0] = color;
                    colors[vertices.Count + 1] = color;
                    colors[vertices.Count + 2] = growColor;
                    colors[vertices.Count + 3] = growColor;

                    vertices.Add(v3);
                    vertices.Add(v4);
                    vertices.Add(v4 + growDiff);
                    vertices.Add(v3 + growDiff);
                }
            }

            for (int i = 0; i < resolution; i++)
            {
                uvs[i] = Vector2.zero;
                lines[i] = i;
            }

            Vector3 rotDirection;
            switch (face)
            {
                case Face.xy:
                    rotDirection = Vector3.forward;
                    break;
                case Face.zx:
                    rotDirection = Vector3.up;
                    break;
                case Face.yz:
                    rotDirection = Vector3.right;
                    break;
                default:
                    rotDirection = Vector3.forward;
                    break;
            }

            mesh.vertices = RotationVertices(vertices.ToArray(), rotDirection);
            mesh.uv = uvs;
            mesh.colors = colors;
            mesh.SetIndices(lines, MeshTopology.Quads, 0);
            mesh.RecalculateNormals();

            return mesh;
        }

        //頂点配列データーをすべて指定の方向へ回転移動させる
        Vector3[] RotationVertices(Vector3[] vertices, Vector3 rotDirection)
        {
            Vector3[] ret = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                ret[i] = Quaternion.LookRotation(rotDirection) * vertices[i];
            }

            return ret;
        }
    }
}