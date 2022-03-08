using UnityEngine;

public class onesignal : MonoBehaviour
{
    // Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {

    }

    private void Start()
    {
        // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
        // OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);
        OneSignal.StartInit("b2f7f966-d8cc-11e4-bed1-df8f05be55ba")
          .HandleNotificationOpened(HandleNotificationOpened)
          .EndInit();
        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
    }
}
