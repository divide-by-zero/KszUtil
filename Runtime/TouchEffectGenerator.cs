using UnityEngine;

namespace KszUtil
{
    public class TouchEffectGenerator : MonoBehaviour
    {
        public GameObject effectPrefab;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 p = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), Input.mousePosition, null, out p);

                var effect = TouchEffect.PoolInstantiate(effectPrefab, p, Quaternion.identity);
                effect.transform.SetParent(this.transform, false);
            }
        }
    }
}
