using UnityEngine;
using System.Collections;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class EnemySight : MonoBehaviour
    {

        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        public enum State
        {
            PATROL,
            CHASE,
            INVESTIGATE
        }

        public State state;
        private bool alive;

        //Variables for patrolling
        public GameObject[] wayPoints;
        private int wayPointIndex;
        public float patrolSpeed = 0.5f;

        //Variables for chasing
        public float chaseSpeed = 1f;
        public GameObject target;

        //Variables for investigating
        private Vector3 investigateSpot;
        private float timer = 0;
        public float investigateWait = 10f;

        //Variables for sight
        public float heightMultiplier;
        public float sightDistance = 10;






        // Use this for initialization
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            //initialise waypoint
            wayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
            wayPointIndex = Random.Range(0, wayPoints.Length);

            //Set initial state
            state = EnemySight.State.PATROL;

            alive = true;

            //initialise heightmultiplier
            heightMultiplier = 1.36f;

            //Start Finite State Machine
            StartCoroutine("FiniteStateMachine");
        }

        IEnumerator FiniteStateMachine()
        {
            //run while alive
            while (alive)
            {
                switch (state)
                {
                    case State.PATROL:
                        Patrol();
                        break;

                    case State.CHASE:
                        Chase();
                        break;

                    case State.INVESTIGATE:
                        Investigate();
                        break;
                }
                yield return null;
            }
        }


        void Patrol()
        {
            agent.speed = patrolSpeed;

            //if too far move closer
            if (Vector3.Distance(this.transform.position, wayPoints[wayPointIndex].transform.position) >= 2)
            {
                agent.SetDestination(wayPoints[wayPointIndex].transform.position);
                character.Move(agent.desiredVelocity, false, false);

            }

            //if too close move to a different way point
            else if (Vector3.Distance(this.transform.position, wayPoints[wayPointIndex].transform.position) <= 2)
            {
                wayPointIndex = Random.Range(0, wayPoints.Length);

            }

            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        //Run after the target
        void Chase()
        {
            agent.speed = chaseSpeed;

            agent.SetDestination(target.transform.position);
            character.Move(agent.desiredVelocity, false, false);
        }

        void Investigate()
        {
            timer += Time.deltaTime;
            
            //stop moving the character immediately
            agent.SetDestination(this.transform.position);
            character.Move(Vector3.zero, false, false);

            //look at the player
            transform.LookAt(investigateSpot);

            //if player not seen, patrol!
            if(timer >= investigateWait)
            {
                state = EnemySight.State.PATROL;
                timer = 0;
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                state = EnemySight.State.INVESTIGATE;
                investigateSpot = collider.gameObject.transform.position;
            }
        }

        void FixedUpdate()
        {
            RaycastHit hit;

            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDistance, Color.green);
            //45degree angle to the right
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDistance, Color.green);
            //45 degree to the left
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDistance, Color.green);

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDistance))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = EnemySight.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDistance))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = EnemySight.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDistance))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = EnemySight.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }
        }

    }
}

