using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamChanger : MonoBehaviour
{
    public GameObject bluePlatform;
    public GameObject redPlatform;
    public Team currentTeam = Team.Blue;

    private void Start()
    {
        LoadTeam();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == bluePlatform)
        {
            ChangeTeam(Team.Blue);
        }
        else if (other.gameObject == redPlatform)
        {
            ChangeTeam(Team.Red);
        }
    }

    private void ChangeTeam(Team newTeam)
    {
        if (currentTeam == newTeam)
            return;

        currentTeam = newTeam;
        
        SaveTeam();

        switch (currentTeam)
        {
            case Team.Blue:
                Debug.Log("Игрок стал играть за синих");
                break;
            case Team.Red:
                Debug.Log("Игрок стал играть за красных");
                break;
        }
    }

    private void SaveTeam()
    {
        PlayerPrefs.SetInt("CurrentTeam", (int)currentTeam);
    }

    private void LoadTeam()
    {
        currentTeam = (Team)PlayerPrefs.GetInt("CurrentTeam", (int)Team.Blue);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadTeam();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

public enum Team
{
    Blue,
    Red
}