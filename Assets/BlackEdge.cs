using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackEdge : MonoBehaviour
{
    /// <summary>
    /// 开发屏幕的宽
    /// </summary>
    public static float DevelopWidth = 1280f;

    /// <summary>
    /// 开发屏幕的长
    /// </summary>
    public static float DevelopHeigh = 720f;

    /// <summary>
    /// 开发高宽比
    /// </summary>
    public static float DevelopRate = DevelopHeigh / DevelopWidth;

    /// <summary>
    /// 设备自身的高
    /// </summary>
    public static int curScreenHeight = Screen.height;

    /// <summary>
    /// 设备自身的高
    /// </summary>
    public static int curScreenWidth = Screen.width;

    /// <summary>
    /// 当前屏幕高宽比
    /// </summary>
    public static float ScreenRate = (float)Screen.height / (float)Screen.width;

    /// <summary>
    /// 世界摄像机rect高的比例
    /// </summary>
    public static float cameraRectHeightRate = DevelopHeigh / ((DevelopWidth / Screen.width) * Screen.height);

    /// <summary>
    /// 世界摄像机rect宽的比例
    /// </summary>
    public static float cameraRectWidthRate = DevelopWidth / ((DevelopHeigh / Screen.height) * Screen.width);

    private void Awake()
    {
        FitCamera(GetComponent<Camera>());
    }

    public void FitCamera(Camera camera)
    {
        ///适配屏幕。实际屏幕比例<=开发比例的 上下黑  反之左右黑
        if (DevelopRate <= ScreenRate)
        {
            camera.rect = new Rect(0, (1 - cameraRectHeightRate) / 2, 1, cameraRectHeightRate);
        }
        else
        {
            camera.rect = new Rect((1 - cameraRectWidthRate) / 2, 0, cameraRectWidthRate, 1);
        }
    }
}
