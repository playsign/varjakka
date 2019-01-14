using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

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

        // Hide the "3D" button when going into AR mode
        GameObject.Find("Background").GetComponent<Image>().enabled = !tofree;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

