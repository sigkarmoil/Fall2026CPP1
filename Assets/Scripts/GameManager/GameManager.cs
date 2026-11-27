using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class EscapeRule
{
    public string whenInScene;
    public string loadScene;
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("Scene names (must match Build Settings)")]
    [SerializeField] private string titleScene = "Title";
    [SerializeField] private string gameScene = "GameScene";
    [SerializeField] private string gameOverScene = "GameOver";

    [Header("ESC: when in this scene, load that scene")]
    [SerializeField] private List<EscapeRule> escapeRules = new List<EscapeRule>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        string current = SceneManager.GetActiveScene().name;
        foreach (EscapeRule rule in escapeRules)
        {
            if (rule.whenInScene == current)
            {
                SceneManager.LoadScene(rule.loadScene);
                return;
            }
        }
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene(titleScene);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene(gameOverScene);
    }
}