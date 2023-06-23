using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private static BuildAnimation _instance;

    public static BuildAnimation Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("BuildAnimation is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    public void StartAnimation(Vector2Int vector2Int)
    {
        _animator.transform.position = new Vector3(vector2Int.x, -vector2Int.y, -1);
        _animator.Play("Building", 0, 0f);
    }
}


