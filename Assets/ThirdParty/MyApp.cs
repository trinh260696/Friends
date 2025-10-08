using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using NewLibrary;
#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

public class MyApp : SingletonX<MyApp>
{
    public delegate void LoadStartCB();
    public delegate void LoadFailCB();
    public delegate void LoadSuccessCB(byte[] text);

    [HideInInspector]
    public int mOpenAtt = -1;                   //-1: chua co du lieu, 0: ko hoi, 1: hoi
    [HideInInspector]
    public int mCfAtt = 1;                      //0: ko show dialog, 1:show dialog
    [HideInInspector]
    private bool mIsRqAttSv = false;            //da request data hay chua
    [HideInInspector]
    public float[] mPadding = {0, 0, 0, 0 };    //vung an toan // trai - tren - phai- duoi
    [HideInInspector]
    public bool mIsRemoveAds = false;           //da mua goi remote ads
    [HideInInspector]
    public string priceIAP = "";                //gia cac goi inapp
    private int mTypeInfoIAP = -1;              //dung khi request thong tin cac goi
    private int mIdInapp = 0;                   //bien tang trong android
    private List<ModelReceipt> mListReceipt;    //mang receiption
    private bool mIsRunUdIAP = false;           //co chay update IAP
    private bool mIsProcessReceipt = false;     //dang goi api xac thuc

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.mOpenAtt = -1;
            instance.mCfAtt = 1;
            instance.mIsRqAttSv = false;
            int sttRemoveAds = PlayerPrefs.GetInt(Config.KEY_REMOVE_ADS, 0);
            if (sttRemoveAds == 1)
            {
                instance.mIsRemoveAds = true;
            }
            else
            {
                instance.mIsRemoveAds = false;
            }
            mListReceipt = new List<ModelReceipt>();
            mListReceipt.Clear();
            mIsRunUdIAP = false;
            mIsProcessReceipt = false;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
#if UNITY_ANDROID
#else
        Application.targetFrameRate = 60;
#endif
        //Tinh toan padding
        mPadding[0] = Mathf.Max(0, GameHelper.GetLeftSafeArea());
        mPadding[1] = Mathf.Max(0, GameHelper.GetUpSafeArea());
        mPadding[2] = Mathf.Max(0, GameHelper.GetRightSafeArea());
        mPadding[3] = Mathf.Max(0, GameHelper.GetBottomSafeArea());

