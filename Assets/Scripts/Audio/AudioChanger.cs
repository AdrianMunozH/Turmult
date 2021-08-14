using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.Switch;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ValueObjects;

namespace Audio
{
    public class AudioChanger : MonoBehaviour
    {
        
        private String[] mixerGroupVolumeArray = {"MasterVol", "FXVol","MusicVol"};

        public AudioMixer mixer;
        public String mixerGroup; 

        public FloatObject master;
        public FloatObject music;
        public FloatObject fx;

        private void Awake()
        {
        
            switch (mixerGroup)
            {
                case "MusicVol":
                    mixer.SetFloat(getMixerGroup(mixerGroup), Mathf.Log10(music.RuntimeValue) * 20);
                    gameObject.GetComponent<Slider>().value = music.RuntimeValue;
                    break;
                case "FXVol":
                    mixer.SetFloat(getMixerGroup(mixerGroup),Mathf.Log10(fx.RuntimeValue) * 20 );
                    gameObject.GetComponent<Slider>().value = fx.RuntimeValue;
                    break;
                case "MasterVol":
                    mixer.SetFloat(getMixerGroup(mixerGroup), Mathf.Log10(master.RuntimeValue) * 20);
                    gameObject.GetComponent<Slider>().value = master.RuntimeValue;
                    break;
                default:
                    break;
            }
        }
        
        public void SetLevel(float sliderValue)
        {
            mixer.SetFloat(getMixerGroup(mixerGroup), Mathf.Log10(sliderValue) * 20);
            switch (getMixerGroup(mixerGroup))
            {
                case "MusicVol":
                    music.RuntimeValue = sliderValue;
                    break;
                case "FXVol":
                    fx.RuntimeValue = sliderValue;
                    break;
                default:
                    master.RuntimeValue = sliderValue;
                    break;
            }
        }
        
        
        public String getMixerGroup(String mixerGroupName)
        {
            foreach (String name in mixerGroupVolumeArray)
            {
                if (mixerGroupName.Equals(name))
                {
                    return name;
                }
            }
            Debug.LogError("Sound "+ mixerGroupName + " not found!");
            return null;
        }
    }
}