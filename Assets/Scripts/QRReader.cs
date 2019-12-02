using System.Collections;
using System.Collections.Generic;
using BarcodeScanner;
using BarcodeScanner.Scanner;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public enum TurnPhase { Initialisation, Scanning, Sorting, WaitResult }
public enum PlayerTurn {One, Two}
public enum CardType {Null, Rock, Paper, Scissor, Joker}
public enum CardColor { Joker, Blue, Red}

[System.Serializable]
public class SidesManager
{
    public int sideId;
    public Color playerColor;
    public string colorPlayerText;
    public string[] registeredCard = new string[6];
}

[System.Serializable]
public class RowColor{
    public CardColor[] cC = new CardColor[3];
}

public class QRReader : MonoBehaviour {
    [Header("Info on turn state")]
    public int turnNumber = -1;
    public TurnPhase tP;
    public PlayerTurn pT;

    [Header("Info on Player One Turn")]
    //Number of card played by player one in the turn [0,2]
    public int scorePlayerOne = 0;
    public int cardPlayedTurnOne = 0;
    //Cards played by player one this turn 
    public CardType[] cTOne = new CardType[3];
    //Color of cards played this turn by player One
    public CardColor[] cCOne = new CardColor[3];
    //Array of each row played by P1 for each turn played(actual maw : 4 turn)
    public RowColor[] rowOne = new RowColor[4];

    [Header("Info on Player Two turn")]
    public int scorePlayerTwo = 0;
    //Number of card played by player two in  the turn [0,2]
    public int cardPlayedTurnTwo = -1;
    //Cards played by player two this turn
    public CardType[] cTTwo = new CardType[3];
    //Color of cards played this turn by player two
    public CardColor[] cCTwo = new CardColor[3];
    //Array of each row player by P2 for each turn
    public RowColor[] rowTwo = new RowColor[4];


    [Header("Utilities for QR Scanning")]
    IScanner scanner;
    float restartTime;
    string qrStringRead;
    public AudioSource audioS;

    [Header("UI Elements")]
    public Text textScorePlayerOne;
    public Text textScorePlayerTwo;
    public Text textPrefab;

    public GameObject[] columnPlayerOne = new GameObject[3];
    public GameObject[] columnPlayerTwo = new GameObject[3];

    // Use this for initialization
    void Start () {
        scanner = new Scanner();
        scanner.Camera.Play();
        scanner.OnReady += (sender, arg) => {
            restartTime = Time.realtimeSinceStartup; 
        };
        textScorePlayerOne.text = "0";
        textScorePlayerTwo.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
        textScorePlayerOne.text = "P1 : " + scorePlayerOne.ToString();
        textScorePlayerTwo.text = "P2 : " + scorePlayerTwo.ToString();
        if (scanner != null)
        {
            scanner.Update();
        }

        if(restartTime != 0 && restartTime < Time.realtimeSinceStartup)
        {
            StartScanner();
            restartTime = 0;
        }

        if (tP == TurnPhase.Initialisation)
        {
            StartCoroutine("Reset");
        }

        if (tP == TurnPhase.Sorting)
        {
            StartCoroutine("SortingCards");
        }
        
    }

