using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Object = System.Object;

/*  https://www.youtube.com/watch?v=QL29aTa7J5Q */
public static class SoundManager
{
    private static Dictionary<Sound,GameObject> _soundsPlaying = new Dictionary<Sound, GameObject>();
    private static Dictionary<Sound,GameObject> _soundsPlayingOnceDontRepeat = new Dictionary<Sound, GameObject>();
    private static List<GameObject> _soundsPlayingOnce = new List<GameObject>();
    
    public enum Sound
    {
        MenuClick,
        VultureMove,
        VultureJump,
        VultureDie,
        RatJump,
        RatMove,
        RatDie,
        RatPush,
        MoleboyJump,
        MoleboyJumpNoise,
        MoleboyMove,
        MoleboyDie,
        MoleboyPush,
        MoleboyLadder,
        MetalDoorOpen,
        MetalDoorAir,
        BoxSound,
        StoneSound,
        SwitchPlayerVultureRat,
        MoleboyLanding,
        SwitchPlayerVultureMoleBoy
    }

    public static void PlaySoundOnce(Sound sound,AudioMixerGroup audioMixerGroup ,float volume =1f,float pitch=1f)
    {
        GameObject soundGameObject = new GameObject("Sound" + sound);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.PlayOneShot(GetAudioClip(sound));
        _soundsPlayingOnce.Add(soundGameObject);
    }

    //Entfernt 
    public static void GarbageCollection()
    {
        foreach (GameObject obj in _soundsPlayingOnce.ToList())
        {
            if (obj ? obj : false)
            {
                if (!obj.GetComponent<AudioSource>().isPlaying)
                {
                    UnityEngine.Object.Destroy(obj);
                    _soundsPlayingOnce.Remove(obj);
                }
            }
        }
    }
    
    public static void PlaySoundOnceButDontRepeat(Sound sound,AudioMixerGroup audioMixerGroup ,float volume =1f,float pitch=1f)
    {
    //Wenn Sound schonmal gespielt..
        if (_soundsPlayingOnceDontRepeat.TryGetValue(sound, out GameObject value))
        {
            if (!value.GetComponent<AudioSource>().isPlaying)
            {
                UnityEngine.Object.Destroy(value);
                _soundsPlayingOnceDontRepeat.Remove(sound);
            }
            else if(!_soundsPlayingOnceDontRepeat.TryGetValue(sound, out GameObject value1))
            {
                GameObject soundGameObject = new GameObject("Sound" + sound);
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                audioSource.PlayOneShot(GetAudioClip(sound));
                _soundsPlayingOnceDontRepeat.Add(sound, soundGameObject);
            }

        }
        else
        {
            GameObject soundGameObject = new GameObject("Sound" + sound);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.PlayOneShot(GetAudioClip(sound));
            _soundsPlayingOnceDontRepeat.Add(sound, soundGameObject);
        }
    }

    public static void PlaySoundLoop(Sound sound, AudioMixerGroup audioMixerGroup, float volume =1f,float pitch=1f)
    {
        if (!_soundsPlaying.TryGetValue(sound,out GameObject value))
        {
            GameObject soundGameObject = new GameObject("SoundLoop " + sound);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.clip = GetAudioClip(sound);
            audioSource.loop = true;
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.Play();
            _soundsPlaying.Add(sound,soundGameObject);
        }
    }

    public static void EndSoundLoop(Sound sound)
    {
        if (_soundsPlaying != null && _soundsPlaying.ContainsKey(sound))
        {
            UnityEngine.Object.Destroy(_soundsPlaying[sound]);
            _soundsPlaying.Remove(sound);

        }
    }

    
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.SoundAudioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.AudioClip;
            }
        }
        Debug.LogError("Sound "+ sound + " not found!");
        return null;
    }
}