        requestUdPr();
        StartCoroutine(callRqAttSv());
    }

    private void Update()
    {
        
    }

    private IEnumerator callRqAttSv()
    {
        yield return new WaitForSeconds(20.0f);
        rqTrackAttSv();
    }

    #region REQUEST DATA
    public void LoadData(string text, LoadStartCB stCB, LoadFailCB failCB, LoadSuccessCB succCB)
    {
        StartCoroutine(callLoadData(text, stCB, failCB, succCB));
    }

    private IEnumerator callLoadData(string text, LoadStartCB stCB, LoadFailCB failCB, LoadSuccessCB succCB)
    {
        if (stCB != null)
        {
            stCB();
        }
        UnityWebRequest www = UnityWebRequest.Get(text);
        www.timeout = 10;
        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error) || www.isNetworkError || www.isHttpError)
        {
            if (failCB != null)
            {
                failCB();
            }
        }
        else
        {
            byte[] dtText = www.downloadHandler.data;
            if (succCB != null)
            {
                succCB(dtText);
            }
        }
    }

    public void PostData(string url, string text, LoadStartCB stCB, LoadFailCB failCB, LoadSuccessCB successCB)
    {
        StartCoroutine(callPostData(url, text, stCB, failCB, successCB));
    }

    private IEnumerator callPostData(string url, string text, LoadStartCB startCB, LoadFailCB failCB, LoadSuccessCB successCB)
    {
        if (startCB != null)
        {
            startCB();
        }
        UnityWebRequest www = UnityWebRequest.PostWwwForm(url, text);
        www.timeout = 10;
        yield return www.SendWebRequest();

        if ((!string.IsNullOrEmpty(www.error)) || (www.isNetworkError) || (www.isHttpError))
        {
            if (failCB != null)
            {
                failCB();
            }
        }
        else
        {
            byte[] dtText = www.downloadHandler.data;
            if (successCB != null)
            {
                successCB(dtText);
            }
        }
    }

    public void LoadImage(string image, string fileName)
    {
        string folder = (Application.persistentDataPath + "/Images");
        string fullName = (folder + "/" + fileName);
        if (!File.Exists(fullName))
        {
            StartCoroutine(callLoadImage(image, folder, fullName));
        }
    }

    private IEnumerator callLoadImage(string image, string folder, string fullName)
    {
        UnityWebRequest www = UnityWebRequest.Get(image);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {

        }
        else
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            byte[] results = www.downloadHandler.data;
            File.WriteAllBytes(fullName, results);
        }
    }
    #endregion

    #region IAP
    public void RequestInfoIAP(int type)
    {
        priceIAP = "PACK_IAP_01=1.99$-PACK_IAP_02=4.99$-PACK_IAP_03=0.99$";
        if (priceIAP.Length > 0)
        {
            ShowDialogIAP(type);
            return;
        }
    }

    public void PurchaseItem(string package)
    {
        //xxx
        bool check = true;
        if (check)
        {
            OnPurchaseSucces(package);
            return;
        }
    }

    public void Restore()
    {
        string packName = "";
        OnPurchaseSucces(packName);
    }

    private void OnGetInfoInApp(int status, string text)
    {
        //xxx Doi theo tung game
        //status: -2_"Your device does not support this feature.", -1_"No data retrieved. Please check network connection and try again."
        Loom.Instance.QueueOnMainThread(() => {
            if ((status == -2) || (status == -1))
            {
                return;
            }
            if (status == 1)
            {
                priceIAP = text;
                ShowDialogIAP(mTypeInfoIAP);
            }
        });
    }

    private void OnResultProduct(int status, string sku, string token, string transId)
    {
        //xxx doi theo tung game
        //status:   -4: "Please force close, then turn on the game and check again."
        //          -3: "Transaction failed."
        //          -2: "Your device does not support this feature."
        //          -1: "No data retrieved. Please check network connection and try again."
        if (status < 0)
        {
            Loom.Instance.QueueOnMainThread(() => {

            });
            return;
        }
        if (status == 0)
        {
            return;
        }
        if ((token.Length == 0) || (sku.Length == 0))
        {
            return;
        }
        bool isExit = false;
        for (int i = 0; i < mListReceipt.Count; i ++)
        {
            if (mListReceipt[i].mToken == token)
            {
                isExit = true;
                break;
            }
        }
        if (isExit == true)
        {
            return;
        }
        ModelReceipt item = new ModelReceipt();
        item.mSku = sku;
        item.mToken = token;
        item.mTransId = transId;
        /* Tren android tu tao tranid nay */
        if (transId.Length == 0)
        {
            item.mTransId = string.Format("{0}", mIdInapp);
            mIdInapp++;
        }
        mListReceipt.Add(item);
        mIsRunUdIAP = true;
        //xxx Hien thi thong bao: "Please wait for transaction validation."
    }

    private void onLoadDataValidateReceipt(byte[] text, string tranId, string sku)
    {
        string str = System.Text.Encoding.UTF8.GetString(text, 0, text.Length);
        var jsObject = JSON.Parse(str);
        int status = jsObject["status"].AsInt;
        string msg = jsObject["msg"];
        if (msg.Length > 0)
        {
            //xxx Thay doi theo tung game - Hien thi toast cua msg
        }
    }

    private void onLoadDataValidateToken(byte[] text, string tranId, string sku, int count)
    {
        string str = System.Text.Encoding.UTF8.GetString(text, 0, text.Length);
        var jsObject = JSON.Parse(str);
        int status = jsObject["status"].AsInt;
        string msg = jsObject["msg"];
        int receiver = jsObject["receiver"].AsInt;
        if ((msg.Length > 0) && ((count == 0) || (status == 1) || (status == 0)))
        {
            //xxx Thay doi theo tung game - Hien thi toast cua msg
        }
    }

    private void ShowDialogIAP(int type)
    {
        //xxx Thay doi theo tung game
        //string[] packIAPs = priceIAP.Split('-','=');
        //if (type == 1)
        //{
        //    PopupNoAds popupNoAds;
        //    PopupUI.Instance.Show<PopupNoAds>(out popupNoAds, null, Menu<PopupUI>.ShowType.NotHide);
        //    popupNoAds.Initialized(GetPricePackIAP(packIAPs, Config.IAP1));
        //}
        //if (type == 2)
        //{
        //    PopupBundle popupBundle;
        //    PopupUI.Instance.Show<PopupBundle>(out popupBundle, null, Menu<PopupUI>.ShowType.NotHide);
        //    popupBundle.Initialized(GetPricePackIAP(packIAPs, Config.IAP2));
        //}
        //if (type == 3)
        //{
        //    PopupMoreHeart popupMoreHeart;
        //    PopupUI.Instance.Show<PopupMoreHeart>(out popupMoreHeart, null, Menu<PopupUI>.ShowType.NotHide);
        //    popupMoreHeart.Initialized(GetPricePackIAP(packIAPs, Config.IAP3));
        //}
    }

    private string GetPricePackIAP(string[] packIAPs, string package)
    {
        for(int i = 0; i < packIAPs.Length; i++)
        {
            if((i % 2) != 0)
            {
                continue;
            }
            if(!packIAPs[i].Equals(package))
            {
                continue;
            }
            return packIAPs[i+1];
        }
        return "";
    }

    private void OnPurchaseSucces(string package)
    {
        ////xxx Thay doi theo tung game
        //var data = ShopSkinsData.Instance.IAPs.Find(x => x.ID.Equals(package));
        //var skins = ShopSkinsData.Instance.SkinDatas;
        //// luu lai data 
        //if (data.IsBundle)
        //{
        //    UserData.main.ActiveBundle = data.IsBundle;
        //}
        //if (data.UnlockAllCustome)
        //{
        //    for (int i = 0; i < skins.Count; i++)
        //    {
        //        UserData.main.SetUnlockSkinBuyID(skins[i].ID, true);
        //    }
        //}
        //if (data.UnlockNoAds)
        //{
        //    UserData.main.ActiveRemoveAds = data.UnlockNoAds;
        //}
        //if (data.UnLimitedHeart)
        //{
        //    UserData.main.data.IsUnlimited = data.UnLimitedHeart;
        //    UserData.main.SaveLocalData();
        //}
        //UserData.main.Canins += data.canins;
        //// cap nhat UI shop SKin
        //Messenger.Broadcast("UpdateCanins", MessengerMode.DONT_REQUIRE_LISTENER);
        //Messenger.Broadcast("UpdateUIItem", MessengerMode.DONT_REQUIRE_LISTENER);
        //// cap nhat UI More Heart
        //if (PopupUI.Instance.Activing(PopupUI.Instance.GetPanelByName<PopupMoreHeart>()))
        //{
        //    PopupUI.Instance.Hide<PopupMoreHeart>();
        //}
        //// cap nhat UI Bundle
        //if (PopupUI.Instance.Activing(PopupUI.Instance.GetPanelByName<PopupBundle>()))
        //{
        //    PopupUI.Instance.Hide<PopupBundle>();
        //}
        //// cap nhat UI RemoveAds
        //if (PopupUI.Instance.Activing(PopupUI.Instance.GetPanelByName<PopupNoAds>()))
        //{
        //    PopupUI.Instance.Hide<PopupNoAds>();
        //}
        //AdsManager.Instance.HideBanner();
        //// cap nhat UI home
        //Messenger.Broadcast("UpdateUIInHome", MessengerMode.DONT_REQUIRE_LISTENER);
        //TxtNotification.Instance.Initialized("Success");
    }
