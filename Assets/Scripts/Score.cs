using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField]
    private string name_score;

    void Update()
    {
        // Player Pref is an unity class that stores player preference information, used for now for score system. using an empty string for casting reasons
        GetComponent<Text>().text = PlayerPrefs.GetInt(name_score)+"";
    }
}
