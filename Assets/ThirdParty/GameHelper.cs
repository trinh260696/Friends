using System;
using UnityEngine;
#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

public class GameHelper  {
    public static void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

	public static long systemCurrentSecond()
    {
		long currTime = (long)(DateTime.Now - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;
		return currTime;
	}

	public static string GetLanguage()
	{
		if (Application.systemLanguage == SystemLanguage.Vietnamese)
		{
			return "VI";
		}
		return "EN";
	}

    public static void Vibration(bool value=false)
    {
        
            Handheld.Vibrate();
             
    }

    public static float GetUpSafeArea()
    {
        return (Screen.height - Screen.safeArea.y - Screen.safeArea.height);
    }

    public static float GetLeftSafeArea()
    {
        return Screen.safeArea.x;
    }

    public static float GetRightSafeArea()
    {
        return (Screen.width-Screen.safeArea.x-Screen.safeArea.width);
    }

    public static void LogFirEventLevelStart(string text)
    {
        Debug.Log(text);
    }

    public static void LogFirEventLevelEnd(string text, bool isSucess)
    {
        Debug.Log(text);
    }

    public static void LogFirEventSelectContent(string contentType, string itemId)
    {
        Debug.Log(contentType+":"+itemId);
    }
    public static float GetBottomSafeArea()
    {
        return Screen.safeArea.y;
    }

#if UNITY_ANDROID
	public static void ShareApp(string url)
	{
		
	}

    public static void RateApp()
	{
		
	}

	public static void ShowLeardBoard(int idLd)
	{
		
	}

	public static void SumitScore(int score, int idLd)
	{
		
	}

#elif UNITY_IPHONE
    public static void ShareApp(string url)
	{
        
    }

    public static void RateApp()
    {
        
    }

	public static void ShowLeardBoard(int idLd)
	{
        
    }

	public static void SumitScore(int score, int idLd)
	{
        
    }
#endif
}
