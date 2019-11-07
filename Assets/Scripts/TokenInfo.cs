using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenInfo : MonoBehaviour {
    Renderer rend;
    public Color tokenColor;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (rend.material.color != tokenColor)
        {
            rend.material.color = tokenColor;
        }
	}



    /*if (barCodeValue.Contains("rouge") && redSpawned == false)
                    {
                        GameObject tokenRed = (GameObject)Instantiate(token, transform.position, transform.rotation);
                        tokenRed.transform.position = new Vector3(1.5f, 0, 0);
                        TokenInfo tIR = tokenRed.GetComponent<TokenInfo>();
                        tIR.tokenColor = Color.red;
                        redSpawned = true;
                    }
                    if (barCodeValue.Contains("bleu") && blueSpawned == false)
                    {
                        GameObject tokenBlue = (GameObject)Instantiate(token, transform.position, transform.rotation);
                        tokenBlue.transform.position = new Vector3(-1.5f, 0, 0);
                        TokenInfo tIR = tokenBlue.GetComponent<TokenInfo>();
                        tIR.tokenColor = Color.blue;
                        blueSpawned = true;
                    }*/
}
