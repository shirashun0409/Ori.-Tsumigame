using UnityEngine;

public class Capsule : MonoBehaviour
{
    [SerializeField]
    private GameObject capsulePartPrefab;

    private void Start()
    {
        CreateCapsule();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right;
        }
    }

    private void CreateCapsule()
    {
        CreatePart(Vector3.zero);
        CreatePart(Vector3.right);
    }

    private void CreatePart(Vector3 offset)
    {
        GameObject obj = Instantiate(
            capsulePartPrefab,
            transform.position + offset,
            Quaternion.identity,
            transform);

        CapsulePart part = obj.GetComponent<CapsulePart>();

        CapsuleColor randomColor = (CapsuleColor)Random.Range(0, 3);

        part.SetColor(randomColor);
    }
}


