using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public Vector3 targetPos;
    public bool startMoving;
    public float speed;


    GridPlayerAnimation gridAnim;
    [SerializeField]
    GameObject gridGenerator;
    public GameObject clashParticle;


/*    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //ray = new Ray(transform.position + new Vector3(0f, 0.5f, 0.3f), transform.forward);
        if (!startMoving) {
            return;
        }
        
        Vector3 directionToTarget = targetPos - transform.position;
        // Calculate the desired rotation based on the direction to the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        targetRotation.x = transform.rotation.x;
        targetRotation.z = transform.rotation.z;
        transform.rotation = targetRotation;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

    }
    private void OnCollisionEnter(Collision collision)
    {
        return;
        if (collision.collider.CompareTag("Player"))
        {
            startMoving = false;
            // Instantiate the particle system prefab at a specific position and rotation
            GameObject instantiatedParticle = Instantiate(clashParticle, transform.position, Quaternion.identity);

            // Optional: You can also access the ParticleSystem component to control it further
            ParticleSystem particleSystemClash = instantiatedParticle.GetComponent<ParticleSystem>();

            particleSystemClash.Play();

            Vector2 lastGrid = GridSystem.instance.WorldToGrid(gameObject.GetComponent<TemporaryStats>().lastPosition.transform.position);
            transform.position = GridSystem.instance._gridArray[(int)lastGrid.x, (int)lastGrid.y].transform.position;
            GetComponent<TemporaryStats>().currentPlayerGridPosition = transform.position;

            if (TryGetComponent(out gridAnim))
            {
                gridAnim.SetMoveAnimation(0, 2);
            }

            RemoveCue.instance.RemoveAllCues();// have to check if game object name is the same

        }
    }*/



}
