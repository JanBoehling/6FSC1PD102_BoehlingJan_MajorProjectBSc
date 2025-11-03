using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Set", menuName = "Audio System/Audio Set")]
public class AudioSet : ScriptableObject
{
    public AudioClip[] Set;
    
    public int Length => Set.Length;
}
