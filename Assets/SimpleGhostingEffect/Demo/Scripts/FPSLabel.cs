using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FPSLabel : MonoBehaviour
{
    [SerializeField]
    private Text fpsText;
	// Use this for initialization
	void Start ()
	{
	    if (!fpsText) fpsText = GetComponent<Text>();
	    dTime = Time.realtimeSinceStartup;
	}

    private int numFrame;
    private float dTime;
	// Update is called once per frame
	void Update ()
	{	    
	    numFrame ++;
	    if (numFrame >= 20)
	    {
	        fpsText.text = string.Format("FPS:{0:0.0}", numFrame/(Time.realtimeSinceStartup - dTime));
	        dTime = Time.realtimeSinceStartup;
	        numFrame = 0;
	    }

        
	}
}
