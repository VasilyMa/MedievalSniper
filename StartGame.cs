using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client 
{
    public class StartGame : MonoBehaviour
    {

        void Start()
        {
            LoadGame();
        }
        void LoadGame()
        {
            if (File.Exists(SaveManager.savePath))
            {
                SaveManager.LoadOnStartGame();
                SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}

