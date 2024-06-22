using UnityEngine;

// поворот нынешнего объекта в target (в основном юзаю для взгляда в камеру)

public class LookAtCam : MonoBehaviour
{
    public Transform target;

    public bool atCamera;

    private void Awake()
    {
        if (atCamera)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        Vector3 dir = target.position- transform.position;
        transform.LookAt(dir);
    }
}
