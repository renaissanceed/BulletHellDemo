using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameDataMgr 
{
    public static GameDataMgr Instance => instance;
    private static GameDataMgr instance = new GameDataMgr();
    //音效音乐相关数据
    private MusicData musicData;
    public MusicData MusicData => musicData;

    //排行榜数据
    private RankData rankData;
    public RankData RankData => rankData;

    //角色数据
    private RoleData roleData;
    public RoleData RoleData => roleData;

    //当前选择的英雄编号
    public int nowSelectIndex = 0;

    //子弹数据
    private BulletData bulletData;
    public BulletData BulletData => bulletData;

    //发射点数据
    private FireData fireData;
    public FireData FireData => fireData;

    //商店货币相关
    private StoreData storeData;
    public StoreData StoreData => storeData;
    private GameDataMgr() 
    {
        //加载音效音乐相关数据 从xml中
        musicData = XmlDataMar.Instacne.LoadData(typeof(MusicData),"MusicData") as MusicData;
        //一开始 读取本地的排行榜数据 从xml中
        rankData = XmlDataMar.Instacne.LoadData(typeof(RankData), "RankData") as RankData;
        //从xml中加载写好的数据文件
        roleData = XmlDataMar.Instacne.LoadData(typeof(RoleData), "RoleData") as RoleData;
        //一开始就读取 子弹数据
        bulletData = XmlDataMar.Instacne.LoadData(typeof(BulletData), "BulletData") as BulletData;
        //一开始就读取 发射点的数据
        fireData = XmlDataMar.Instacne.LoadData(typeof(FireData), "FireData") as FireData;
        //一开始就读取 商店里面的数据
        storeData = XmlDataMar.Instacne.LoadData(typeof(StoreData), "StoreData") as StoreData;
    }
    #region 音乐音效相关

    //保存音乐相关数据的方法
    public void SaveMusicData()
    {
        XmlDataMar.Instacne.SavaData(musicData, "MusicData");
    }

    //提供给外部开关音乐
    public void SetMusicIsOpen(bool isOpen)
    {
        //改数据 改变背景音乐的开关
        musicData.musicIsOpen = isOpen;
        BKMusic.Instance.SetBkMusicIsOpen(isOpen);
    }

    //提供给外部开关音效
    public void SetSoundIsOpen(bool isOpen)
    {
        //改数据 改变音效的开关
        musicData.soundIsOpen = isOpen;
    }

    
    //设置音乐大小 0-1
    public void SetMusicValue(float value)
    {
        musicData.musicValue = value;
        BKMusic.Instance.SetBkMusicValue(value);
    }

    //设置音效大小 0-1
    public void SetSoundValue(float value)
    {
        musicData.soundValue = value;
    }
    #endregion

    #region 排行榜数据相关
    //添加排行榜数据
    public void AddRankData(string name,int time)
    {
        //单条数据
        RankInfo rankInfo = new RankInfo();
        rankInfo.name = name;
        rankInfo.time = time;
        rankData.rankList.Add(rankInfo);

        //移除大于10条的内容 
        if(rankData.rankList.Count > 10)
        {
            rankData.rankList.RemoveAt(10);
        }

        //保存数据
        XmlDataMar.Instacne.SavaData(rankData, "RankData");
    }
    #endregion

    #region 角色数据相关
    /// <summary>
    /// 提供给外部 当前选择索引的数据
    /// </summary>
    /// <returns></returns>
    public RoleInfo GetNowSelHeroInfo()
    {
        return roleData.roleList[nowSelectIndex];
    }
    #endregion

    #region 商店数据相关
    //提供给外部的保存数据的方法
    public void SaveStoreData()
    {
        XmlDataMar.Instacne.SavaData(storeData, "StoreData");
    }
    #endregion
}
