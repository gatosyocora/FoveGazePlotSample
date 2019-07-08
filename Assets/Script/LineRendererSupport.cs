using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ref : https://note.mu/macgyverthink/n/n3278ee335850

public class LineRendererSupport : MonoBehaviour {

	LineRenderer lRenderer;
	int vCount;
	Vector3 nowPosition;
	Vector3 prePosition;

	[SerializeField]
	private Material sphereMat;

	[SerializeField]
	private Transform spheresParentTrans;

	const int INVISIBLE_LAYER_NUMBER = 8;

	float diffDist;
	GameObject obj;
	private Vector3 objScale = new Vector3 (0.05f, 0.05f, 0.05f);

	public float maxLineLength = 0.01f;

	public float stopViewAreaRadius = 0.01f;
	bool isAleadyCreatedObj = false;

	private int sCount;
	private GameObject newSphere;
	private const int maxVNumForLastColor = 256;

	private bool allowDrawingGaizPlot;

	private Vector3 incrementScaleValue = new Vector3(0.001f, 0.001f, 0.001f);

	// Use this for initialization
	void Start () {
		lRenderer = this.gameObject.GetComponent<LineRenderer>();

		vCount = 0;
		sCount = 0;

		prePosition = transform.position;

		allowDrawingGaizPlot = true;

		// 線の間に表示されるオブジェクトの設定
		obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		var collider = obj.GetComponent<Collider> ();
		if (collider != null) Object.Destroy (collider);
		obj.transform.position = transform.position;
		obj.transform.localScale = objScale;
		obj.layer = INVISIBLE_LAYER_NUMBER;
		obj.GetComponent<MeshRenderer>().material = new Material(sphereMat);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!allowDrawingGaizPlot) return;
		
		nowPosition = transform.position;
		diffDist = Vector3.Distance (prePosition, nowPosition);

		/*
		// draw line
		if (diffDist < maxLineLength) 
		{
			vCount++;
			lRenderer.positionCount = vCount;
			lRenderer.SetPosition (vCount - 1, nowPosition);
		}

		// draw sphere
		if (diffDist < stopViewAreaRadius) {

			if (!isAleadyCreatedObj) {
				sCount++;
				newSphere = Instantiate (obj, nowPosition, Quaternion.identity);
				newSphere.GetComponent<MeshRenderer> ().material.color = FromHueToColor (sCount / (maxVNumForLastColor * 1f) - Mathf.Floor (sCount / (maxVNumForLastColor * 1f)));
				newSphere.transform.SetParent (spheresParentTrans);
				isAleadyCreatedObj = true;
			} 
			else {
				newSphere.transform.localScale += incrementScaleValue;
			}

		} 
		else {
			isAleadyCreatedObj = false;
		}

		prePosition = transform.position;
		*/

		// draw new line & sphere
		if (diffDist > stopViewAreaRadius) {

			// line
			vCount++;
			lRenderer.positionCount = vCount;
			lRenderer.SetPosition (vCount - 1, nowPosition);

			// sphere
			sCount++;
			newSphere = Instantiate (obj, nowPosition, Quaternion.identity);
			var colorLevel0To1 = sCount / (maxVNumForLastColor * 1f) - Mathf.Floor (sCount / (maxVNumForLastColor * 1f));
			newSphere.GetComponent<MeshRenderer> ().material.color = FromHueToColor (colorLevel0To1);
			newSphere.transform.SetParent (spheresParentTrans);

		}
		else 
		{
			newSphere.transform.localScale += incrementScaleValue;
		}

		prePosition = transform.position;

	}


	private Color LerpColor(Color firstColor, Color lastColor, float value){
		var r = Mathf.Lerp (firstColor.r, lastColor.r, value);
		var g = Mathf.Lerp (firstColor.g, lastColor.g, value);
		var b = Mathf.Lerp (firstColor.g, lastColor.b, value);
		var a = Mathf.Lerp (firstColor.a, lastColor.a, value);
		return new Color (r, g, b, a);
	}

	public void ResetColor(){
		sCount = 0;
	}

	public void ResetGaizPlot(){
		ResetColor ();
		vCount = 0;
		lRenderer.positionCount = 0;
		for (int i = 0; i < spheresParentTrans.childCount; i++) {
			GameObject.Destroy (spheresParentTrans.GetChild(i).gameObject);
		}
		isAleadyCreatedObj = false;
	}

	public void SetPlayerCamVisible(ref Camera playerCam, bool isVisible){

		var playerCamTrans = playerCam.gameObject.transform;
		var cam1 = playerCamTrans.GetChild(0).gameObject.GetComponent<Camera>();
		var cam2 = playerCamTrans.GetChild (1).gameObject.GetComponent<Camera>();

		if (isVisible) {
			cam1.cullingMask |= (1 << INVISIBLE_LAYER_NUMBER);
			cam2.cullingMask |= (1 << INVISIBLE_LAYER_NUMBER);
		} 
		else {
			cam1.cullingMask &= ~(1 << INVISIBLE_LAYER_NUMBER);
			cam2.cullingMask &= ~(1 << INVISIBLE_LAYER_NUMBER);
		}
	}
		
	public void SetAllowDrawingGaizPlot(bool allow){
		allowDrawingGaizPlot = allow;
	}

	private Color FromHueToColor(float h){
		float r, g, b;
		float v = 1f, s = 1f;
		int i = (int)Mathf.Floor(h*6f);
		float f = 6f*h - i;
		float p = v * (1f - s);
		float q;

		if (i%2==0)
			q = v * (1f - (1f - f) * s);
		else
			q = v * (1f - f * s);

		Debug.Log (i);

		switch (i) 
		{
			case 0:
				r = v;
				g = q;
				b = p;
				break;
			case 1:
				r = q;
				g = v;
				b = p;
				break;
			case 2:
				r = p;
				g = v;
				b = q;
				break;
			case 3:
				r = p;
				g = q;
				b = v;
				break;
			case 4:
				r = q;
				g = p;
				b = v;
				break;
			case 5:
				r = v;
				g = p;
				b = q;
				break;
		default:
				r = 0;
				g = 0;
				b = 0;
				break;
		}

		return new Color (r, g, b, 1f);

	}
}
