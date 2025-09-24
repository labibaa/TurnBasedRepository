using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class doorOpeningAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play(animationName);

        }

    }
}
