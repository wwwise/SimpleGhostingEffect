using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{

    public Transform target;


    public float yaw, pitch ,distance=3f;

    private Transform cameraPivot;

    void Awake()
    {
        cameraPivot = transform.Find("CameraPivot");
        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
	    if (Input.GetMouseButton(0)&& EventSystem.current.currentSelectedGameObject ==null)
	    {
	        var mx = Input.GetAxis("Mouse X");
	        var my = Input.GetAxis("Mouse Y");
	        pitch = Mathf.Clamp(pitch - my*4, -45f, 45f);
	        yaw += mx*10f;
	        cameraPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
	    }

	    var scroll = Input.GetAxis("Mouse ScrollWheel");
	    if (scroll != 0f) distance = Mathf.Clamp(distance*(1f + scroll*0.5f), 1f, 10f);
	    cameraPivot.localScale = new Vector3(distance, distance, distance);

	    if (target)
	    {
	        transform.position = target.position;
	    }
	}
}
