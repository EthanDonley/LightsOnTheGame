using UnityEngine;

public class RetainInfo : MonoBehaviour
{
    void Awake()
    {
        // Ensure that this GameObject persists between scene changes
        DontDestroyOnLoad(gameObject);
    }
}
