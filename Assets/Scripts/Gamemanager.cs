using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField]
    private GameObject capsulePrefab;

    private Capsule currentCapsule;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnCapsule();
    }

    public void SpawnCapsule()
    {
        GameObject obj = Instantiate(
            capsulePrefab);

        currentCapsule = obj.GetComponent<Capsule>();
    }
}