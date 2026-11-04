using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTipPanel : BasePanel<GameTipPanel>
{
    public override void Init()
    {
        Time.timeScale = 0f;
    }
    private void Update()
    {
        //鼠标左键关闭
        if(Input.GetMouseButtonDown(0))
        {
            HideMe();
            GamePanel.Instance.ShowMe();
            Time.timeScale = 1f;
        }
    }
}
