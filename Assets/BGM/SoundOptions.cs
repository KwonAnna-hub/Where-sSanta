using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SoundOptions : MonoBehaviour
{
    public AudioMixer audioMixer; // 오디오 믹서 연결
    public Slider musicSlider;  // UI 슬라이더 연결
    private static SoundOptions instance;

      private void Awake()
    {
        // 오브젝트가 이미 존재하면 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 이 오브젝트를 유지하고 instance로 설정
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume")){
            LoadVolume();
        }else{
            SetMusicVolume();
        }
    }

    // 볼륨 조절
    public void SetMusicVolume()
    {
        float volume=musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void LoadVolume(){
        musicSlider.value=PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }
}
