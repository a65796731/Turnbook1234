using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 打开书本及翻书功能
/// 1、打开书本和关闭书本
/// 书本由合着的状态变为打开的状态，打开后显示第一页
/// 2、翻页功能：
/// 从第一页翻到第二页
/// 在书本打开后，显示第一页，需要翻到第二页时，先将翻页物体的第一张和第二张贴图设置成第一页的后半张及第二页的前半张
/// 在翻页成功后，将书本的左页设置成第二页的前半张，书本的右页设置成第二页的后半张，之后将翻页物体隐藏
/// </summary>
public class Book_WSY : MonoBehaviour
{

    /// <summary>
    /// 首页
    /// </summary>
    GameObject FirstPage;

    /// <summary>
    /// 尾页
    /// </summary>
    GameObject LastPage;

    /// <summary>
    /// 翻页
    /// </summary>
    GameObject OverTurnPage;

    [Header("全部书页")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("翻页时间")]
    public float time = 1;

    /// <summary>
    /// 当前页数
    /// </summary>
    int initPageNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        FirstPage = transform.Find("首页").gameObject;
        LastPage = transform.Find("尾页").gameObject;
        OverTurnPage = transform.Find("翻页").gameObject;

        FirstPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum].texture);
        LastPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 1].texture);

        //关闭翻页物体
        OverTurnPage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 打开书本
    /// </summary>
    void OpenBooKEvent()
    {

    }

    /// <summary>
    /// 关闭书本
    /// </summary>
    void CloseBookEvent()
    {

    }

    /// <summary>
    /// 翻转书本--正翻
    /// 从第一页翻到第二页
    /// 在书本打开后，显示第一页，需要翻到第二页时，先将翻页物体的第一张和第二张贴图设置成第一页的后半张及第二页的前半张
    /// 在翻页成功后，将书本的左页设置成第二页的前半张，书本的右页设置成第二页的后半张，之后将翻页物体隐藏
    /// </summary>
    void OverTurnBookEvent()
    {
        if (initPageNum + 2 >= sprites.Count || timer > Time.time)
        {
            return;
        }
        timer = Time.time + JianGe;
        //打开翻页物体
        OverTurnPage.gameObject.SetActive(true);
        //设置翻页物体的贴图
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 1].texture);
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_SecTex", sprites[initPageNum + 2].texture);

        //先将尾页设置成当前页数+2
        LastPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 3].texture);
        //开始翻页
        int rotateVal = 0;

        DOTween.To(() => rotateVal, x => rotateVal = x, 180, 1).OnUpdate(delegate
        {
            OverTurnPage.GetComponent<MeshRenderer>().material.SetInt("_Angle", rotateVal);
        }).OnComplete(delegate
        {
            //翻页结束
            //将首页替换为当前页数+1
            FirstPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 2].texture);

            //关闭翻页物体
            OverTurnPage.gameObject.SetActive(false);
            initPageNum += 2;
        });

    }

    /// <summary>
    /// 逆翻书效果
    /// </summary>
    void OverTurnBookBackEvent()
    {
        if (initPageNum - 2 < 0 || timer > Time.time)
        {
            return;
        }
        timer = Time.time + JianGe;

        //打开翻页物体
        OverTurnPage.gameObject.SetActive(true);
        //设置翻页物体的贴图
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum - 1].texture);
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_SecTex", sprites[initPageNum].texture);

        //先将首页设置成当前页数-2
        FirstPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum - 2].texture);
        //开始翻页
        int rotateVal = 180;

        DOTween.To(() => rotateVal, x => rotateVal = x, 0, 1).OnUpdate(delegate
        {
            OverTurnPage.GetComponent<MeshRenderer>().material.SetInt("_Angle", rotateVal);
        }).OnComplete(delegate
        {
            //翻页结束
            //将未页替换为当前页数-1
            LastPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum - 1].texture);

            //关闭翻页物体
            OverTurnPage.gameObject.SetActive(false);
            initPageNum -= 2;
        });
    }


    float timer;
    float JianGe = 1.1f;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            OverTurnBookEvent();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OverTurnBookBackEvent();
        }



    }
}

