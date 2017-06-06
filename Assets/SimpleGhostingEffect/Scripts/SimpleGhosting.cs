using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Simply add this script component to an animated character,then call BeginSnapping().
/// </summary>
public class SimpleGhosting : MonoBehaviour
{
    private bool _snapping;
    
    public bool snapping
    {
        get { return _snapping; }
        set
        {
            if (_snapping != value)
            {
                if (value)
                {
                    BeginSnapping();
                }
                else
                {
                    EndSnapping();
                }
                _snapping = value;
            }
        }
    }

    public Gradient colorOverTimeline= new Gradient()
    {
        alphaKeys = new [] { new GradientAlphaKey(1f,0f),new GradientAlphaKey(0f,1f)},
        colorKeys = new []{new GradientColorKey(Color.white,0f),new GradientColorKey(Color.white,1f)}
    };
    public float lifeTime = 1f;
    public int maxGhostsCount = 20;
    public float snapDistance = 0.1f;
    public float snapInterval = 0.02f;
    public string shaderName = "Unlit/Ghost";
    private Shader usedShader;
    private List<SkinnedMeshRenderer> smrs = new List<SkinnedMeshRenderer>();
    private GameObject LGObject;
    private List<GhostInfo> ghosts = new List<GhostInfo>();
    Queue<GhostInfo> ghostPool =new Queue<GhostInfo>();
	// Use this for initialization
	void Start ()
	{
        if (!animator)
            animator = GetComponent<Animator>();

        Initial();
	    


	}


    private Animator animator;

    void OnEnable()
    {
        

    }

    void OnDisable()
    {
    }

    void OnDestroy()
    {
        snapping = false;
        if (LGObject)
        {
            Destroy(LGObject);
        }
    }

    private Transform waist, leftHand, rightHand, leftFoot, rightFoot;
    private Vector3 waistPos, leftHandPos, rightHandPos, leftFootPos, rightFootPos;
    private void Initial()
    {
        usedShader = Shader.Find(shaderName);
        smrs = new List<SkinnedMeshRenderer>(GetComponentsInChildren<SkinnedMeshRenderer>());
        List<Material> matList = new List<Material>();
        Dictionary<Material,Material> origToInstMatDict = new Dictionary<Material, Material>();

        if (LGObject)
        {
            Destroy(LGObject, lifeTime);
        }
        LGObject = new GameObject("_LegacyGhosting:" + gameObject.name);
        LGObject.layer = gameObject.layer;


        for (int i = 0; i < maxGhostsCount; i++)
        {
            var gi = new GhostInfo();
            var go = new GameObject("_SnappedGhost");
            go.transform.parent = LGObject.transform;
            go.layer = LGObject.layer;
            var giMeshList = new List<Mesh>();
            foreach (var smr in smrs)
            {
                var m = smr.sharedMesh;
                var mats = new List<Material>();
                for (int j = 0; j < m.subMeshCount; j++)
                {
                    var oriMat = smr.sharedMaterials[j];
                    if (!origToInstMatDict.ContainsKey(oriMat))
                    {
                        var instMat = new Material(oriMat);
                        if (usedShader) instMat.shader = usedShader;
                        origToInstMatDict.Add(oriMat, instMat);
                    }
                    mats.Add(origToInstMatDict[oriMat]);
                }
                var child = new GameObject("_mesh:" + smr.name);
                child.transform.parent = go.transform;
                child.layer = go.layer;
                var nmesh = new Mesh();
                nmesh.MarkDynamic();
                
                giMeshList.Add(child.AddComponent<MeshFilter>().sharedMesh = nmesh);
                child.AddComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
            }            
            gi.snappedObject = go;
            gi.mesh = giMeshList.ToArray();
            go.SetActive(false);
            ghostPool.Enqueue(gi);
        }




        waist = animator.GetBoneTransform(HumanBodyBones.Hips);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

    }
    
    private AnimatorStepSampler animSamp;

    [ContextMenu("BeginSnap")]
    public void BeginSnapping()
    {
        _snapping = true;
        if (!animator) animator = GetComponent<Animator>();
        if (!animSamp) animSamp = AnimatorStepSampler.Get(animator.gameObject, snapInterval, SnapLegacyGhost);
    }

    [ContextMenu("EndSnap")]
    public void EndSnapping()
    {
        _snapping = false;
        if (animSamp)
        {
            animSamp.OnStepUpdate -= SnapLegacyGhost;
        }
    }

