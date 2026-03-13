using UnityEngine;
using UnityEngine.UI;

namespace KszUtil
{
    public class TouchEffect : PoolMonoBehaviour<TouchEffect>
    {
        public Sprite[] sprites;

        Image Image { set; get; }
        // Use this for initialization
        void Start()
        {
            Image = GetComponent<Image>();
            Image.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            transform.localScale = Vector3.one;
            Image.sprite = sprites.RandomAt();
        }

        // Update is called once per frame
        void Update()
        {
            //	    transform.AddPosition(Vector2.up*Time.deltaTime * 60);
            transform.localScale *= 1.02f;
            var color = Image.color;
            color.a -= Time.deltaTime;
            if (color.a < 0)
            {
                PoolDestroy(this);
            }
            Image.color = color;
        }
    }
}
