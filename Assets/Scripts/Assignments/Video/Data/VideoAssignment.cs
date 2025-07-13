using UnityEngine;

[CreateAssetMenu]
public class VideoAssignment : AssignmentDataBase
{
    [field: SerializeField, Tooltip("The url for the video. Can't use VideoClips as they are not supported in WebGL")] public string VideoURL { get; private set; }
    [field: SerializeField, Tooltip("The duration of the video in seconds")] public float Duration { get; private set; }
    [field: SerializeField, Tooltip("The percentage that the user needs to watch to complete the video assignment")] public float WatchtimePercentThreshold { get; private set; }

}
