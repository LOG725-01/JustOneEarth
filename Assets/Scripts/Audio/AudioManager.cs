using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSourceObject;
    [SerializeField] private GameObject MusicSource;

    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
    }

    /** @brief Set an AudioSource instance to a sound and play it.
     * 
     * @param audioSource The audioSource instance to set the audio clip to.
     * @param clip The sound to play
     * @param volume The volume at which the sound should be played, 0 for no sound, 1 for max volume
     * @param loop Does the sound have to play in loop or not.
     */
    private void SetAudioClip(AudioSource audioSource, AudioClip clip, float volume, bool loop)
    {
        if (audioSource == null) return;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
    }

    /** @brief Create a new AudioSource to a position and play the clip once.
     * 
     * @param clip The sound to play
     * @param position The position at which the sound should be played
     * @param volume The volume at which the sound should be played, 0 for no sound, 1 for max volume
     */
    private float PlayAudioClip(AudioClip clip, Vector3 position, float volume)
    {
        AudioSource audioSource = Instantiate(audioSourceObject, position, Quaternion.identity);

        SetAudioClip(audioSource, clip, volume, false);

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength );
        return clipLength;
    }
}
