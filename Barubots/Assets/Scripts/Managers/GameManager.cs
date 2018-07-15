using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Singleton GameManagers contains all guards and all players in a scene. Scene is also managed in this scene.
 * This manager can be called from any other script with GameManager.Instance.
 */
public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    private readonly float maxSlowMotionDuration = 0.3f;

    public float MaxSlowMotionDuration { get { return maxSlowMotionDuration; } }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }

            return _instance;
        }
    }


    void Awake()
    {
        _instance = this;
    }
}
