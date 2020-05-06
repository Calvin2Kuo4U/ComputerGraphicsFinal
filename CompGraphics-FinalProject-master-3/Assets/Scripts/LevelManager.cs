using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	
	public string LevelName;

	private const float fadeTime = 2f;			//Time for text fade in and fade out.
	private GUIStyle levelTextStyle;
	private Color levelTextColor;
	private Rect displayArea;

    void Start()
    {
        //initialize the GUIStyle and other needed variables
		this.levelTextStyle = new GUIStyle();
		this.levelTextColor = new Color(0, 0, 0, 0);	//alpha starts at 0 for fade in.
		this.levelTextStyle.normal.textColor = levelTextColor;
		this.levelTextStyle.fontSize = Mathf.RoundToInt(100*(Screen.height/1080f));
		this.levelTextStyle.alignment = TextAnchor.UpperCenter;
		this.displayArea = new Rect(0, 0, Screen.width, Screen.height);
		StartCoroutine(TextFade());
    }

	private void OnGUI()
	{
		GUI.Label(displayArea, LevelName, levelTextStyle);
	}

	private IEnumerator TextFade(){
		float progressedTime = 0f;
		//Fade in
		while(progressedTime < fadeTime){
			levelTextColor.a = progressedTime/fadeTime;
			levelTextStyle.normal.textColor = levelTextColor;
			yield return null;
			progressedTime += Time.deltaTime;
		}
		//Text linger at full opacity
		levelTextColor.a = 1;
		levelTextStyle.normal.textColor = levelTextColor;
		yield return new WaitForSeconds(fadeTime*2);
		//Text fade out
		progressedTime = 0f;
		while(progressedTime < fadeTime){
			levelTextColor.a = 1 - progressedTime/fadeTime;
			levelTextStyle.normal.textColor = levelTextColor;
			yield return null;
			progressedTime += Time.deltaTime;
		}
		levelTextColor.a = 0;
		levelTextStyle.normal.textColor = levelTextColor;
	}
}
