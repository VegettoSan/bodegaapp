using UnityEngine;
using UnityEngine.Android;

public class PermissionManager : MonoBehaviour
{
    void Start()
    {
        CheckAndRequestPermission();
    }

    void CheckAndRequestPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Obtener versión de Android
        using (AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            int sdkInt = versionClass.GetStatic<int>("SDK_INT");

            if (sdkInt >= 30) // Android 11+
            {
                if (!HasManageExternalStoragePermission())
                {
                    OpenManageAllFilesPermissionScreen();
                    Debug.Log("Solicitando permiso MANAGE_EXTERNAL_STORAGE");
                }
                else
                {
                    Debug.Log("Permiso MANAGE_EXTERNAL_STORAGE ya concedido");
                }
            }
            else // Android 6–10
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                }
            }
        }
#endif
    }

    // ✅ Verifica si ya tenemos MANAGE_EXTERNAL_STORAGE
    bool HasManageExternalStoragePermission()
    {
        using (var environment = new AndroidJavaClass("android.os.Environment"))
        {
            return environment.CallStatic<bool>("isExternalStorageManager");
        }
    }

    // ✅ Abre la pantalla de ajustes para conceder MANAGE_EXTERNAL_STORAGE
    void OpenManageAllFilesPermissionScreen()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (var settingsClass = new AndroidJavaClass("android.provider.Settings"))
            {
                string packageName = activity.Call<string>("getPackageName");
                string uriString = "package:" + packageName;
                var uri = uriClass.CallStatic<AndroidJavaObject>("parse", uriString);

                var intent = new AndroidJavaObject("android.content.Intent",
                    settingsClass.GetStatic<string>("ACTION_MANAGE_APP_ALL_FILES_ACCESS_PERMISSION"),
                    uri);

                activity.Call("startActivity", intent);
            }
        }
    }
}
