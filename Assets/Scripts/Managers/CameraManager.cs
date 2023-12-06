using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CinemachineVirtualCamera bearCam, buildingCam;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        else
            instance = this;
    }

    public void SetBuildMode(bool state)
    {
        if (state)
            buildingCam.transform.position = bearCam.transform.position;
        animator.SetBool("BuildMode", state);
    }

}
