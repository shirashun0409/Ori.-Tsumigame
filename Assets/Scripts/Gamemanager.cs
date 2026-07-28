using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject capsulePrefab;

    private Capsule currentCapsule;

    private void Start()
    {
        SpawnCapsule();
    }

    public void SpawnCapsule()
    {
        GameObject obj = Instantiate(
            capsulePrefab,
            Vector3.zero,
            Quaternion.identity);

        currentCapsule = obj.GetComponent<Capsule>();
    }
}