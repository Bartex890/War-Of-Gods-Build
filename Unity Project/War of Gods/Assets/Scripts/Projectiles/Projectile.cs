using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "newProjectile", menuName = "Projectile")]
public class Projectile : ScriptableObject
{   
    public Sprite sprite;
    public float speed;
    public bool rotateInFlightDirection = false;
    public float rotationOffset = 0;
    public bool spin = false;
    public float spinSpeed = 0;
}