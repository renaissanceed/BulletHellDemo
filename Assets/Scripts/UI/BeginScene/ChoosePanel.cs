using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoosePanel : BasePanel<ChoosePanel>
{
    //各个按钮
    public UIButton btnClose;
    public UIButton btnLeft;
    public UIButton btnRight;
    public UIButton btnStart;
    public UIButton btnStore;

    //模型生成位置
    public Transform heroPos;

    //属性列表
    public List<GameObject> hpObjs;
    public List<GameObject> speedObjs;
    public List<GameObject> volumeObjs;

    //记录当前显示的模型
    private GameObject nowAirplaneObj;
    public override void Init()
    {
        //关闭按钮
        btnClose.onClick.Add(new EventDelegate(() =>
        {
            HideMe();
            BeginPanel.Instance.ShowMe();
        }));

        //商店按钮
        btnStore.onClick.Add(new EventDelegate(() =>
        {
            HideMe();
            //打开商店面板
            StorePanel.Instance.ShowMe();
        }));

        //左边切换按钮
        btnLeft.onClick.Add(new EventDelegate(() =>
        {
            //按左按钮 减我们的索引
            --GameDataMgr.Instance.nowSelectIndex;
            //如果小于最小索引 直接等于 最后一个索引
            if (GameDataMgr.Instance.nowSelectIndex < 0)
            {
                GameDataMgr.Instance.nowSelectIndex = GameDataMgr.Instance.RoleData.roleList.Count - 1;
            }
            ChangeNowHero();
        }));

        //右边切换按钮
        btnRight.onClick.Add(new EventDelegate(() =>
        {
            //按右按钮 减我们的索引
            ++GameDataMgr.Instance.nowSelectIndex;
            //如果大于最小索引 直接等于 第一个索引
            if (GameDataMgr.Instance.nowSelectIndex >= GameDataMgr.Instance.RoleData.roleList.Count)
            {
                GameDataMgr.Instance.nowSelectIndex = 0;
            }
            ChangeNowHero();
        }));

        //游戏开始按钮
        btnStart.onClick.Add(new EventDelegate(() =>
        {
            SceneManager.LoadScene("GameScene");
        }));

        HideMe();
    }

    //切换当前的选择
    private void ChangeNowHero()
    {
        //得到 当前选择的 玩家数据信息
        RoleInfo info = GameDataMgr.Instance.GetNowSelHeroInfo();
        //更新模型
        //先删除上一次的飞机模型
        DestroyObj();
        //再创建当前 模型
        nowAirplaneObj = Instantiate(Resources.Load<GameObject>(info.resName));
 
        nowAirplaneObj.transform.SetParent(heroPos,false);
        nowAirplaneObj.transform.localScale = Vector3.one * info.scale;
        nowAirplaneObj.layer = LayerMask.NameToLayer("UI");
        //更新属性
        for (int i = 0; i < 10; i++)
        {
            hpObjs[i].SetActive(i < info.hp);
            speedObjs[i].SetActive(i < info.speed);
            volumeObjs[i].SetActive(i < info.volume);
        }

    }

    /// <summary>
    /// 用于删除 上一次显示的模型对象
    /// </summary>
    private void DestroyObj()
    {
        if (nowAirplaneObj != null)
        {
            Destroy(nowAirplaneObj);
            nowAirplaneObj = null;
        }
    }

    public override void ShowMe()
    {
        base.ShowMe();
        GameDataMgr.Instance.nowSelectIndex = 0;
        ChangeNowHero();
    }

    public override void HideMe()
    {
        base.HideMe();
        DestroyObj();
    }

    private float time;
    //是否鼠标选择 模型
    private bool isSel;
    void Update()
    {
        //让飞机 上下浮动
        time += Time.deltaTime;
        heroPos.Translate(Vector3.up * Mathf.Sin(time) * 0.0001f,Space.World);

        //射线检测 让飞机 可以左右转动
        if (Input.GetMouseButtonDown(0))
        {
            //点击到了 UI层碰撞器 认为 需要开始 拖动 飞机了
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                1000,1<<LayerMask.NameToLayer("UI"))) 
            {
                isSel = true;
            }
        }

        //抬起取消选中
        if (Input.GetMouseButtonUp(0))
        {
            isSel = false;
        }

        //旋转对象 
        if (Input.GetMouseButton(0) && isSel)
        {
            heroPos.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * -20 , Vector3.up);
        }
    }

}
