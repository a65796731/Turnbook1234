using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ���鱾�����鹦��
/// 1�����鱾�͹ر��鱾
/// �鱾�ɺ��ŵ�״̬��Ϊ�򿪵�״̬���򿪺���ʾ��һҳ
/// 2����ҳ���ܣ�
/// �ӵ�һҳ�����ڶ�ҳ
/// ���鱾�򿪺���ʾ��һҳ����Ҫ�����ڶ�ҳʱ���Ƚ���ҳ����ĵ�һ�ź͵ڶ�����ͼ���óɵ�һҳ�ĺ���ż��ڶ�ҳ��ǰ����
/// �ڷ�ҳ�ɹ��󣬽��鱾����ҳ���óɵڶ�ҳ��ǰ���ţ��鱾����ҳ���óɵڶ�ҳ�ĺ���ţ�֮�󽫷�ҳ��������
/// </summary>
public class Book_WSY : MonoBehaviour
{

    /// <summary>
    /// ��ҳ
    /// </summary>
    GameObject FirstPage;

    /// <summary>
    /// βҳ
    /// </summary>
    GameObject LastPage;

    /// <summary>
    /// ��ҳ
    /// </summary>
    GameObject OverTurnPage;

    [Header("ȫ����ҳ")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("��ҳʱ��")]
    public float time = 1;

    /// <summary>
    /// ��ǰҳ��
    /// </summary>
    int initPageNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        FirstPage = transform.Find("��ҳ").gameObject;
        LastPage = transform.Find("βҳ").gameObject;
        OverTurnPage = transform.Find("��ҳ").gameObject;

        FirstPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum].texture);
        LastPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 1].texture);

        //�رշ�ҳ����
        OverTurnPage.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���鱾
    /// </summary>
    void OpenBooKEvent()
    {

    }

    /// <summary>
    /// �ر��鱾
    /// </summary>
    void CloseBookEvent()
    {

    }

    /// <summary>
    /// ��ת�鱾--����
    /// �ӵ�һҳ�����ڶ�ҳ
    /// ���鱾�򿪺���ʾ��һҳ����Ҫ�����ڶ�ҳʱ���Ƚ���ҳ����ĵ�һ�ź͵ڶ�����ͼ���óɵ�һҳ�ĺ���ż��ڶ�ҳ��ǰ����
    /// �ڷ�ҳ�ɹ��󣬽��鱾����ҳ���óɵڶ�ҳ��ǰ���ţ��鱾����ҳ���óɵڶ�ҳ�ĺ���ţ�֮�󽫷�ҳ��������
    /// </summary>
    void OverTurnBookEvent()
    {
        if (initPageNum + 2 >= sprites.Count || timer > Time.time)
        {
            return;
        }
        timer = Time.time + JianGe;
        //�򿪷�ҳ����
        OverTurnPage.gameObject.SetActive(true);
        //���÷�ҳ�������ͼ
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 1].texture);
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_SecTex", sprites[initPageNum + 2].texture);

        //�Ƚ�βҳ���óɵ�ǰҳ��+2
        LastPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 3].texture);
        //��ʼ��ҳ
        int rotateVal = 0;

        DOTween.To(() => rotateVal, x => rotateVal = x, 180, 1).OnUpdate(delegate
        {
            OverTurnPage.GetComponent<MeshRenderer>().material.SetInt("_Angle", rotateVal);
        }).OnComplete(delegate
        {
            //��ҳ����
            //����ҳ�滻Ϊ��ǰҳ��+1
            FirstPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum + 2].texture);

            //�رշ�ҳ����
            OverTurnPage.gameObject.SetActive(false);
            initPageNum += 2;
        });

    }

    /// <summary>
    /// �淭��Ч��
    /// </summary>
    void OverTurnBookBackEvent()
    {
        if (initPageNum - 2 < 0 || timer > Time.time)
        {
            return;
        }
        timer = Time.time + JianGe;

        //�򿪷�ҳ����
        OverTurnPage.gameObject.SetActive(true);
        //���÷�ҳ�������ͼ
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum - 1].texture);
        OverTurnPage.GetComponent<MeshRenderer>().material.SetTexture("_SecTex", sprites[initPageNum].texture);

        //�Ƚ���ҳ���óɵ�ǰҳ��-2
        FirstPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum - 2].texture);
        //��ʼ��ҳ
        int rotateVal = 180;

        DOTween.To(() => rotateVal, x => rotateVal = x, 0, 1).OnUpdate(delegate
        {
            OverTurnPage.GetComponent<MeshRenderer>().material.SetInt("_Angle", rotateVal);
        }).OnComplete(delegate
        {
            //��ҳ����
            //��δҳ�滻Ϊ��ǰҳ��-1
            LastPage.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", sprites[initPageNum - 1].texture);

            //�رշ�ҳ����
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

