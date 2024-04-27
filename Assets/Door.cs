using DistilledGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    private int scene;

    public Transform bearPos;

	private void OnTriggerEnter2D(Collider2D collision)
	{
        SceneController.instance.SwitchScene(scene);
	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
