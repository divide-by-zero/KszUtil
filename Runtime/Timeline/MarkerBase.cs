using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace KszUtil.Timeline
{
    /// <summary>
    /// Base class of Marker what notify something on Timeline
    /// </summary>
    public abstract class MarkerBase : Marker, INotification
    {
        public PropertyName id => new PropertyName(GetIdentifier());

        protected virtual string GetIdentifier()
        {
            return string.Empty;
        }
    }
}