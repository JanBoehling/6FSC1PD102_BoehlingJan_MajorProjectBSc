using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu]
public class VideoAssignment : Assignment
{
    [SerializeField] private VideoClip _video;
    [SerializeField] private float _watchtimePercentThreshold;

    public float Duration => (float)_video.length;
}
