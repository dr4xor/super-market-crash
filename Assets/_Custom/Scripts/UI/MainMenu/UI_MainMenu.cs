using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private Button buttonStartGame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonStartGame.onClick.AddListener(OnStartGameClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnStartGameClicked()
    {
        GameManager.Instance.StartGame();
    }
}
