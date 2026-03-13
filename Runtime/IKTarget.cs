using UnityEngine;

namespace KszUtil
{
    public class IKTarget : MonoBehaviour
    {
        [SerializeField] private AvatarIKGoal _ik;
        [Range(0, 1)]
        [SerializeField] private float positionWeight;
        [Range(0, 1)]
        [SerializeField] private float rotationWeight;

        public void ApplyAnimator(Animator targetAnim)
        {
            targetAnim.SetIKPositionWeight(_ik, positionWeight);
            targetAnim.SetIKRotationWeight(_ik, rotationWeight);
            targetAnim.SetIKPosition(_ik, transform.position);
            targetAnim.SetIKRotation(_ik, transform.rotation);
        }
    }
}
