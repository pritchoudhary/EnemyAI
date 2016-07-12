using UnityEngine;
using System.Collections;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class EnemyCamera : MonoBehaviour
    {

        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        public enum State
        {
            PATROL,
            CHASE
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

        //Variables fot camera sight
        public GameObject player;
        public Collider playerCol;
        public Camera myCamera;
        private Plane[] planes;

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

            playerCol = player.GetComponent<Collider>();

            //Set initial state
            state = EnemyCamera.State.PATROL;

            alive = true;

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
  
        void Update()
        {
            //calculate frustum planes through camera evry single frame
            planes = GeometryUtility.CalculateFrustumPlanes(myCamera);

            if(GeometryUtility.TestPlanesAABB(planes, playerCol.bounds))
            {
                Debug.Log("Player Sighted");
                CheckForPlayer();
            }
            else
            {

            }
        }
        
        void CheckForPlayer()
        {
            RaycastHit hit;
            Debug.DrawRay(myCamera.transform.position, transform.forward * 10, Color.green);

            if (Physics.Raycast(myCamera.transform.position, transform.forward, out hit, 10))
            {
                state = EnemyCamera.State.CHASE;
                target = hit.collider.gameObject;
            }
        }
    }
}

