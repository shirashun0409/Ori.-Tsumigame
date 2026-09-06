using UnityEngine;


public class LogoRotate : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);
    }
}
