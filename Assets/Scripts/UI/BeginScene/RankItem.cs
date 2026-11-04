using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据排行榜单条数据 对组合控件 进行显示初始化
/// </summary>
public class RankItem : MonoBehaviour
{
    public UILabel labRanking;
    public UILabel labName;
    public UILabel labTime;

    public void InitInfo(int ranking,string name,int time)
    {
        //排名
        labRanking.text = ranking.ToString();
        //名字
        labName.text = name;
        //时间
        string str = "";
        if (time / 3600 > 0)
        {
            str += time / 3600 + "h";
        }
        if (time % 3600 /60 > 0 || str != "")
        {
            str += time % 3600 / 60 + "m";
        }
        str += time % 60 + "s";
        labTime.text = str;
    }
}
