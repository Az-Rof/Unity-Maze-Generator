using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource; // Untuk Background Music
    public AudioSource sfxSource; // Untuk Sound Effects

    [Header("Audio Clips")]
    public List<Audio> bgmList;  // List untuk BGM
    public List<Audio> sfxList;  // List untuk SFX

    private void Awake()
    {
        // Singleton Pattern: Pastikan hanya ada satu AudioManager di scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // AudioManager tetap ada saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM("DefaultBGM"); // Mainkan musik awal (ubah sesuai nama BGM di list)
    }

    public void PlayBGM(string name)
    {
        Audio bgm = bgmList.Find(b => b.name == name);
        if (bgm != null)
        {
            bgmSource.clip = bgm.audioClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM dengan nama " + name + " tidak ditemukan!");
        }
    }

    public void PlaySFX(string name)
    {
        Audio sfx = sfxList.Find(s => s.name == name);
        if (sfx != null)
        {
            sfxSource.PlayOneShot(sfx.audioClip);
        }
        else
        {
            Debug.LogWarning("SFX dengan nama " + name + " tidak ditemukan!");
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
}
