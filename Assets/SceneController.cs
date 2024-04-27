using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DistilledGames
{
	public class SceneController : MonoBehaviour
	{
		public bool hasGamePlayed = false;
		public double cash;

		private GameManager currentGameManager;
		
		public static SceneController instance;


		private void Start()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			currentGameManager = FindObjectOfType<GameManager>();
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			currentGameManager = FindObjectOfType<GameManager>();
		}


		private void Awake()
		{
			if (instance == null)
				instance = this;
			else
				Destroy(gameObject);

			DontDestroyOnLoad(gameObject);
		}

		public void SwitchScene(int scene)
		{
		
			if (!hasGamePlayed)
				hasGamePlayed = true;
			cash = currentGameManager.Cash;
			SceneManager.LoadScene(scene);
		}

	}

}
