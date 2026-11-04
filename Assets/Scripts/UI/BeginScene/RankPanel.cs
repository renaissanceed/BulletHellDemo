using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankPanel : BasePanel<RankPanel>
{
    public UIButton btnClose;
    public UIScrollView sv_List;

    //专门储存 单条数据控件
    private List<RankItem> itemList = new List<RankItem>();
    public override void Init()
    {
        btnClose.onClick.Add(new EventDelegate(() =>
        {
            //隐藏自己
            HideMe();
            //打开开始面板
            BeginPanel.Instance.ShowMe();
        }));
        HideMe();
    }
    public override void ShowMe()
    {
        base.ShowMe();
        //获取本地储存的排行榜数据
        List<RankInfo> list = GameDataMgr.Instance.RankData.rankList;
        //排序排行榜
        list.Sort((a, b) => b.time - a.time);
        //根据数据 更新面板上 组合控价的信息
        //组合 控件数量 只会增加 不会变少 因为玩家只会增加数据 不会 删除数据
        for (int i = 0; i < list.Count; i++)
        {
            //如果面板上 已经存在 组合控件 直接更新即可
            if (itemList.Count > i)
            {
                itemList[i].InitInfo(i + 1, list[i].name, list[i].time);
            }
            //面板上不够多 就更新预设体
            else
            {
                //创建预设体
                GameObject obj = Instantiate(Resources.Load<GameObject>("UI/RankItem"));
                //设置父对象
                obj.transform.SetParent(sv_List.transform,false);
                //设置位置
                obj.transform.localPosition = new Vector3(-1,-23 + i * -113,0);

                //获取脚本数据
                RankItem rankItem = obj.GetComponent<RankItem>();
                //设置数据
                rankItem.InitInfo(i + 1, list[i].name, list[i].time);
                //记录数据
                itemList.Add(rankItem);
            }
        }
        //重置高度 防止反复打开排行榜 数据往下面掉
        sv_List.ResetPosition();
    }
}
