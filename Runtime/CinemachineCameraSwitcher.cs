using System.Linq;
using Cinemachine;
using UnityEngine;
namespace KszUtil
{
    public class CinemachineCameraSwitcher : MonoBehaviour
    {
        private CinemachineVirtualCamera[] _wellkownCameras;
        public void Start()
        {
            _wellkownCameras = this.GetComponentsInChildren<CinemachineVirtualCamera>();
            this.ChangeCamera(0);
        }

        public void ChangeCamera(int index)
        {
            this.ChangeCamera(_wellkownCameras.ElementAtOrDefault(index % _wellkownCameras.Length));
        }

        /// <summary>
        ///     指定したカメラだけを有効化（他管理カメラを無効化)
        /// </summary>
        /// <param name="targetCame"></param>
        public void ChangeCamera(CinemachineVirtualCamera targetCame)
        {
            foreach (var camera in _wellkownCameras)
            {
                camera.enabled = camera == targetCame;
            }
        }
    }
}