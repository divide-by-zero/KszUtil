using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KszUtil.Editor
{
    [EditorTool("Examples/MyMoveTool")]
    public class MyEditor : EditorTool
    {
        [SerializeField, Min(1)] private int _cnt = 1;
        [SerializeField] private Vector3 _direction = new Vector3(0, 0, 1);

        public override void OnToolGUI(EditorWindow window)
        {
            var center = Selection.transforms.Aggregate(Vector3.zero, (vector3, transform) => vector3 += transform.position) / Selection.transforms.Length;
            if (targets.Any() == false) return; //何も選択されていない

            //自分自身をSerializeObjectと見なすことで、PropertyFieldによる値変更をしやすくする
            var serializedObject = new SerializedObject(this);

            var rect = new Rect(HandleUtility.WorldToGUIPoint(Tools.handlePosition) + new Vector2(20, 20), new Vector2(200, 20));

            Handles.BeginGUI();

            var isEnable = _cnt > 0 && _direction.sqrMagnitude > 0.1f;

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(200)))
            {
                EditorGUILayout.LabelField("LinerCopy");
                var nameProperty = serializedObject.FindProperty("_cnt");
                EditorGUILayout.PropertyField(nameProperty);
                var powerProperty = serializedObject.FindProperty("_direction");
                EditorGUILayout.PropertyField(powerProperty);
                if (isEnable == false)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }

                if (GUILayout.Button("LinerCopy!"))
                {
                    TargetClone(_direction, _cnt, false);
                }

                EditorGUI.EndDisabledGroup();
            }

            GUILayout.EndArea();

            //Propertyの値が変更されていたら、Handleの位置も更新
            if (serializedObject.ApplyModifiedProperties())
            {
            }

            Handles.EndGUI();

            //クローン予定地の描画
            if (isEnable)
            {
                TargetClone(_direction, _cnt, true);
            }
        }

        private void TargetClone(Vector3 distance, int cnt, bool isReserve = true)
        {
            for (int i = 1; i <= cnt; i++)
            {
                foreach (var transform in Selection.transforms)
                {
                    if (isReserve)
                    {
                        Handles.DrawWireCube(transform.position + distance * i, new Vector3(2, 2, 2));
                    }
                    else
                    {
                        GameObject go = null;
                        //prefabがある場合は、Prefabとしてコピー
                        // var prefab = PrefabUtility.GetPrefabParent(transform.gameObject);
                        var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(transform.gameObject);
                        if (prefab != null)
                        {
                            go = PrefabUtility.InstantiatePrefab(prefab, transform.parent) as GameObject;
                            PrefabUtility.SetPropertyModifications(go, PrefabUtility.GetPropertyModifications(transform.gameObject));
                        }
                        else
                        {
                            go = Object.Instantiate(transform.gameObject, transform.parent) as GameObject;
                        }

                        if (go != null)
                        {
                            go.transform.position = transform.position + distance * i;
                            go.transform.rotation = transform.rotation;
                            Undo.RegisterCreatedObjectUndo(go, "LinerCopy");
                        }
                    }
                }
            }
        }

        private static IEnumerator SimulateIterator(float time)
        {
            var targetRigiBodies = UnityEditor.Selection.gameObjects.SelectMany(o => o.GetComponentsInChildren<Rigidbody>()).ToArray();
            var currentSceneRigidBodies = FindObjectsOfType<Rigidbody>().Except(targetRigiBodies).ToArray();

            var positions = currentSceneRigidBodies.Select(r => r.position).ToArray();
            var velocitys = currentSceneRigidBodies.Select(r => r.linearVelocity).ToArray();
            var rotations = currentSceneRigidBodies.Select(r => r.rotation).ToArray();
            var angularVelocitys = currentSceneRigidBodies.Select(r => r.angularVelocity).ToArray();

            Physics.autoSimulation = false;
            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                Physics.Simulate(Time.fixedDeltaTime);
                yield return null;
            }

            Physics.autoSimulation = true;

            for (int i = 0; i < currentSceneRigidBodies.Length; i++)
            {
                currentSceneRigidBodies[i].transform.position = positions[i];
                currentSceneRigidBodies[i].transform.rotation = rotations[i];
                currentSceneRigidBodies[i].linearVelocity = velocitys[i];
                currentSceneRigidBodies[i].angularVelocity = angularVelocitys[i];
            }
        }
    }
}