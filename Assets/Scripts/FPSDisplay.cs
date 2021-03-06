using UnityEngine;
 
public class FPSDisplay : MonoBehaviour
{
	float _deltaTime = 0.0f;

    void Start(){
        Application.targetFrameRate = 144;
    }
 
	void Update()
	{
		_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, 0, w + 4f, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 80;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		
		float msec = _deltaTime * 1000.0f;
		float fps = 1.0f / _deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		
		GUI.Label(rect, text, style);
	}
}