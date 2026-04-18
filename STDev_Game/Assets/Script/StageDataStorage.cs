using UnityEngine;

public class StageDataStorage : MonoBehaviour
{
    public static StageDataStorage Instance;

    public Vector2 savedPosition;
    public bool isPositionSaved = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}