﻿/****************************************************
	文件：HotfixPanel.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:29   	
	Features：
*****************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EdgeFramework.Res;
using EdgeFramework;

public class HotfixPanel : MonoBehaviour
{
    public Image image;
    public Text text;
    public Text sliderTopTex;
    [Header("热更信息界面")]
    public GameObject infoPanel;
    public Text hotContentTex;

    private float sumTime = 0;
    public void OnInit()
    {
        sumTime = 0;
        image.fillAmount = 0;
        text.text = string.Format("{0:F}M/S", 0);
        HotPatchManager.Instance.ServerInfoError += ServerInfoError;
        HotPatchManager.Instance.ItemError += ItemError;
        if (HotPatchManager.Instance.ComputeUnPackFile())
        {
            sliderTopTex.text = "解压中";
            HotPatchManager.Instance.StartUnackFile(() =>
            {
                sumTime = 0;
                Hotfix();
            });
        }
        else
        {
            Hotfix();
        }
    }

    void Hotfix()
    {
        if (AppConfig.CheckVersionUpdate)
        {
#if UNITY_EDITOR
            CheckVersion();
#else
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //提示网络错误,检测网络连接是否正常
                // GameRoot.OpenCommonConfirm("网络连接失败", "网络连接失败，请检查网络连接是否正常?", () => { Application.Quit(); }, () => { Application.Quit(); });
                GameEventSystem.SendEvent(ShareEvent.OpenSelectMessageBox, "网络连接失败", "网络连接失败，请检查网络连接是否正常?",
              new CallbackSelect(NetworkAnomaly));
            }
            else
            {
                CheckVersion();
            }
#endif
        }
        else
            StartOnFinish();
    }
    private void NetworkAnomaly(SelectMessageBox box, bool b, object[] objs)
    {
        box.gameObject.SetActive(false);
        if (b)
        {
            //点击开始下载
            QuitApp();
        }
        else
        {
            //点击取消下载
            QuitApp();
        }
    }
    void CheckVersion()
    {
        //检查该版本是否有热更
        HotPatchManager.Instance.CheckVersion((hot) =>
    {
            //如果有热更
            if (hot)
        {
                //提示玩家是否确定热更下载
                GameEventSystem.SendEvent(ShareEvent.OpenSelectMessageBox, "热更确定", string.Format("当前版本为{0},有{1:F}M大小热更新,是否确定下载?",
                HotPatchManager.Instance.CurVersion, HotPatchManager.Instance.LoadSumSize / 1024.0f),
              new CallbackSelect(SelectHotfix));
        }
        else
        {
                //进入游戏
                StartOnFinish();
        }
    });
    }
    private void SelectHotfix(SelectMessageBox box, bool b, object[] objs)
    {
        box.gameObject.SetActive(false);
        if (b)
        {
            //点击开始下载
            OnClickStartDownLoad();
        }
        else
        {
            //点击取消下载
            QuitApp();
        }
    }
    //点击开始下载
    void OnClickStartDownLoad()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //提示玩家是否确定热更下载
                GameEventSystem.SendEvent(ShareEvent.OpenSelectMessageBox, "下载确认", "当前使用的是手机流量，是否继续下载？",
                  new CallbackSelect(DownLoadConfirm));
            }
            else
                CheckVersion();
        }
        else
        {
            StartDownLoad();
        }
    }
    private void DownLoadConfirm(SelectMessageBox box, bool b, object[] objs)
    {
        box.gameObject.SetActive(false);
        if (b)
        {
            //开始下载
            StartDownLoad();
        }
        else
        {
            //点击取消下载
            QuitApp(); ;
        }
    }
    /// <summary>
    /// 正式开始下载
    /// </summary>
    void StartDownLoad()
    {

        sliderTopTex.text = "下载中。。。";
        infoPanel.SetActive(true);
        string content = HotPatchManager.Instance.CurrentPatches.Des;
        //content.Replace("\\n", "\n");
        hotContentTex.text = content;
        hotContentTex.text = hotContentTex.text.Replace("\\n", "\n");
        GameRoot.Instance.StartCoroutine(HotPatchManager.Instance.StartDownLoadAB(StartOnFinish));
    }

    /// <summary>
    /// 下载完成回调，或者没有下载的东西直接进入游戏
    /// </summary>
    void StartOnFinish()
    {
        GameRoot.Instance.StartCoroutine(OnFinish());
    }
    IEnumerator OnFinish()
    {
        ProcedureCheckUpdate checkUpadte = GameRoot.Instance.ProcedureMgr.FsmCtrl.CurState as ProcedureCheckUpdate;
        yield return checkUpadte.StartGame(image, sliderTopTex);
        // 关闭该页面
        gameObject.SetActive(false);
        HotPatchManager.Instance.ServerInfoError -= ServerInfoError;
        HotPatchManager.Instance.ItemError -= ItemError;
        //加载场景
        checkUpadte.ChangeState();
    }

    public void Update()
    {
        if (HotPatchManager.Instance.StartUnPack)
        {
            sumTime += Time.deltaTime;
            image.fillAmount = HotPatchManager.Instance.GetUnpackProgress();
            float speed = (HotPatchManager.Instance.AlreadyUnPackSize / 1024.0f) / sumTime;
            text.text = string.Format("{0:F} M/S", speed);
        }

        if (HotPatchManager.Instance.StartDownload)
        {
            sumTime += Time.deltaTime;
            image.fillAmount = HotPatchManager.Instance.GetProgress();
            float speed = (HotPatchManager.Instance.GetLoadSize() / 1024.0f) / sumTime;
            text.text = string.Format("{0:F} M/S", speed);
        }
    }

    void ServerInfoError()
    {
        GameEventSystem.SendEvent(ShareEvent.OpenSelectMessageBox, "服务器列表获取失败", "服务器列表获取失败，请检查网络链接是否正常？尝试重新下载！",
               new CallbackSelect(GetServerListFailed));
    }
    private void GetServerListFailed(SelectMessageBox box, bool b, object[] objs)
    {
        box.gameObject.SetActive(false);
        if (b)
        {
            //再次检查更新
            CheckVersion();
        }
        else
        {
            //点击取消下载
            QuitApp();
        }
    }
    void ItemError(string all)
    {
        GameEventSystem.SendEvent(ShareEvent.OpenSelectMessageBox, "资源下载失败", string.Format("{0}等资源下载失败，请重新尝试下载！", all),
             new CallbackSelect(ResourceDownloadFailed));
        //GameRoot.OpenCommonConfirm("资源下载失败", string.Format("{0}等资源下载失败，请重新尝试下载！", all), AnewDownload, QuitApp);
    }
    private void ResourceDownloadFailed(SelectMessageBox box, bool b, object[] objs)
    {
        box.gameObject.SetActive(false);
        if (b)
        {

            AnewDownload();
        }
        else
        {
            //点击取消下载
            QuitApp();
        }
    }
    void QuitApp()
    {
        Application.Quit();
    }
    void AnewDownload()
    {
        HotPatchManager.Instance.CheckVersion((hot) =>
        {
            if (hot)
            {
                StartDownLoad();
            }
            else
            {
                StartOnFinish();
            }
        });
    }


}