#endregion

#region RATE APP
    public void ShowRateDialog(int add)
    {
        int ver = PlayerPrefs.GetInt(Config.KEY_MAX_VER_FB, 100);
        if (ver < Config.VS_CURR)
        {
            return;
        }
        int isRate = PlayerPrefs.GetInt(Config.KEY_CHECK_RATE, 0);
        if (isRate == 1)
        {
            return;
        }
        int currDay = DateTime.Now.DayOfYear;
        int saveDay = PlayerPrefs.GetInt(Config.KEY_SAVE_DAY_RATE, -1);
        if (currDay == saveDay)
        {
            return;
        }
        int checkDay = PlayerPrefs.GetInt(Config.KEY_SAVE_NUMBER_DATE_RATE, 0);
        int countRate = PlayerPrefs.GetInt(Config.KEY_SAVE_NUMBER_RATE, 0);
        if (checkDay != currDay)
        {
            countRate = 1;
            PlayerPrefs.SetInt(Config.KEY_SAVE_NUMBER_DATE_RATE, currDay);
        }
        else
        {
            countRate += add;
        }
        PlayerPrefs.SetInt(Config.KEY_SAVE_NUMBER_RATE, countRate);
        //xxx Gia tri nay thay doi theo tung game
        if (countRate < 3)
        {
            return;
        }
        //xxx Show Dialog Rate, kiem tra them o 2 dialog rate con lai, khi kich vao rate thi luu bien checkrate = 1

        PlayerPrefs.SetInt(Config.KEY_SAVE_DAY_RATE, currDay);
    }
