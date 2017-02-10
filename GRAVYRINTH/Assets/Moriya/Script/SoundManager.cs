using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public SoundVolume volume = new SoundVolume();
    public VolumeSetting[] seVolumeSettings;

    private AudioClip[] seClips;
    private AudioClip[] bgmClips;

    private Dictionary<string, int> seIndexes = new Dictionary<string, int>();
    private Dictionary<string, int> bgmIndexes = new Dictionary<string, int>();

    const int cNumChannel = 16;
    private AudioSource bgmSource;
    private AudioSource[] seSources = new AudioSource[cNumChannel];
    private AudioSource[] loopSeSources = new AudioSource[cNumChannel];

    Queue<int> seRequestQueue = new Queue<int>();
    Queue<int> loopSeRequestQueue = new Queue<int>();

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        for (int i = 0; i < seSources.Length; i++)
        {
            seSources[i] = gameObject.AddComponent<AudioSource>();
            loopSeSources[i] = gameObject.AddComponent<AudioSource>();
        }

        seClips = Resources.LoadAll<AudioClip>("SE");
        bgmClips = Resources.LoadAll<AudioClip>("BGM");

        for (int i = 0; i < seClips.Length; ++i)
        {
            seIndexes[seClips[i].name] = i;
        }

        for (int i = 0; i < bgmClips.Length; ++i)
        {
            bgmIndexes[bgmClips[i].name] = i;
        }

        /* Debug.Log("se ========================"); */
        /* foreach(var ac in seClips ) { Debug.Log( ac.name ); } */
        /* Debug.Log("bgm ========================"); */
        /* foreach(var ac in bgmClips ) { Debug.Log( ac.name ); } */
    }

    
    void Update()
    {
        bgmSource.mute = volume.mute;
        foreach (var source in seSources)
        {
            source.mute = volume.mute;
        }
        foreach (var source in loopSeSources)
        {
            source.mute = volume.mute;
        }

        //音量設定
        bgmSource.volume = volume.bgm;
        //foreach (var source in seSources)
        //{
        //    source.volume = volume.se;
        //}
        //foreach (var source in loopSeSources)
        //{
        //    source.volume = volume.se;
        //}

        int count = seRequestQueue.Count;
        if (count != 0)
        {
            int sound_index = seRequestQueue.Dequeue();
            playSeImpl(sound_index);
        }
        count = loopSeRequestQueue.Count;
        if (count != 0)
        {
            int sound_index = loopSeRequestQueue.Dequeue();
            playLoopSeImpl(sound_index);
        }
    }

    
    private void playSeImpl(int index)
    {
        if (0 > index || seClips.Length <= index)
        {
            return;
        }

        foreach (AudioSource source in seSources)
        {
            if (!source.isPlaying)
            {
                source.clip = seClips[index];
                source.loop = false;
                source.name = seClips[index].name;
                //音量をセットして再生
                source.volume = volume.se;
                foreach (var setting in seVolumeSettings)
                {
                    if (source.name == setting.name)
                        source.volume = setting.volume;
                }
                source.Play();
                return;
            }
        }
    }

    private void playLoopSeImpl(int index)
    {
        if (0 > index || seClips.Length <= index)
        {
            return;
        }

        foreach (AudioSource source in loopSeSources)
        {
            if (false == source.isPlaying)
            {
                source.clip = seClips[index];
                source.loop = true;
                source.name = seClips[index].name;
                //音量をセットして再生
                source.volume = volume.se;
                foreach (var setting in seVolumeSettings)
                {
                    if (source.name == setting.name)
                        source.volume = setting.volume;
                }
                source.Play();
                return;
            }
        }
    }
    
    public int GetSeIndex(string name)
    {
        return seIndexes[name];
    }

    
    public int GetBgmIndex(string name)
    {
        return bgmIndexes[name];
    }

    
    public void PlayBgm(string name)
    {
        int index = bgmIndexes[name];
        PlayBgm(index);
    }

    
    public void PlayBgm(int index)
    {
        if (0 > index || bgmClips.Length <= index)
        {
            return;
        }

        if (bgmSource.clip == bgmClips[index])
        {
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = bgmClips[index];
        bgmSource.Play();
    }

    
    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    
    public void PlaySe(string name)
    {
        PlaySe(GetSeIndex(name));
    }

    //一旦queueに溜め込んで重複を回避しているので
    //再生が1frame遅れる時がある
    public void PlaySe(int index)
    {
        if (!seRequestQueue.Contains(index))
        {
            seRequestQueue.Enqueue(index);
        }
    }

    public void PlayLoopSe(string name)
    {
        PlayLoopSe(GetSeIndex(name));
    }

    public void PlayLoopSe(int index)
    {
        if (!loopSeRequestQueue.Contains(index))
        {
            loopSeRequestQueue.Enqueue(index);
        }
    }

    
    //すべてのSeを止める
    public void StopSe()
    {
        ClearAllSeRequest();
        foreach (AudioSource source in seSources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    //一部のSeを止める
    public void StopSe(string stopname)
    {
        foreach (AudioSource source in seSources)
        {
            if (source.name == stopname)
            source.Stop();
            source.clip = null;
        }
    }

    //すべてのLoopSeを止める
    public void StopLoopSe()
    {
        ClearAllLoopSeRequest();
        foreach (AudioSource source in loopSeSources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    //一部のLoopSeを止める
    public void StopLoopSe(string stopname)
    {
        foreach (AudioSource source in loopSeSources)
        {
            if (source.name == stopname)
                source.Stop();
            source.clip = null;
        }
    }

    public void ClearAllSeRequest()
    {
        seRequestQueue.Clear();
    }

    public void ClearAllLoopSeRequest()
    {
        loopSeRequestQueue.Clear();
    }

    public AudioSource GetBGMSource()
    {
        return bgmSource;
    }

    public AudioClip GetBGMClip(string name)
    {
        return bgmClips[GetBgmIndex(name)];
    }

    public AudioClip GetSEClip(string name)
    {
        return seClips[GetSeIndex(name)];
    }


}