using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.Timeline
{
    [RequireComponent(typeof(PlayableDirector))]
    public sealed class RunnableMarkerReceiver : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private PlayableDirector playableDirector;

        private void Awake()
        {
            if (playableDirector == default)
            {
                return;
            }

            playableDirector.RebuildGraph();
            var playableGraph = playableDirector.playableGraph;
            for (var index = 0; index < playableGraph.GetOutputCount(); index++)
            {
                // 未登録の場合は自身を NotificationReceiver として登録する
                if (playableGraph.GetOutput(index).GetNotificationReceivers().All(x => !(x is RunnableMarkerReceiver)))
                {
                    playableGraph.GetOutput(index).AddNotificationReceiver(this);
                }
            }
        }

        void INotificationReceiver.OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is RunnableMarkerBase runnableMarker)
            {
                runnableMarker.ExposedPropertyTable = playableDirector;
                runnableMarker.Run();
            }
        }

        private void Reset()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
    }
}