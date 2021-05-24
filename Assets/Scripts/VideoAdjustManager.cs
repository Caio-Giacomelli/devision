using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoAdjustManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button metronomeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Text delayText;

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

    void Start(){
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
        PreparaColorBlock(new Color(255, 255, 255));
        metronomeButton.colors = colorBlock;

        shouldHaveClicked = Time.time;

        yield return new WaitForSeconds(flashTime);
        
        PreparaColorBlock(baseColor);
        metronomeButton.colors = colorBlock;
    }

    private void PreparaColorBlock(Color color){
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
    }

    public void SetActualClick(){
        actualClick = Time.time;
        delayMs = (delayMs + (actualClick - shouldHaveClicked)) / 2;
        
        delayText.text = (delayMs*1000).ToString("F2") + " ms";
    }
} 