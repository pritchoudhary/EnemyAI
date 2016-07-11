using UnityEngine;
using System.Collections;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class basicAI : MonoBehaviour
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
            state = basicAI.State.PATROL;

            alive = true;

            //Start Finite State Machine
            StartCoroutine("FiniteStateMachine");
        }

        IEnumerator FiniteStateMachine()
        {
            //run while alive
            while(alive)
            {
                switch(state)
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
            if(Vector3.Distance(this.transform.position, wayPoints[wayPointIndex].transform.position) >= 2)
            {
                agent.SetDestination(wayPoints[wayPointIndex].transform.position);
                character.Move(agent.desiredVelocity, false, false);

            }

            //if too close move to a different way point
            else if(Vector3.Distance(this.transform.position, wayPoints[wayPointIndex].transform.position) <= 2)
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

        void OnTriggerEnter(Collider collider)
        {
            if(collider.tag == "Player")
            {
                state = basicAI.State.CHASE;
                target = collider.gameObject;
            }
        }

    }
}

