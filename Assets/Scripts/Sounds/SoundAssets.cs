using UnityEngine;
using UnityEngine.Audio;

public class SoundAssets : MonoBehaviour
{
    private static SoundAssets _i;
    public static AudioMixer _audioMixer;
    public static AudioMixerGroup _master;
    public static AudioMixerGroup _fx;
    public static AudioMixerGroup _music;
    
    public SoundAudioClip[] SoundAudioClips;

    void Start()
    {
        InvokeRepeating("GarbageCollection",0f,1.5f);
    }

    private void GarbageCollection()
    {
        SoundManager.GarbageCollection();
    }

    public static SoundAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<SoundAssets>("Sounds/SoundAssets"));
            _audioMixer = Resources.Load("Sounds/Main") as AudioMixer;
            _master = _audioMixer.FindMatchingGroups("Master")[0];
            _fx = _audioMixer.FindMatchingGroups("FX")[0];
            _music = _audioMixer.FindMatchingGroups("Music")[0];
            
            return _i;
        }  
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip AudioClip;
    }
}
