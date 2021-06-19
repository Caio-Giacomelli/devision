using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoAdjustManager : MonoBehaviour{
    
    [Header("UI")]
    [SerializeField] private Button _metronomeButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private InputField _delayInput;

    [Header("Video Calibration Settings")]
    [SerializeField] private float _repeatTime;
    [SerializeField] private float _flashTime;
    [SerializeField] private float _delayStartTime;
    [SerializeField] private float _totalCalibrationTime;

    [Header("Color Visualization")]
    [SerializeField] private Color _baseColor;
    [SerializeField] private ColorBlock _colorBlock;

    private float _shouldHaveClicked;
    private float _actualClick;
    private float _delayMs;
    private List<float> _listDelay;

    void Start(){
        _listDelay = new List<float>();
        _delayInput.text = (PlayerPrefs.GetFloat("VideoDelay") * 1000).ToString();
        _colorBlock = _metronomeButton.colors;
        _baseColor = _colorBlock.normalColor;
    }

    public void StartVideoCalibration(){
        InvokeRepeating("FlashMetronomeButton", _delayStartTime, _repeatTime);
        StartCoroutine(FinishVideoCalibration());
    }

    private void FlashMetronomeButton(){
        _metronomeButton.interactable = true;
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine(){     
        PrepareColorBlock(new Color(255, 255, 255));
        _metronomeButton.colors = _colorBlock;

        _shouldHaveClicked = Time.time;

        yield return new WaitForSeconds(_flashTime);
        
        PrepareColorBlock(_baseColor);
        _metronomeButton.colors = _colorBlock;
    }

    private void PrepareColorBlock(Color color){
        _colorBlock.normalColor = color;
        _colorBlock.disabledColor = color;
        _colorBlock.highlightedColor = color;
        _colorBlock.pressedColor = color;
        _colorBlock.selectedColor = color;
    }

    IEnumerator FinishVideoCalibration(){
        yield return new WaitForSeconds(_totalCalibrationTime);
        CancelInvoke();
        _startButton.interactable = true;
        _metronomeButton.interactable = false;

        CalculateMedianDelay();
        
        _delayInput.text = _delayMs.ToString();
    }

    private void CalculateMedianDelay(){
        float medianDelay = 0;
        foreach (float delay in _listDelay)
        {
            medianDelay += delay;
        }

        _delayMs = (medianDelay / _listDelay.Count) * 1000;
    }

    public void SetActualClick(){
        _actualClick = Time.time;    
        _listDelay.Add(_actualClick - _shouldHaveClicked); 
    }

    public void SubmitDelayInput(string input){
        PlayerPrefs.SetFloat("VideoDelay", float.Parse(input) / 1000);
    }
} 