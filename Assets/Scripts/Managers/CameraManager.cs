using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance;
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private CinemachineVirtualCamera bearCam, buildingCam;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void SetBuildMode(bool state)
	{
		if (state)
		{
			buildingCam.transform.position = bearCam.transform.position;
		}

		animator.SetBool("BuildMode", state);
	}

}