    void StartScanner()
    {
        scanner.Scan((barCodeType, barCodeValue) => {
            scanner.Stop();
            Debug.Log(barCodeValue);

            switch (tP)
            {
                case TurnPhase.Scanning:
                    if (pT == PlayerTurn.One)
                    {
                        if (barCodeValue.Contains("pierre")){
				            cTOne[cardPlayedTurnOne] = CardType.Rock;
			            }
			            if (barCodeValue.Contains("ciseau")){
				            cTOne[cardPlayedTurnOne] = CardType.Scissor;
			            }
			            if (barCodeValue.Contains("feuille")){
				cTOne[cardPlayedTurnOne] = CardType.Paper;
			}
                        if (barCodeValue.Contains("Joker")) {
                            cTOne[cardPlayedTurnOne] = CardType.Joker;
                        }
			            if (barCodeValue.Contains("rouge")){
				cCOne[cardPlayedTurnOne] = CardColor.Red;
			}else if (barCodeValue.Contains("bleu")){
				            cCOne[cardPlayedTurnOne] = CardColor.Blue;
			            }           
			            cardPlayedTurnOne += 1;
			            if (cardPlayedTurnOne >= 3){
				pT = PlayerTurn.Two;
			}
			            break;
                    }
                    if (pT == PlayerTurn.Two)
                    {
                        if (barCodeValue.Contains("pierre"))
                        {
                            cTTwo[cardPlayedTurnTwo] = CardType.Rock;
                        }
                        if (barCodeValue.Contains("ciseau"))
                        {
                            cTTwo[cardPlayedTurnTwo] = CardType.Scissor;
                        }
                        if (barCodeValue.Contains("feuille"))
                        {
                            cTTwo[cardPlayedTurnTwo] = CardType.Paper;
                        }
                        if (barCodeValue.Contains("Joker"))
                        {
                            cTTwo[cardPlayedTurnTwo] = CardType.Joker;
                        }
                        if (barCodeValue.Contains("rouge"))
                        {
                            cCTwo[cardPlayedTurnTwo] = CardColor.Red;
                        }
                        else if (barCodeValue.Contains("bleu"))
                        {
                            cCTwo[cardPlayedTurnTwo] = CardColor.Blue;
                        }
                        cardPlayedTurnTwo += 1;
                        if (cardPlayedTurnTwo >= 3)
                        {
                            pT = PlayerTurn.One;
                            tP = TurnPhase.Sorting;
                        }
                        break;
                    }
                    break;
                case TurnPhase.Sorting:
                    Debug.LogError("YEAH LET'S SORT DAT SHIET");
                    break;
            }
            restartTime += Time.realtimeSinceStartup + 1.5f;
            audioS.Play();
        });
    }
    
