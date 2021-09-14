using GameAnalyticsSDK;
using Voodoo.Sauce.Internal;
using Voodoo.Sauce.Internal.Analytics;
//using UnityEditor.Build;
//using UnityEditor.Build.Reporting;


public static class TinySauce
{ 
    public const string Version = "4.2.0";
    public static string token = "";
    /// <summary>
    ///  Method to call whenever the user starts a game.
    /// </summary>
    /// <param name="levelNumber">The game Level, this parameter is optional for game without level</param>
    public static void OnGameStarted(string  levelNumber = null)
    {
        AnalyticsManager.OnGameStarted(levelNumber);
    }

    /// <summary>
    /// Method to call whenever the user completes a game.
    /// </summary>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(float score)
    {
        OnGameFinished(null,  score);
    }
    
    /// <summary>
    /// Method to call whenever the user completes a game with levels.
    /// </summary>
    /// <param name="levelNumber">The game Level</param>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(string  levelNumber, float score)
    {
        OnGameFinished(levelNumber,true, score);
    }
    
    
    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelNumber">The game Level</param>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(string  levelNumber, bool levelComplete, float score)
    {
        AnalyticsManager.OnGameFinished(levelComplete, score, levelNumber, null);
    }

    /// <summary>
    /// Call this method to track any custom event you want.
    /// </summary>
    /// <param name="eventName">The name of the event to track</param>
    public static void TrackCustomEvent(string eventName)
    {
        AnalyticsManager.TrackCustomEvent(eventName);
    }
    
    /// <summary>
    /// Call this method to track any custom event you want.
    /// </summary>
    /// <param name="eventName">The name of the event to track</param>
    /// <param name="eventValue">Number value of event.</param>
    public static void TrackCustomEvent(string eventName, float eventValue)
    {
        AnalyticsManager.TrackCustomEvent(eventName, eventValue);
    }

    public static string UpdateAdjustToken(TinySauceSettings settings)
    {
#if UNITY_IOS
        token = settings.adjustIOSToken;
#endif
#if UNITY_ANDROID
        token = settings.adjustAndroidToken;
#endif

        return token;
    }


    public static string getToken()
    {
        TinySauceSettings sauceSettings = TinySauceSettings.Load();
#if UNITY_ANDROID
        return sauceSettings.adjustAndroidToken.Replace(" ", string.Empty);
#endif
#if UNITY_IOS
        return sauceSettings.adjustIOSToken.Replace(" ", string.Empty);
#endif
        return "";

    }

}
