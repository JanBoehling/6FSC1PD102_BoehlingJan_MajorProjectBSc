using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audio Set", menuName = "Audio System/Audio Set")]
public class AudioSet : ScriptableObject
{
    public AudioResource[] Set;
    
    public int Length => Set.Length;
}
