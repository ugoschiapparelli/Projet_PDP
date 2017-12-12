using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera myCamera;

	public Camera GetCamera(){
		return myCamera;
	}

    // Use this for initialization
	public void SetupCamera()
    {
        GameObject toInstantiate = new GameObject("MainCamera");
		BoardManager bm = GetComponent<BoardManager> ();
		toInstantiate.transform.position = new Vector3 (bm.GetColumns()/2, bm.GetRows()/2, -5);
        myCamera = toInstantiate.AddComponent<Camera>();
        myCamera.orthographic = true;
		myCamera.aspect = (float)Screen.width / Screen.height;
		myCamera.orthographicSize = (float)bm.GetRows()/2;
        toInstantiate.transform.SetParent(toInstantiate.transform);

		myCamera.projectionMatrix = Matrix4x4.Ortho(-myCamera.orthographicSize * myCamera.aspect,
														myCamera.orthographicSize * myCamera.aspect,
														-myCamera.orthographicSize,
														myCamera.orthographicSize,
														myCamera.nearClipPlane,
														myCamera.farClipPlane);
    }

	public void End(){
		Destroy (myCamera);
	}


}