    // Update is called once per frame
	void Update ()
	{
        
	}

    

    void LateUpdate()
    {

        FadingGhosts();
    }

    
    public void FadingGhosts()
    {
        if (ghosts.Count <= 0) return;
        float time = Time.time;
        while (true)
        {
            if (ghosts.Count <= 0) break;
            var gi = ghosts[0];
            if (time - gi.snapTime > lifeTime)
            {                
                gi.snappedObject.SetActive(false);
                ghostPool.Enqueue(gi);
                ghosts.RemoveAt(0);
                if (lastGhost == gi) lastGhost = null;
            }
            else
                break;
        }
        
        foreach (var gi in ghosts)
        {
            float timeLived = time - gi.snapTime;
            float normalizedTime = Mathf.InverseLerp(0, lifeTime, timeLived);
            Color32 c = colorOverTimeline.Evaluate(normalizedTime);
            //Debug.Log("color:" + c + " nt:" + normalizedTime + " t:" + timeLived);
            foreach (var m in gi.mesh)
            {
                var clrs = m.colors32;
                if (clrs == null||clrs.Length!=m.vertexCount)
                    clrs = new Color32[m.vertexCount];
                for (var i = 0; i < clrs.Length; i++) clrs[i] = c;
                
                m.colors32 = clrs;
            }

            
            
            
        }
        

    }

    

    public class GhostInfo
    {
        public GameObject snappedObject;
        public Mesh[] mesh;
        public float snapTime;       
    }

    private GhostInfo lastGhost;
    public void SnapLegacyGhost(float currentTime)
    {
        float sqrDist = snapDistance*snapDistance;
        Vector3 rhPos = rightHand.position,lhPos = leftHand.position,
            rfPos = rightFoot.position,lfPos = leftFoot.position,wsPos = waist.position;
        if ((rhPos - rightHandPos).sqrMagnitude < sqrDist && (lhPos - leftHandPos).sqrMagnitude < sqrDist
            && (rfPos - rightFootPos).sqrMagnitude < sqrDist && (lfPos - leftFootPos).sqrMagnitude < sqrDist
            && (wsPos - waistPos).sqrMagnitude < sqrDist)
            return;

        rightHandPos = rhPos;
        leftHandPos = lhPos;
        rightFootPos = rfPos;
        leftFootPos = lfPos;
        waistPos = wsPos;

        bool overFlowed = false;
        GhostInfo gi = null;
        
       
        //var mesh = Snap();
        

        if (ghosts.Count >= maxGhostsCount)
        {
            overFlowed = true;
            gi = ghosts[0];
            ghosts.RemoveAt(0);
            ghosts.Add(gi);
        }
        else
        {
            gi = ghostPool.Dequeue();
            ghosts.Add(gi);
            gi.snappedObject.SetActive(true);
        }
/*
        GameObject go = gi.snappedObject;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        
        mf.sharedMesh = mesh;
        mr.sharedMaterials = materialList;    
        gi.mesh = mesh;*/
        Snap(gi.mesh);
        var sot = gi.snappedObject.transform;
        sot.position = transform.position;
        sot.rotation = transform.rotation;
        sot.localScale = transform.localScale;
        gi.snapTime = currentTime;

        lastGhost = gi;

        

        if (overFlowed)
        {
            float earliestTime = currentTime - lifeTime;
            float timeStep = (float)lifeTime/(float)(ghosts.Count);
            for (int i = 0; i < ghosts.Count; i++)
            {
                ghosts[i].snapTime = earliestTime + ((i + 1)*timeStep);
            }

        }
        
    }


    Mesh Snap()
    {
        Mesh mesh = new Mesh();        
        List<CombineInstance> combineList= new List<CombineInstance>();
        foreach (var smr in smrs)
        {
            
            Mesh m = new Mesh();
            smr.BakeMesh(m);            

            for (int i = 0; i < m.subMeshCount; i++)
            {
                CombineInstance ci = new CombineInstance
                {
                    mesh = m, 
                    subMeshIndex = i,
                    transform = smr.localToWorldMatrix
                };
                combineList.Add(ci);               
            }
        }


        mesh.CombineMeshes(combineList.ToArray(),false,true);


        return mesh;
    }

    Mesh[] Snap(Mesh[] mesh)
    {
        for (int i = 0; i < smrs.Count; i++)
        {
            var smr = smrs[i];
            smr.BakeMesh(mesh[i]);
        }

        return mesh;
    }
}
