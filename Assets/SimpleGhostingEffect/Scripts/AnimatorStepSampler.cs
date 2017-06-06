using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;
using UnityEngineInternal;


public class AnimatorStepSampler : MonoBehaviour
{

    public float timeStep = 0.02f;

    private Action<float> _onStepUpdate = null;
    public Action<float> OnStepUpdate
    {
        get { return _onStepUpdate; }
        set
        {
            _onStepUpdate = value;
            if (_onStepUpdate == null) Destroy(this);
        }
    }

    private Animator animator;

    // Use this for initialization
    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }
    void OnEnable()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        if (animator)
        {
            animator.enabled = false;
        }
    }

    void OnDisable()
    {
        if (animator)
            animator.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator && animator.enabled) animator.enabled = false;
        float dt = Time.deltaTime;
        float beginTime = Time.time - dt;

        while (true)
        {
            if (dt > timeStep)
            {
                dt -= timeStep;         
                animator.Update(timeStep);
                if (_onStepUpdate != null)
                    _onStepUpdate(beginTime + timeStep);

            }
            else
            {
                animator.Update(dt);
                if (_onStepUpdate != null)
                    _onStepUpdate(beginTime + dt);
                break;
            }
        }

    }
    

    
    static public AnimatorStepSampler Get(GameObject animatorObject)
    {
        if (!animatorObject) return null;
        var ss = animatorObject.GetComponent<AnimatorStepSampler>();
        if (ss) return ss;
        return animatorObject.AddComponent<AnimatorStepSampler>();
    }

    static public AnimatorStepSampler Get(GameObject animatorObject,float timeStep, Action<float> onStepUpdate)
    {
        var ass = Get(animatorObject);
        ass.OnStepUpdate += onStepUpdate;
        ass.timeStep = timeStep;
        return ass;
    }
    
}

