using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	[SerializeField]
	private Camera[] cameras;

	[SerializeField]
	private Camera playerCam;

	[SerializeField]
	private LineRendererSupport lRendSup;
	private int camIndex; // 0:cameraなし, 1~:各カメラ

    [SerializeField]
    private string screenShootOutputPath = string.Empty;

	private bool playerIsVisibleGaizPlot;
	private bool isRunningToDrawGaizPlot;

	// Use this for initialization
	void Start () {
		camIndex = 0;

		playerIsVisibleGaizPlot = false;

		isRunningToDrawGaizPlot = true;
	}
	
	// Update is called once per frame
	void Update () {
		
		// switch camera
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			SwitchActiveCamera (ref cameras, ref camIndex);
		}

		if (Input.GetKeyDown (KeyCode.C)) 
		{
			lRendSup.ResetColor ();
		}

		if (Input.GetKeyDown (KeyCode.R)) 
		{
			lRendSup.ResetGaizPlot ();
		}

		if (Input.GetKeyDown (KeyCode.X)) 
		{
			playerIsVisibleGaizPlot = !playerIsVisibleGaizPlot;
			lRendSup.SetPlayerCamVisible (ref playerCam, playerIsVisibleGaizPlot);
		}

		if (Input.GetKeyDown (KeyCode.S)) 
		{
			isRunningToDrawGaizPlot = !isRunningToDrawGaizPlot;
			lRendSup.SetAllowDrawingGaizPlot (isRunningToDrawGaizPlot);
		}

        if (Input.GetKeyDown(KeyCode.P))
        {
            // HMD用のカメラのときは書き出さない
            if (camIndex <= 0) return;

            string path = screenShootOutputPath;
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets" + Path.DirectorySeparatorChar;
            }
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar;
            }
            path += DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png";
            ScreenShot(cameras[camIndex-1], path);

            Debug.Log("ScreenShot "+path);
        }

	}
    
    // カメラを切り替える
    // CamIndex==0はHMDのカメラなので切り替えないようにする
	private void SwitchActiveCamera(ref Camera[] cameras, ref int camIndex)
	{
		if (camIndex > 0)
			cameras [camIndex-1].gameObject.SetActive (false);
		
		camIndex = (camIndex+1) % (cameras.Length+1); // next camera index

		if (camIndex > 0)
			cameras [camIndex-1].gameObject.SetActive (true);

	}

    // GameViewの解像度のサイズになるので注意
    // Unityプロジェクト内に書き出した場合は書き出し後にAssetDatabase.Refreshが走らないと
    // 画像がProjectタブに表示されないので注意
    private void ScreenShot(Camera camera, string path)
    {
        var screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        var renderTexture = new RenderTexture(screenShot.width, screenShot.height, 24);
        var previousTexture = camera.targetTexture;
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = previousTexture;
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        screenShot.Apply();

        var bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);
        
        File.WriteAllBytes(path, bytes);
    }
}
