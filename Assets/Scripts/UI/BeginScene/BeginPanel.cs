using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 开始游戏面板
/// </summary>
public class BeginPanel : BasePanel<BeginPanel>
{
    public UIButton btnStart;
    public UIButton btnQuit;
    public UIButton btnRank;
    public UIButton btnSetting;
    public override void Init()
    {
        btnStart.onClick.Add(new EventDelegate(() =>
        {
            //显示选角面板
            ChoosePanel.Instance.ShowMe();
            //隐藏自己
            HideMe();
        }));

        btnQuit.onClick.Add(new EventDelegate(() =>
        {
            //退出游戏
            Application.Quit();
        }));

        btnRank.onClick.Add(new EventDelegate(() =>
        {
            //打开排行榜面板
            RankPanel.Instance.ShowMe();
            //隐藏自己
            HideMe();
        }));

        btnSetting.onClick.Add(new EventDelegate(() =>
        {
            //打开设置面板
            SettingPanel.Instance.ShowMe();
            //隐藏自己
            HideMe();
        }));
    }

    public override void ShowMe()
    {
        base.ShowMe();
    }
}
