using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
	private VideoPlayer player;
	private void Awake()
	{
		player = GetComponent<VideoPlayer>();
		player.url = System.IO.Path.Combine(Application.streamingAssetsPath, "BGVid.mp4");
	}

	// Start is called before the first frame update
	private void Start()
	{
		player.Play();
	}

}