    //Comparing all the card 
    private IEnumerator SortingCards ()
    {
        tP = TurnPhase.WaitResult;
        yield return new WaitForSeconds(1f);
        for (int j = 0; j < 3; j++)
        {
            rowOne[turnNumber].cC[j] = cCOne[j];
            rowTwo[turnNumber].cC[j] = cCTwo[j];
        }
        for (int i = 0; i <= 2; i++)
        {
            if (cTOne[i] == CardType.Paper)
            {
                Text newTextOne = Instantiate(textPrefab, columnPlayerOne[i].transform); 
                if (cCOne[i] == CardColor.Blue)
                {
                    newTextOne.text = "P.B";
                }else if (cCOne[i] == CardColor.Red)
                {
                    newTextOne.text = "P.R";
                }

                if (cTTwo[i] == CardType.Paper)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "P.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "P.R";
                    }
                    Debug.LogWarning("Equal");
                }
                if (cTTwo[i] == CardType.Rock)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "R.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "R.R";
                    }                 
                    Debug.LogWarning("P1 Win");
                    PlayerOneScore(cCOne[i], i);
                }
                if (cTTwo[i] == CardType.Scissor)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "S.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "S.R";
                    }
                    PlayerTwoScore(cCTwo[i], i);
                    Debug.LogWarning("P2 Win");
                }
                if (cTTwo[i] == CardType.Joker)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    newTextTwo.text = "J";
                    scorePlayerTwo += 1;
                    Debug.Log("P2 win");
                }
            }
            if (cTOne[i] == CardType.Rock)
            {
                Text newTextOne = Instantiate(textPrefab, columnPlayerOne[i].transform);
                if (cCOne[i] == CardColor.Blue)
                {
                    newTextOne.text = "R.B";
                }
                else if (cCOne[i] == CardColor.Red)
                {
                    newTextOne.text = "R.R";
                }
                if (cTTwo[i] == CardType.Paper)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "P.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "P.R";
                    }
                    PlayerTwoScore(cCTwo[i], i);
                    Debug.LogWarning("P2 Win");
                }
                if (cTTwo[i] == CardType.Rock)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "R.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "R.R";
                    }
                    Debug.LogWarning("Equal");
                }
                if (cTTwo[i] == CardType.Scissor)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "S.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "S.R";
                    }
                    PlayerOneScore(cCOne[i], i);
                    Debug.LogWarning("P1 Win");
                }
                if (cTTwo[i] == CardType.Joker)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    newTextTwo.text = "J";
                    scorePlayerTwo += 1;
                    Debug.Log("P2 win");
                }
            }
            if (cTOne[i] == CardType.Scissor)
            {
                Text newTextOne = Instantiate(textPrefab, columnPlayerOne[i].transform);
                if (cCOne[i] == CardColor.Blue)
                {
                    newTextOne.text = "S.B";
                }
                else if (cCOne[i] == CardColor.Red)
                {
                    newTextOne.text = "S.R";
                }
                if (cTTwo[i] == CardType.Paper)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "P.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "P.R";
                    }
                    PlayerOneScore(cCOne[i], i);
                    Debug.LogWarning("P1 Win");
                }
                if (cTTwo[i] == CardType.Rock)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "R.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "R.R";
                    }
                    PlayerTwoScore(cCTwo[i], i);
                    Debug.LogWarning("P2 Win");
                }
                if (cTTwo[i] == CardType.Scissor)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "S.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "S.R";
                    }
                    Debug.LogWarning("Equal");
                }
                if (cTTwo[i] == CardType.Joker)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    newTextTwo.text = "J";
                    scorePlayerTwo += 1;
                    Debug.Log("P2 win");
                }
            }
            if (cTOne[i] == CardType.Joker)
            {
                Text newTextOne = Instantiate(textPrefab, columnPlayerOne[i].transform);
                newTextOne.text = "J";
                if (cTTwo[i] == CardType.Paper)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "P.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "P.R";
                    }
                    scorePlayerOne += 1;
                    Debug.LogWarning("P1 Win");
                }
                if (cTTwo[i] == CardType.Rock)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "R.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "R.R";
                    }
                    scorePlayerOne += 1;
                    Debug.LogWarning("P1 Win");
                }
                if (cTTwo[i] == CardType.Scissor)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    if (cCTwo[i] == CardColor.Blue)
                    {
                        newTextTwo.text = "S.B";
                    }
                    else if (cCTwo[i] == CardColor.Red)
                    {
                        newTextTwo.text = "S.R";
                    }
                    scorePlayerOne += 1;
                    Debug.LogWarning("P1 Win");
                }
                if (cTTwo[i] == CardType.Joker)
                {
                    Text newTextTwo = Instantiate(textPrefab, columnPlayerTwo[i].transform);
                    newTextTwo.text = "J";
                    Debug.Log("Equal");
                }
            }
            yield return new WaitForSeconds(1f);
        }
        tP = TurnPhase.Initialisation;
        yield return null;
    }

    private IEnumerator Reset()
    {
        turnNumber += 1;
        tP = TurnPhase.Scanning;
        cardPlayedTurnOne = 0;
        cardPlayedTurnTwo = 0;
        for (int i = 0; i <= 2; i++)
        {
            cTOne[i] = CardType.Null;
            cTTwo[i] = CardType.Null;
            cCOne[i] = CardColor.Joker;
            cCTwo[i] = CardColor.Joker;
            if (i == 3)
            {
                tP = TurnPhase.Scanning;
            }
        }
        yield return null;
    }


    void PlayerOneScore(CardColor color, int columnNumber)
    {
        for (int i = turnNumber; i >= 0; i--)
        {
            if (rowOne[i].cC[columnNumber] == color)
            {
                Debug.LogWarning(rowOne[turnNumber].cC[columnNumber]);
                Debug.LogWarning(color);
                scorePlayerOne += 1;
            }else
            {
                i = -1;
            }
        }
    }
    void PlayerTwoScore(CardColor color, int columnNumber)
    {
        for (int i = turnNumber; i >= 0; i--)
        {
            if (rowTwo[i].cC[columnNumber] == color)
            {
                scorePlayerTwo += 1;
            }
            else
            {
                i = -1;
            }
        }
    }
}
