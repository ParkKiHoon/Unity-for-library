using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShowParts : MonoBehaviour
{
    bool[][] objNum = new bool[5][] {
        new bool[] { false },
        new bool[] { false, false },
        new bool[] { false },
        new bool[] { false, false },
        new bool[] { false, false, false, false, false, false, false, false }
    };
    float totalWeight;
    float totalPrice;
    GameObject cameraPosition;
    Text weightText;
    Text priceText;
    FrameData frameData;
    int showCnt;
    int divideCnt;
    float[] cal_price = new float[4] { 0f, 0f, 0f, 0f };
    float[] cal_weight = new float[4] { 0f, 0f, 0f, 0f };
    public Button col1, col2;
    public Material[] materials;
    int btn_state;

    void Start()
    {
        cameraPosition = GameObject.Find("CameraPosition");
        weightText = GameObject.Find("WeightText").GetComponent<Text>();
        priceText = GameObject.Find("PriceText").GetComponent<Text>();
    }
    void Update()
    {
        SetUi();
    }

    void Show(string str)
    {
        string[] result = str.Split(new char[] { '/' });
        int value = int.Parse(result[0]);
        if (value == 1106 || value == 1107)
        {
            ChangeShaderTexture(value);
        }
        else
        {
            col1.gameObject.SetActive(false);
            col2.gameObject.SetActive(false);
        }
        int partType = value / 1000;
        if (partType == 1)
        {
            FrameShow(result[0], result[1], result[2]);
            return;
        }
        switch (partType)
        {
            case 2: showCnt = 2; break;
            case 3: showCnt = 1; break;
            case 4: showCnt = 2; break;
            case 5: showCnt = 8; break;
        }

        divideCnt = showCnt;
        while (showCnt != 0)
        {
            PartsShow(value.ToString(), result[1], result[2]);
            value += 100;
            showCnt--;
        }
        switch (str[0])
        {
            case '2': cal_price[0] = Convert.ToSingle(result[2].ToString()); cal_weight[0] = Convert.ToSingle(result[1].ToString()); break;
            case '3': cal_price[1] = Convert.ToSingle(result[2].ToString()); cal_weight[1] = Convert.ToSingle(result[1].ToString()); break;
            case '4': cal_price[2] = Convert.ToSingle(result[2].ToString()); cal_weight[2] = Convert.ToSingle(result[1].ToString()); break;
            case '5': cal_price[3] = Convert.ToSingle(result[2].ToString()); cal_weight[3] = Convert.ToSingle(result[1].ToString()); break;
        }

    }
    void PartsShow(string str, string weight, string price)
    {
        int value = int.Parse(str);
        int partType = value / 1000;
        int partType2 = (value % 1000) / 100;
        GameObject temp = Resources.Load<GameObject>(partType.ToString() + "/" + str);

        if (objNum[0][0] == false) return;
        if (objNum[partType - 1][partType2 - 1] == true)
        {
            string tempForDestroy="";
            switch (str[0])
            {
                case '2':
                    tempForDestroy = "wheelset"; 
                    totalWeight -= cal_weight[0] / divideCnt;
                    totalPrice -= cal_price[0] / divideCnt; break;
                case '3': 
                    tempForDestroy = "handlebar";
                    totalWeight -= cal_weight[1] / divideCnt;
                    totalPrice -= cal_price[1] / divideCnt; break;
                case '4':
                    tempForDestroy = "saddle";
                    totalWeight -= cal_weight[2] / divideCnt;
                    totalPrice -= cal_price[2] / divideCnt; break;
                case '5':
                    tempForDestroy = "groupset";
                    totalWeight -= cal_weight[3] / divideCnt;
                    totalPrice -= cal_price[3] / divideCnt; break;
            }
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tempForDestroy);
            for (int i = 0; i < objects.Length; i++)
                Destroy(objects[i]);
            objNum[partType - 1][partType2 - 1] = false;
        }
        else
        {
            Vector3 partPos = new Vector3(0, 0, 0);
            switch (partType * 10 + partType2)
            {
                case 21:
                    partPos = frameData.frontwheel; break;
                case 22:
                    partPos = frameData.rearwheel; break;
                case 31:
                    partPos = frameData.handlebar; break;
                case 41:
                    partPos = frameData.saddle; break;
                case 42:
                    partPos = frameData.seatpost; break;
                case 51:
                    partPos = frameData.apex; break;
                case 52:
                    partPos = frameData.cassette; break;
                case 53:
                    partPos = frameData.chain; break;
                case 54:
                    partPos = frameData.chainring; break;
                case 55:
                    partPos = frameData.forceleft; break;
                case 56:
                    partPos = frameData.forceright; break;
                case 57:
                    partPos = frameData.frontrimbreak; break;
                case 58:
                    partPos = frameData.rearrimbreak; break;
                default: break;
            }
            GameObject.Instantiate(temp, partPos+temp.transform.position+new Vector3(0,0.55f, 0), temp.transform.rotation).transform.parent = cameraPosition.transform;
            totalWeight += float.Parse(weight) / divideCnt;
            totalPrice += float.Parse(price) / divideCnt;
            objNum[partType - 1][partType2 - 1] = true;
        }
    }

    void FrameShow(string str, string weight, string price)
    {
        GameObject temp = Resources.Load<GameObject>("1/" + str);
        frameData = temp.GetComponent<FrameData>();

        if (objNum[0][0] == true)
        {
            Transform[] childList = cameraPosition.GetComponentsInChildren<Transform>();
            if (childList != null)
            {
                for (int i = 1; i < childList.Length; i++)
                {
                    if (childList[i] != transform)
                        Destroy(childList[i].gameObject);
                }
            }
            for (int k = 1; k < objNum.Length; k++)
                for (int j = 0; j < objNum[k].Length; j++)
                    objNum[k][j] = false;
            totalWeight = 0;
            totalPrice = 0;

            objNum[0][0] = false;
        }
        else
        {
            GameObject.Instantiate(temp, temp.transform.position+ new Vector3(0, 0.55f, 0), temp.transform.rotation).transform.parent = cameraPosition.transform;
            totalWeight += float.Parse(weight);
            totalPrice += float.Parse(price);
            objNum[0][0] = true;
        }
    }


    void ShowForAi(string str)
    {
        string[] result = str.Split(new char[] { '/' });
        int value = int.Parse(result[0]);
        if (value == 1106 || value == 1107)
        {
            ChangeShaderTexture(value);
        }
        else
        {
            col1.gameObject.SetActive(false);
            col2.gameObject.SetActive(false);
        }
        int partType = value / 1000;
        if (partType == 1)
        {
            FrameShowForAi(result[0], result[1], result[2]);
            return;
        }
        switch (partType)
        {
            case 2: showCnt = 2; break;
            case 3: showCnt = 1; break;
            case 4: showCnt = 1; break;
            case 5: showCnt = 8; break;
        }

        divideCnt = showCnt;
        while (showCnt != 0)
        {
            PartsShowForAi(value.ToString(), result[1], result[2]);
            value += 100;
            showCnt--;
        }
        switch (str[0])
        {
            case '2': cal_price[0] = Convert.ToSingle(result[2].ToString()); cal_weight[0] = Convert.ToSingle(result[1].ToString()); break;
            case '3': cal_price[1] = Convert.ToSingle(result[2].ToString()); cal_weight[1] = Convert.ToSingle(result[1].ToString()); break;
            case '4': cal_price[2] = Convert.ToSingle(result[2].ToString()); cal_weight[2] = Convert.ToSingle(result[1].ToString()); break;
            case '5': cal_price[3] = Convert.ToSingle(result[2].ToString()); cal_weight[3] = Convert.ToSingle(result[1].ToString()); break;
        }

    }
    void PartsShowForAi(string str, string weight, string price)
    {
        int value = int.Parse(str);
        int partType = value / 1000;
        int partType2 = (value % 1000) / 100;
        GameObject temp = Resources.Load<GameObject>(partType.ToString() + "/" + str);

        if (objNum[partType - 1][partType2 - 1] == true)
        {
            string tempForDestroy = "";
            switch (str[0])
            {
                case '2':
                    tempForDestroy = "wheelset";
                    totalWeight -= cal_weight[0] / divideCnt;
                    totalPrice -= cal_price[0] / divideCnt; break;
                case '3':
                    tempForDestroy = "handlebar";
                    totalWeight -= cal_weight[1] / divideCnt;
                    totalPrice -= cal_price[1] / divideCnt; break;
                case '4':
                    tempForDestroy = "saddle";
                    totalWeight -= cal_weight[2] / divideCnt;
                    totalPrice -= cal_price[2] / divideCnt; break;
                case '5':
                    tempForDestroy = "groupset";
                    totalWeight -= cal_weight[3] / divideCnt;
                    totalPrice -= cal_price[3] / divideCnt; break;
            }
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tempForDestroy);
            for (int i = 0; i < objects.Length; i++)
                Destroy(objects[i]);
            objNum[partType - 1][partType2 - 1] = false;
        }
        else
        {
            Vector3 partPos = new Vector3(0, 0, 0);
            switch (partType * 10 + partType2)
            {
                case 51:
                    partPos = new Vector3(0.097f,-0.167f+ 0.4f, -0.143f); break;
                case 52:
                    partPos = new Vector3(-0.02f, -0.412f+ 0.4f, 0.03f); break;
                case 53:
                    partPos = new Vector3(-0.072f, -0.575f+ 0.4f, 0.106f); break;
                case 54:
                    partPos = new Vector3(0.081f, -0.563f+ 0.4f, -0.119f); break;
                case 55:
                    partPos = new Vector3(-0.11f, -0.297f+ 0.4f, 0.307f); break;
                case 56:
                    partPos = new Vector3(-0.159f, -0.297f+ 0.4f, 0.304f); break;
                case 57:
                    partPos = new Vector3(-0.0545f, -0.167f+ 0.4f, 0.0794f); ; break;
                case 58:
                    partPos = new Vector3(-0.132f, -0.169f+0.4f, 0.195f); break;
                default: break;
            }
            GameObject.Instantiate(temp, partPos + temp.transform.position + new Vector3(0, 0.55f, 0), temp.transform.rotation).transform.parent = cameraPosition.transform;
            totalWeight += float.Parse(weight) / divideCnt;
            totalPrice += float.Parse(price) / divideCnt;
            objNum[partType - 1][partType2 - 1] = true;
        }
    }

    void FrameShowForAi(string str, string weight, string price)
    {
        GameObject temp = Resources.Load<GameObject>("1/" + str); 
        frameData = temp.GetComponent<FrameData>();

        if (objNum[0][0] == true)
        {
            Transform[] childList = cameraPosition.GetComponentsInChildren<Transform>();
            if (childList != null)
            {
                for (int i = 1; i < childList.Length; i++)
                {
                    if (childList[i] != transform)
                        Destroy(childList[i].gameObject);
                }
            }
            for (int k = 1; k < objNum.Length; k++)
                for (int j = 0; j < objNum[k].Length; j++)
                    objNum[k][j] = false;
            totalWeight = 0;
            totalPrice = 0;

            objNum[0][0] = false;
        }
        else
        {
            GameObject.Instantiate(temp, temp.transform.position + new Vector3(0, 0.55f, 0), temp.transform.rotation).transform.parent = cameraPosition.transform;
            totalWeight += float.Parse(weight);
            totalPrice += float.Parse(price);
            objNum[0][0] = true;
        }
    }

    void SetUi()
    {
        if (totalWeight == 0)
            weightText.text = "무게 : ";
        else
            weightText.text = "무게 : " + string.Format("{0:0.##}", totalWeight) + " kg";

        if (totalPrice == 0)
            priceText.text = "가격 : ";
        else
            priceText.text = "가격 : " + string.Format("{0:#,###}", totalPrice) + " ₩";
    }

    public void ChangeTextColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        weightText.color = color;
        priceText.color = color;
    }

    public void ChangeShaderTexture(int value)
    {
        if (value == 1106)
        {
            col1.gameObject.SetActive(true);
            col2.gameObject.SetActive(true);
            Color color;
            Color color2;
            ColorUtility.TryParseHtmlString("#1b50ac", out color);
            ColorUtility.TryParseHtmlString("#01d90a", out color2);
            col1.GetComponent<Image>().color = color;
            col2.GetComponent<Image>().color = color2;
            btn_state = 1;

        }
        else if (value == 1107)
        {
            col1.gameObject.SetActive(true);
            col2.gameObject.SetActive(true);
            Color color;
            Color color2;
            ColorUtility.TryParseHtmlString("#0aa2ba", out color);
            ColorUtility.TryParseHtmlString("#da9a47", out color2);
            col1.GetComponent<Image>().color = color;
            col2.GetComponent<Image>().color = color2;
            btn_state = 2;
        }

    }

    public void FrameColor(int num)
    {
        if (btn_state==1)
        {
            GameObject a = GameObject.Find("1106" + "(Clone)");
            if (num == 1)
                a.GetComponent<MeshRenderer>().material = materials[0];
            else
                a.GetComponent<MeshRenderer>().material = materials[1];
        }
        else
        {
            GameObject a = GameObject.Find("1107" + "(Clone)");
            if (num == 1)
                a.GetComponent<MeshRenderer>().material = materials[2];
            else
                a.GetComponent<MeshRenderer>().material = materials[3];
        }
    }
}
