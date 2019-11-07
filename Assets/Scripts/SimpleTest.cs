using BarcodeScanner;
using BarcodeScanner.Scanner;
using UnityEngine;

public class SimpleTest : MonoBehaviour {
    IScanner scanner;
	// Use this for initialization
	void Start () {
        scanner = new Scanner();
        scanner.Camera.Play();
        scanner.OnReady += OnScannerReady;
	}
	
    void OnScannerReady (object sender, System.EventArgs e)
    {
        scanner.Scan(OnScannerScanned);
    }
    void OnScannerScanned (string barCodeType, string barCodeValue)
    {
        scanner.Stop();
        Debug.LogFormat("Found {0}/{1}", barCodeType, barCodeValue);


    }

	// Update is called once per frame
	void Update () {
        scanner.Update();
	}

    void OnGUI()
    {
        /*if (scanner != null)
        {
            if (scanner.Status == ScannerStatus.Running)
            {
                Rect r = new Rect(0, 0, scanner.Camera.Texture.width, scanner.Camera.Texture.height);
                GUI.DrawTexture(r, scanner.Camera.Texture);
            }
        }*/
    }


}
