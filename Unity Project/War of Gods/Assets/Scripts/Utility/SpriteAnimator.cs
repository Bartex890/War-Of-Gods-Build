using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public List<Sprite> frames = new List<Sprite>();
    public float miliseconds;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = frames[0];

        StartCoroutine(animationLoop());
    }

    private void OnEnable()
    {
        StartCoroutine(animationLoop());
    }

    private IEnumerator animationLoop()
    {
        int currentIndex = Random.Range(0, frames.Count);

        yield return new WaitForSeconds((miliseconds / 1000)*Random.value);

        while (true)
        {
            currentIndex++;
            currentIndex %= frames.Count;

            _spriteRenderer.sprite = frames[currentIndex]; 

            yield return new WaitForSeconds(miliseconds/1000);
        }
    }
}
