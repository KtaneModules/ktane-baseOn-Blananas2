using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using UnityEngine.UI;

public class baseOnScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Keypad;
    public KMSelectable Blank;
    public Text Top;
    public Text Bottom;

    private int index;
    private string generatedNumber;
    private string baseName;
    private string yourAnswer = "";
    private int[] rationalNumbers = { 1, 2, 1, 3, 3, 2, 2, 3, 1, 4, 4, 3, 3, 5, 5, 2, 2, 5, 5, 3, 3, 4, 1, 5, 5, 4, 4, 7, 7, 3, 3, 8, 8, 5, 5, 7, 7, 2, 2, 7, 7, 5, 5, 8, 8, 3, 3, 7, 7, 4, 4, 5, 1, 6, 6, 5, 5, 9, 9, 4, 4, 11, 11, 7, 7, 10, 10, 3, 3, 11, 11, 8, 8, 13, 13, 5, 5, 12, 12, 7, 7, 9, 9, 2, 2, 9, 9, 7, 7, 12, 12, 5, 5, 13, 13, 8, 8, 11, 11, 3, 3, 10, 10, 7, 7, 11, 11, 4, 4, 9, 9, 5, 5, 6, 1, 7, 7, 6, 6, 11, 11, 5, 5, 14, 14, 9, 9, 13, 13, 4, 4, 15, 15, 11, 11, 18, 18, 7, 7, 17, 17, 10, 10, 13, 13, 3, 3, 14, 14, 11, 11, 19, 19, 8, 8, 21, 21, 13, 13, 18, 18, 5, 5, 17, 17, 12, 12, 19, 19, 7, 7, 16, 16, 9, 9, 11, 11, 2, 2, 11, 11, 9, 9, 16, 16, 7, 7, 19, 19, 12, 12, 17, 17, 5, 5, 18, 18, 13, 13, 21 };

    private Coroutine buttonHold;
	private bool holding = false;
    float elapsed = 0f;
    float otherElapsed = 0f;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in Keypad) {
            button.OnInteract += delegate () { KeypadPress(button); return false; };
        }

        Blank.OnInteract += delegate () { BlankPress(); return false; };
        Blank.OnInteractEnded += delegate { BlankRelease(); };
    }

    void Start () {
        index = UnityEngine.Random.Range(0, 11111);

        if (index < 10000) {
            generatedNumber = (index+1).ToString();
            baseName = BaseNamingScript.NumberToName(index+1, BaseNamingScript._baseBaseNames.ContainsKey(index+1));
        } else if (index < 11000) {
            generatedNumber = (index-11000).ToString();
            baseName = BaseNamingScript.NumberToName(index-11000);
        } else if (index < 11100) {
            generatedNumber = rationalNumbers[(index-11000)*2] + "/" + rationalNumbers[(index-11000)*2+1];
            baseName = BaseNamingScript.FractionToName(rationalNumbers[(index-11000)*2], rationalNumbers[(index-11000)*2+1]);
        } else if (index < 11110) {
            generatedNumber = "-" + rationalNumbers[(index-11100)*2] + "/" + rationalNumbers[(index-11100)*2+1];
            baseName = BaseNamingScript.FractionToName(-1 * rationalNumbers[(index-11100)*2], rationalNumbers[(index-11100)*2+1]);
        } else {
            generatedNumber = "0".ToString();
            baseName = BaseNamingScript.NumberToName(0, BaseNamingScript._baseBaseNames.ContainsKey(0));
        }

        Bottom.text = baseName;
        Debug.LogFormat("[Base On #{0}] Base name: {1}", moduleId, baseName);
        Debug.LogFormat("[Base On #{0}] Answer: {1}", moduleId, generatedNumber);
    }

    void Update () {
        if (moduleSolved)
            return;

        if (holding) {
            elapsed += Time.deltaTime;
            otherElapsed += Time.deltaTime;
            if (otherElapsed > 1f) {
                otherElapsed -= 1f;
                if (yourAnswer != "") {
                    yourAnswer = yourAnswer.Substring(0, yourAnswer.Length - 1);
                    Top.text = yourAnswer;
                    if (yourAnswer == "")
                        Top.text = "?";
                }
            }
        }
    }

    void KeypadPress(KMSelectable button) {
        button.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        if (moduleSolved)
            return;

        for (int b = 0; b < 12; b++) {
            if (button == Keypad[b]) {
                yourAnswer += ("1234567890-/"[b]);
                Top.text = yourAnswer;
            }
        }
    }

    private IEnumerator BlankPress() {
        holding = true;
        Blank.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        return null;
    }

    private IEnumerator BlankRelease() {
        holding = false;
        if (elapsed < 1f) {
            Submit();
        }
        elapsed = 0f;
        otherElapsed = 0f;

        return null;
    }

    void Submit() {
        if (yourAnswer == generatedNumber) {
            Debug.LogFormat("[Base On #{0}] {1} is correct, module solved.", moduleId, yourAnswer);
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
        } else {
            Debug.LogFormat("[Base On #{0}] {1} is incorrect, strike!", moduleId, yourAnswer);
            GetComponent<KMBombModule>().HandleStrike();
        }
    }

}
