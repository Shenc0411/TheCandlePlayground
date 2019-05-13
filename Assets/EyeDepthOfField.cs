using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.Rendering.PostProcessing;

public class EyeDepthOfField : MonoBehaviour
{

    private PostProcessVolume _postProcessVolume;
    private DepthOfField DOF;

    // Start is called before the first frame update
    void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out DOF);
    }

    // Update is called once per frame
    void Update()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        Ray ray = Camera.main.ScreenPointToRay(gazePoint.Screen);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            DOF.focusDistance.value = Mathf.Lerp(hit.distance, DOF.focusDistance.value, Time.deltaTime);
            Debug.Log(DOF.focusDistance.value);
        }
    }
}
