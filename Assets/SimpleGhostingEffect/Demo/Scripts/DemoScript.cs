using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour
{

    public SimpleGhosting characterGhosting;

    public Toggle togGhosting,togAnimation;
	// Use this for initialization
	void Start ()
	{
	    togGhosting.isOn = true;
	    togAnimation.isOn = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void OnToggleGhostingEffect()
    {
        if(togGhosting.isOn) characterGhosting.BeginSnapping();
        else characterGhosting.EndSnapping();
    }

    public void OnToggleAnimation()
    {
        var animator = characterGhosting.GetComponent<Animator>();
        animator.speed = togAnimation.isOn ? 1f : 0f;
    }

    public void OnChangeTimeScale(float ts)
    {
        Time.timeScale = ts;
    }
}
