using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioPlayer : MonoBehaviour
{
    [Tooltip("Sets whether the sound should play through an Audio Mixer first or directly to the Audio Listener.")]
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    
    [Space]
    
    [Tooltip("The audio set to be played. Leave empty and use Play(AudioSet set) to dynamically set the audio set.")]
    [SerializeField] private AudioSet localAudioSet;

    [Tooltip("Select the play mode of the auto play feature. Requires a audio set in the \"Local Audio Set\" field.")]
    [SerializeField] private AutoPlayMode autoPlayMode;
    
    private enum AutoPlayMode
    {
        Manual,
        PlayRandomSingleLooping,
        PlayAllLooping
    }
    
    private AudioSource audioSource;

    private void Awake()
    {
        if (!gameObject.TryGetComponent<AudioSource>(out audioSource)) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;
    }

    private void Start()
    {
        switch (autoPlayMode)
        {
            case AutoPlayMode.Manual:
                break;
            case AutoPlayMode.PlayRandomSingleLooping:
                PlayLooping();
                break;
            case AutoPlayMode.PlayAllLooping:
                PlayAllLooping();
                break;
            default:
                Debug.LogError($"A not supported play mode was set: {autoPlayMode.ToString()}");
                break;
        }
    }

    #region Playing local audio set
    /// <summary>
    /// Plays a random audio clip from the local audio set.
    /// </summary>
    public void Play() => Play(localAudioSet);

    /// <summary>
    /// Plays a random audio clip from the local audio set and loops.
    /// </summary>
    public void PlayLooping() => Play(localAudioSet, true);

    /// <summary>
    /// Plays all audio clips from the local audio set. Stops, if all audio clips have been played.
    /// </summary>
    public void PlayAll() => PlayAll(localAudioSet);

    /// <summary>
    /// Plays all audio clips from the local audio set. Loops, if all audio clips have been played.
    /// </summary>
    public void PlayAllLooping() => PlayAllLooping(localAudioSet);
    #endregion
    
    /// <summary>
    /// Plays a random audio clip from the given audio set.
    /// </summary>
    /// <param name="set">The audio set, from where the clip will be picked.</param>
    public void Play(AudioSet set)
    {
        int clipAmount = set.Length;

        if (clipAmount == 0)
        {
            string objectRoot = transform.parent ? transform.parent.name : gameObject.name;
            Debug.LogWarning($"The audio set that was tried to play by {objectRoot} is empty.");
            return;
        }
        
        int clipIndex = Random.Range(0, clipAmount);
        Play(set.Set[clipIndex]);
    }

    /// <summary>
    /// Plays a random audio clip from the given audio set.
    /// </summary>
    /// <param name="set">The audio set, from where the clip will be picked.</param>
    /// <param name="doLoop">Defines if the given audio clip should loop or not.</param>
    public void Play(AudioSet set, bool doLoop)
    {
        int clipAmount = set.Length;

        if (clipAmount == 0)
        {
            string objectRoot = transform.parent ? transform.parent.name : gameObject.name;
            Debug.LogWarning($"The audio set that was tried to play by {objectRoot} is empty.");
            return;
        }
        
        int clipIndex = Random.Range(0, clipAmount);
        Play(set.Set[clipIndex], doLoop);
    }

    /// <summary>
    /// Plays the given audio clip.
    /// </summary>
    /// <param name="clip">The audio clip to be played.</param>
    public void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    /// <summary>
    /// Plays the given audio clip.
    /// </summary>
    /// <param name="clip">The audio clip to be played.</param>
    /// <param name="doLoop">Defines if the given audio clip should loop or not.</param>
    public void Play(AudioClip clip, bool doLoop)
    {
        audioSource.clip = clip;
        audioSource.loop = doLoop;
        audioSource.Play();
    }

    /// <summary>
    /// Plays all audio clips from the given audio set. Stops, if all audio clips have been played.
    /// </summary>
    /// <param name="set">The audio set, that should be played.</param>
    public void PlayAll(AudioSet set) => StartCoroutine(PlayCO(set.Set, false));
    
    /// <summary>
    /// Plays all audio clips from the given audio set. Loops, if all audio clips have been played.
    /// </summary>
    /// <param name="set">The audio set, that should be played.</param>
    public void PlayAllLooping(AudioSet set) => StartCoroutine(PlayCO(set.Set, true));

    /// <summary>
    /// Defines a random start index and starts playing all audio clips one by one.
    /// </summary>
    /// <param name="audioClips">The audio set, that should be played.</param>
    /// <param name="doLoop">If true, this coroutine will run infinitely.</param>
    /// <param name="playSingle">If true, only one random clip from the audio set will be played instead of all. Default is false.</param>
    private IEnumerator PlayCO(IReadOnlyList<AudioClip> audioClips, bool doLoop, bool playSingle = false)
    {
        int clipAmount = audioClips.Count;

        if (clipAmount == 0)
        {
            string objectRoot = transform.parent ? transform.parent.name : gameObject.name;
            Debug.LogWarning($"The audio set that was tried to play by {objectRoot} is empty.");
            yield break;
        }
        
        int index = Random.Range(0, clipAmount);
        int startIndex = index;
        
        while (true)
        {
            var currentClip = audioClips[playSingle ? index : index++];
            
            Play(currentClip);

            yield return new WaitForSeconds(currentClip.length);

            if (playSingle) yield return null;

            if (index == startIndex && !doLoop) yield break;
            if (index == audioClips.Count) index = 0;
        }
    }
}
