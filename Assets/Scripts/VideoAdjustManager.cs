using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoAdjustManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button metronomeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private InputField delayInput;

    [Header("Video Calibration Settings")]
    [SerializeField] private float repeatTime;
    [SerializeField] private float flashTime;
    [SerializeField] private float delayStartTime;
    [SerializeField] private float totalCalibrationTime;

    [Header("Color Visualization")]
    [SerializeField] private Color baseColor;
    [SerializeField] private ColorBlock colorBlock;

    [Header("Video Calibration Results")]
    [SerializeField] private float shouldHaveClicked;
    [SerializeField] private float actualClick;
    [SerializeField] private float delayMs;
    [SerializeField] private List<float> listDelay;

    void Start(){
        listDelay = new List<float>();
        delayInput.text = (PlayerPrefs.GetFloat("VideoDelay") * 1000).ToString();
        colorBlock = metronomeButton.colors;
        baseColor = colorBlock.normalColor;
    }

    public void StartVideoCalibration(){
        InvokeRepeating("FlashMetronomeButton", delayStartTime, repeatTime);
        StartCoroutine(FinishMetronomeExecution());
    }

    private void FlashMetronomeButton(){
        metronomeButton.interactable = true;
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine(){     
        PrepareColorBlock(new Color(255, 255, 255));
        metronomeButton.colors = colorBlock;

        shouldHaveClicked = Time.time;

        yield return new WaitForSeconds(flashTime);
        
        PrepareColorBlock(baseColor);
        metronomeButton.colors = colorBlock;
    }

    private void PrepareColorBlock(Color color){
        colorBlock.normalColor = color;
        colorBlock.disabledColor = color;
        colorBlock.highlightedColor = color;
        colorBlock.pressedColor = color;
        colorBlock.selectedColor = color;
    }

    IEnumerator FinishMetronomeExecution(){
        yield return new WaitForSeconds(totalCalibrationTime);
        CancelInvoke();
        startButton.interactable = true;
        metronomeButton.interactable = false;

        CalculateMedianDelay();
        
        delayInput.text = delayMs.ToString();
    }

    private void CalculateMedianDelay(){
        float medianDelay = 0;
        foreach (float delay in listDelay)
        {
            medianDelay += delay;
        }

        delayMs = (medianDelay / listDelay.Count) * 1000;
    }

    public void SetActualClick(){
        actualClick = Time.time;    
        listDelay.Add(actualClick - shouldHaveClicked); 
    }

    public void SubmitDelayInput(string input){
        PlayerPrefs.SetFloat("VideoDelay", float.Parse(input) / 1000);
    }
} 