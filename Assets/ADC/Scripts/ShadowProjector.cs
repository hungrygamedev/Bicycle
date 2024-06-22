using UnityEngine;

public class ShadowProjector : MonoBehaviour
{
    [SerializeField] private Transform _shadowTarget;
    [Space] [SerializeField] private SpriteRenderer _spriteRenderer;
    [Space] [SerializeField] private float _maxDistance = 20;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private LayerMask _layerMask;
    private readonly RaycastHit[] result = new RaycastHit[1];

    private void Update()
    {
        var ray = new Ray(_shadowTarget.position, Vector3.down);

        if (Physics.RaycastNonAlloc(ray, result, _maxDistance, _layerMask) <= 0)
        {
            return;
        }

        var raycastHit = result[0];
        transform.position = raycastHit.point + _offset;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);

        var color = _spriteRenderer.color;
        color.a = 1 - raycastHit.distance / _maxDistance;
        _spriteRenderer.color = color;
    }
}