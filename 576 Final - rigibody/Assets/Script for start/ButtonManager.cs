using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject instruction;
    public GameObject others;

    public void BackMenu() {
        instruction.SetActive(false);
        others.SetActive(true);
    }

    public void OpenInstruction() {
        instruction.SetActive(true);
        others.SetActive(false);
    }

    public void BeginTutorial() {
        SceneManager.LoadScene(1);
    }

    public void BeginLevelOne() {
        SceneManager.LoadScene(3);
    }
}