#endregion

#region PRM
    private void requestUdPr()
    {
        long currTime = GameHelper.systemCurrentSecond();
        if ((currTime - Config.DAY_RW_END) < Config.DEL_DAY)
        {
            int svVer = PlayerPrefs.GetInt(Config.KEY_MAX_VER_FB, 0);
            if (svVer < Config.VS_CURR)
            {
                return;
            }
        }

        string strPrT = PlayerPrefs.GetString(Config.KEY_PRE_GET_DATA, "0");
        long prT = 0;
        bool isParse = long.TryParse(strPrT, out prT);
        if (isParse == false)
        {
            prT = 0;
        }
        if ((currTime-prT) < (12*60*60))
        {
            return;
        }
        List<int> vtIdx = new List<int>();
        vtIdx.Add(0);
        vtIdx.Add(1);
        vtIdx.Add(2);
        callRqDt(vtIdx, false);
    }
#endregion

#region ATT
    public int GetStatusATT()
    {
        return 3;
    }

    public void RequestATT()
    {
    }

    public void OpenTrackATT(int open)
    {
 
    }

    public void ActionATT()
    {
     
    }

    private void rqTrackAttSv()
    {
        if (mOpenAtt != -1)
        {
            return;
        }
        if (mIsRqAttSv == true)
        {
            return;
        }
        mIsRqAttSv = true;

        string strPrT = PlayerPrefs.GetString(Config.KEY_PRE_GET_DATA, "0");
        long prT = 0;
        bool isParse = long.TryParse(strPrT, out prT);
        if (isParse == false)
        {
            prT = 0;
        }
        if ((GameHelper.systemCurrentSecond() - prT) > (12*60*60))
        {
            List<int> vtIdx = new List<int>();
            vtIdx.Add(0);
            vtIdx.Add(1);
            vtIdx.Add(2);
            callRqDt(vtIdx, true);
            return;
        }
        onRqTrackAttSvFail();
    }

    private void onRqTrackAttSvFail()
    {
        int value = PlayerPrefs.GetInt(Config.KEY_OPEN_ATT_SER, -1);
        mCfAtt = PlayerPrefs.GetInt(Config.KEY_CFR_ATT_SER, 1);
        if (value != -1)
        {
            OpenTrackATT(value);
            return;
        }
        value = PlayerPrefs.GetInt(Config.KEY_OPEN_ATT_FIR, -1);
        mCfAtt = PlayerPrefs.GetInt(Config.KEY_CFR_ATT_FIR, 1);
        if (value != -1)
        {
            OpenTrackATT(value);
            return;
        }
        OpenTrackATT(0);
    }

    private void callRqDt(List<int> vtIdx, bool isAtt)
    {
        string[] strs = { Config.API_DT_1, Config.API_DT_2, Config.API_DT_3};
        int rd = UnityEngine.Random.Range(0, vtIdx.Count);
        int index = vtIdx[rd];
        vtIdx.RemoveAt(rd);
        string text = string.Format(strs[index], Config.PLATFORM, GameHelper.GetLanguage(), Config.GAMEID, Config.VS_CURR);
        LoadData(text, null, () => {
            parseDtFail(vtIdx, isAtt);
        }, (byte[] textD) => {
            parseDtSucc(textD, isAtt);
        });
    }

    private void parseDtFail(List<int> vtIdx, bool isAtt)
    {
        if (vtIdx.Count > 0)
        {
            callRqDt(vtIdx, isAtt);
            return;
        }
        if (isAtt == true)
        {
            onRqTrackAttSvFail();
        }
    }

    private void parseDtSucc(byte[] textB, bool isAtt)
    {
        string strCT = string.Format("{0}", GameHelper.systemCurrentSecond());
        PlayerPrefs.SetString(Config.KEY_PRE_GET_DATA, strCT);

        string str = System.Text.Encoding.UTF8.GetString(textB, 0, textB.Length);
        var jsObject = JSON.Parse(str);
        string link = jsObject["link"];
        string text = jsObject["text"];
        string linkpr = jsObject["linkpr"];
        string imgpr = jsObject["imgpr"];
        string cfads = jsObject["setting"];
        string strAtt = jsObject["open_att"];
        string strCft = jsObject["cfatt"];

        int openAtt = -1;
        bool isParse = int.TryParse(strAtt, out openAtt);
        if (isParse == false)
        {
            openAtt = 0;
        }
        else
        {
            PlayerPrefs.SetInt(Config.KEY_OPEN_ATT_SER, openAtt);
        }

        int cfAtt = -1;
        isParse = int.TryParse(strCft, out cfAtt);
        if (isParse == false)
        {
            cfAtt = 1;
        }
        else
        {
            PlayerPrefs.SetInt(Config.KEY_CFR_ATT_SER, cfAtt);
        }

        //xxx Goi DialogUpdate, chu y them tham so text va link
        if ((isAtt == false) && (link.Length > 0))
        {
            
        }

        PlayerPrefs.SetString(Config.KEY_FILENAME_IMG, "");
        if ((isAtt == false) && (linkpr.Length > 0) && (imgpr.Length > 0))
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(imgpr);
            string fileName = Convert.ToBase64String(bytesToEncode);
            LoadImage(imgpr, fileName);
            PlayerPrefs.SetString(Config.KEY_LINK_IMG, linkpr);
            PlayerPrefs.SetString(Config.KEY_FILENAME_IMG, fileName);
        }
        
        if (isAtt == true)
        {
            mCfAtt = cfAtt;
            OpenTrackATT(openAtt);
        }
    }
#endregion

#region CALL TO NATIVE IOS
    public void nativeOnRestoreFail(string text)
    {
        Loom.Instance.QueueOnMainThread(() => {
            //xxx Thay doi theo tung game
            //Thong bao: "Restore fail."
        });
    }
#endregion

    private class ModelReceipt
    {
        public string mSku;     //pakage goi
        public string mToken;   //token
        public string mTransId; //transaction

        public int mCount;      //so lan request
        public long mPreTime;   //thoi gian cua request truoc
        public int mReceiver;   //0_chua tung nhan dc kq tu server, 1_da tung nhan

        public ModelReceipt()
        {
            mSku = "";
            mToken = "";
            mTransId = "";
            mCount = 0;
            mPreTime = 0;
            mReceiver = 0;
        }
    }
}