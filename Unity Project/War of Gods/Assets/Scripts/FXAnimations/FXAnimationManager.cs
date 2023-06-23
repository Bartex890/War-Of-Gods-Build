using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXAnimationManager : MonoBehaviour
{
    private Animator _exampleAnimator;

    private static FXAnimationManager _instance;

    public static FXAnimationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("FXAnimationManager is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;

        _exampleAnimator = transform.Find("ExampleAnimation").GetComponent<Animator>();
        _exampleAnimator.gameObject.SetActive(false);
    }

    public static void PlayAnimationAtPosition(string name, Vector2Int pos)
    {
        Animator animator =  Instantiate(Instance._exampleAnimator);

        animator.transform.position = new Vector3(pos.x, -pos.y, -3);
        animator.gameObject.SetActive(true);

        animator.SetTrigger(name);
    }
}
