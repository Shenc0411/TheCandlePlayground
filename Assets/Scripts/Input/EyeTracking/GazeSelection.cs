using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class GazeSelection : MonoBehaviour
{

    public Material matNotSelected;
    public Material matSelected;

    private GazeAware _gazeAware;
    private MeshRenderer _meshRenderer;
    private bool isBeingDragged = false;
    private float distance = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _gazeAware = GetComponent<GazeAware>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_gazeAware.HasGazeFocus)
        {
            _meshRenderer.material = matSelected;
            if (Input.GetKey(KeyCode.Space))
            {
                isBeingDragged = true;
                distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            }
            else
            {
                isBeingDragged = false;
            }

            if (isBeingDragged)
            {
                GazePoint gazePoint = TobiiAPI.GetGazePoint();
                Ray ray = Camera.main.ScreenPointToRay(gazePoint.Screen);
                transform.position = Camera.main.transform.position +  distance * ray.direction.normalized;
                transform.LookAt(Camera.main.transform.position);
            }

        }
        else
        {
            _meshRenderer.material = matNotSelected;
        }
    }
}
