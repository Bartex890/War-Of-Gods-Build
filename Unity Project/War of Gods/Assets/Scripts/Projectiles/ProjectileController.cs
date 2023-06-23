using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Vector3 _from;
    private Vector3 _to;
    private Projectile _projectile;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUpProjectile(Projectile projectile, Vector3 from, Vector3 to)
    {
        _spriteRenderer.sprite = projectile.sprite;

        _from = from;
        _to = to;
        _projectile = projectile;

        LeanTween.value(gameObject, 0, 1, Vector3.Distance(from, to) / projectile.speed).setOnUpdate((float value) => _SetPosition(value)).setOnComplete(() => Destroy(gameObject) );
    }

    private void _SetPosition(float value)
    {
        Vector3 newPos = Vector3.Lerp(_from, _to, value)
            + new Vector3(0, (-4 * Mathf.Pow(value - 0.5f, 2) + 1) * 0.5f, 0);

        if (_projectile.rotateInFlightDirection)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(newPos.y - transform.position.y, newPos.x - transform.position.x) * (180 / Mathf.PI) + _projectile.rotationOffset);

        if (_projectile.spin)
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + _projectile.spinSpeed * 360 * Time.deltaTime);

        transform.position = newPos;
    }
}
