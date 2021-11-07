using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCameraOnUGUIOrGameObject : MonoBehaviour
{
    public RawImage rawImage;//Camera rendering UI
    public GameObject quad;//Camera render GameObject
    private WebCamTexture webCamTexture;

    void Start()
    {
        ToOpenCamera();
    }

    /// <summary>
    /// Turn on the camera
    /// </summary>
    public void ToOpenCamera()
    {
        StartCoroutine("OpenCamera");
    }
    public IEnumerator OpenCamera()
    {

        int maxl = Screen.width;
        if (Screen.height > Screen.width)
        {
            maxl = Screen.height;
        }

        // Applying for Camera Permission
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
            }

            //Open the render
            if (rawImage != null)
            {
                rawImage.gameObject.SetActive(true);
            }
            if (quad != null)
            {
                quad.gameObject.SetActive(true);
            }

            // Monitor the first authorization, whether the device was obtained (because it is likely that the first authorization, but did not obtain the device, this is avoided)
            // If the device is not obtained for many times, it may be true that there is no camera
            int i = 0;
            while (WebCamTexture.devices.Length <= 0 && 1 < 300)
            {
                yield return new WaitForEndOfFrame();
                i++;
            }
            WebCamDevice[] devices = WebCamTexture.devices;//Obtaining available Devices
            if (WebCamTexture.devices.Length <= 0)
            {
                Debug.LogError("没有摄像头设备，请检查");
            }
            else
            {
                string devicename = devices[0].name;
                webCamTexture = new WebCamTexture(devicename, maxl, maxl == Screen.height ? Screen.width : Screen.height, 30)
                {
                    wrapMode = TextureWrapMode.Repeat
                };

                // Render to the UI or game object
                if (rawImage != null)
                {
                    rawImage.texture = webCamTexture;
                }

                if (quad != null)
                {
                    quad.GetComponent<Renderer>().material.mainTexture = webCamTexture;
                }


                webCamTexture.Play();
            }

        }
        else
        {
            Debug.LogError("未获得读取摄像头权限");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        // Pause the camera while the app is paused, and continue using it when it continues
        if (webCamTexture != null)
        {
            if (pause)
            {
                webCamTexture.Pause();
            }
            else
            {
                webCamTexture.Play();
            }
        }

    }


    private void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}