using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	[SerializeField]
	private Camera[] cameras;

	[SerializeField]
	private Camera playerCam;

	[SerializeField]
	private LineRendererSupport lRendSup;
	private int camIndex; // 0:cameraなし, 1~:各カメラ

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

	}

	private void SwitchActiveCamera(ref Camera[] cameras, ref int camIndex)
	{
		if (camIndex > 0)
			cameras [camIndex-1].gameObject.SetActive (false);
		
		camIndex = (camIndex+1) % (cameras.Length+1); // next camera index

		if (camIndex > 0)
			cameras [camIndex-1].gameObject.SetActive (true);

	}
}
