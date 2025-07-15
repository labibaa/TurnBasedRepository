using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class ObjectToBePushed : MonoBehaviour
{


    Rigidbody body;
    [SerializeField]
    Vector3 target;
    [SerializeField]
    AnimationCurve speedCurve;
   // private float forceMultiplier =6f;
    private float speed= 5f;
    bool isShoot;
    public Queue<Vector2> positionTraversed = new Queue<Vector2>();
    public bool IsShoot { get => isShoot; set => isShoot = value; }
    public Vector3 Target { get => target; set => target = value; }

    public bool OwnShoot = false;
    public ObjectToBePushed PlayerPushScript;

    private void Awake()
    {
       
    }
    private void Start()
    {
        positionTraversed.Clear();
       // positionTraversed.Enqueue(transform.position);
    }
    private void Update()
    {



        PositionTracker();
        if (isShoot && PlayerPushScript.OwnShoot)
        {
           
            Shoot(Target - transform.position);
        }
       
    }

    void Shoot(Vector3 force)
    {
        // Calculate the direction towards the target
        Vector3 moveDirection = target - transform.position;

        // Evaluate the animation curve to determine the speed based on the distance
        float distanceToTarget = moveDirection.magnitude;
        float normalizedDistance = Mathf.Clamp01(distanceToTarget / speed); // Normalize distance
        float speedMultiplier = speedCurve.Evaluate(normalizedDistance); // Evaluate the animation curve
        float currentSpeed = speed * speedMultiplier;
        transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
        this.GetComponent<TemporaryStats>().currentPlayerGridPosition= transform.position;
        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            Invoke("IsShootFalseFunc",1);
           

        }

    }

    void IsShootFalseFunc()
    {
        isShoot = false;
        PlayerPushScript.OwnShoot = false;
        
    }
    void IsShootTrueFunc()
    {
       OwnShoot = true;

    }

    public async UniTask WaituntillPushFinished()
    {
        while (isShoot)
        {
            await UniTask.Yield(); // Use UniTask.Yield to yield control to the main thread
        }
    }

    public void PositionTracker()
    {
        if (isShoot)
        {
            Vector2 lastPosition = GridSystem.instance.WorldToGrid(transform.position);
            if (positionTraversed.Count>1 )
            {
                if (positionTraversed.Peek()!=lastPosition)
                {
                    positionTraversed.Dequeue();
                    positionTraversed.Enqueue(lastPosition);   
                }
            }
            else
            {
                positionTraversed.Enqueue(lastPosition);
            }
        }
    }
}
 