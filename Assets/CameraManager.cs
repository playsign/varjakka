using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public GameObject ARCam;
    //public GameObject FreeCam;
    public GameObject EnvironmentView;
    /*public GameObject HotspotCanvas;
    public GameObject PlayButton;*/

    public void ToggleFree(bool tofree)
    {
        ARCam.SetActive(!tofree);
        //HotspotCanvas.SetActive(!tofree);

        /*if (tofree)
        {
        }*/

        EnvironmentView.SetActive(tofree);
        //cam is in there here
        //FreeCam.SetActive(tofree);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

