using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDesth : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private static AnimationDesth _instance;

    public static AnimationDesth Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AnimationDesth is null");
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
        _animator.transform.position =new Vector3(vector2Int.x, -vector2Int.y+0.5f,-1);
        _animator.Play("Death",0,0f);
    }
}
