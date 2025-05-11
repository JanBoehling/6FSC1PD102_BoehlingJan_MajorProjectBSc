using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu]
public class VideoAssignment : Assignment
{
    [field: SerializeField] public VideoClip Video { get; private set; }
    [field:SerializeField] public float WatchtimePercentThreshold { get; private set; }

    public float Duration => (float)Video.length;
}
