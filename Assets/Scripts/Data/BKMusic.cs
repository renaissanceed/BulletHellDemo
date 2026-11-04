using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理音乐组件
/// </summary>
public class BKMusic : MonoBehaviour
{
    private static BKMusic instance;
    public static BKMusic Instance => instance;

    private AudioSource bkAudio;
    private void Awake()
    {
        instance = this;
        if (gameObject.GetComponent<AudioSource>() != null)
        {
            bkAudio = gameObject.GetComponent<AudioSource>();
        }
        //初始化当前是否播放音乐
        SetBkMusicIsOpen(GameDataMgr.Instance.MusicData.musicIsOpen);
        //初始化当前音乐的大小
        SetBkMusicValue(GameDataMgr.Instance.MusicData.musicValue);
    } 

    //设置背景音乐的开关
    public void SetBkMusicIsOpen(bool isOpen)
    {
        bkAudio.mute = !isOpen;
    }

    //设置背景音乐的大小
    public void SetBkMusicValue(float value)
    {
        bkAudio.volume = value;
    }
}
