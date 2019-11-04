using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class EnemyIA : PunBehaviour
{
    public enum EnemyState
    {
        Chase,
        Patrolling,
        Attacking
    }

    public int HP = 100;
    public Animator animator;
    public EnemyState status;
    public Rigidbody enemy_rigidbody;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public GameObject playertoChase;
    public List<Transform> visibleTargets = new List<Transform>();
    public Transform patternPoint;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float stoppingDistance;
    public float speed;
    public float WalkDistance;
    public float startWaitTime;
    private float waitTime;

   
    
    float minX, maxX, minY, maxY;



    void Start()
    {
        minX = 4.75f;
        maxX = 24.75f;
        minY = 4.75f;
        maxY = 14.75f;
        //patternPoint.gameObject.GetComponent<PatternPoint>().GenerateNewPosition(minX, maxX, minY, maxY);
        waitTime = startWaitTime;
        speed = 1f;
        status = EnemyState.Patrolling;
        StartCoroutine("FindTargets", .2f);


    }

    public void OnEnable()
    {
        minX = 4.75f;
        maxX = 24.75f;
        minY = 4.75f;
        maxY = 14.75f;
        //patternPoint.gameObject.GetComponent<PatternPoint>().GenerateNewPosition(minX, maxX, minY, maxY);
        waitTime = startWaitTime;
        speed = 1f;
        status = EnemyState.Patrolling;
        StartCoroutine("FindTargets", .2f);
        HP = 100;
    }

    IEnumerator FindTargets(float delay)
    {
        while (true)
        {
            //Cada cierto tiempo busco jugadores en mi rango de visión
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    //Metodo con el que saco el angúlo de visión de mi enemy
    public Vector3 DirFromAngle(float angleInDegres, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegres += transform.eulerAngles.y;
        }
        //Regresó un vector3 multiplicando el seno y coseno del triangulo que quiero sacar
        return new Vector3(Mathf.Sin(angleInDegres * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegres * Mathf.Deg2Rad));
    }

    void FindVisibleTargets()
    {
        
        //Limpio la lista de jugadores en vista, para evitar que se acaparé
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {

            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {

                    visibleTargets.Add(target);

                    status = EnemyState.Chase;
                    if (visibleTargets.Count == 1)
                    {
                        playertoChase = target.gameObject;
                    }
                    else
                    {
                        for (int x = 0; x < visibleTargets.Count; x++)
                        {
                            if ((Vector3.Distance(transform.position, visibleTargets[x].transform.position) < (Vector3.Distance(transform.position, playertoChase.transform.position))))
                            {
                                playertoChase = visibleTargets[x].gameObject;
                            }
                        }

                    }

                }
                /*
                */
            }
        }
       
      
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitMelee"))
        {
            animator.SetTrigger("hit");
            HP -= 10;
            Debug.Log(HP);
        }
        else if (other.gameObject.CompareTag("Proyectile"))
        {
            animator.SetTrigger("hit");
            HP -= 50;
            Debug.Log(HP);
        }
    }

    void PatrolArea()
    {
        //Me muevo al patrollingPoint
        transform.position = Vector3.MoveTowards(transform.position, patternPoint.position, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, patternPoint.position) < 3f)
        {
            if(waitTime <= 0)
            {
                //Asigno nuevo punto de patrolling después de que paso el tiempo de espera
                patternPoint.gameObject.GetComponent<PatternPoint>().GenerateNewPosition(minX, maxX, minY, maxY);
              
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void ChasePlayer()
    {
        
        //Persigo a jugador
        if (Vector3.Distance(transform.position, playertoChase.transform.position) > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, playertoChase.transform.position, speed * Time.deltaTime);
        }
        if(Vector3.Distance(transform.position, playertoChase.transform.position) <= stoppingDistance)
        {
            Debug.Log("ataques");
            animator.SetTrigger("attak");
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Attack1"))
            {
                Debug.Log("jijiji");
            }
        }
           
        
        //Paso a estado patrolling si no hay jugadores cerca
        if (visibleTargets.Count == 0)
        {
                playertoChase = null;
                status = EnemyState.Patrolling;
                enemy_rigidbody.velocity = Vector3.zero;
                enemy_rigidbody.angularVelocity = Vector3.zero;
        }
    }

    /*void CreatePatrolPattern()
    {
        /*patternPoints.Clear();
        //Notas para quienes lean: en los ejes X se dibujan lineas rojas si NO TOCAN TERRENOS/obstaculos
        //Si chocan con algo, las lineas se pintan de amarillo indicando que ese es el limite
        RaycastHit hit;
        RaycastHit backHit;
        RaycastHit rightHit;
        RaycastHit leftHit;
    
        //RAyo que va en dirección frontal
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f) && hit.collider.CompareTag("Terrain"))
        {

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            patternPoints.Add(hit.transform);
        }
        else
        {
            Debug.Log(hit.point);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.black);
        }

        //Rayo que va en direción abajo
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out backHit, 5f) && backHit.collider.CompareTag("Terrain"))
        {
           Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * backHit.distance, Color.yellow);
            patternPoints.Add(backHit.transform);
        }
        else
        {
            //patternPoints.Add(backHit.point);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * 5, Color.black);
        }
        //Rayo que va en direción derecha
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out rightHit, 5f) && rightHit.collider.CompareTag("Terrain"))
        {

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * rightHit.distance, Color.yellow);
            patternPoints.Add(rightHit.transform);
        }
        else
        {
            //patternPoints.Add(rightHit.point);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 5, Color.red);
        }


        //Rayo que va en direción izquierda
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out leftHit, 5f) && leftHit.collider.CompareTag("Terrain"))
        {
         
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * leftHit.distance, Color.yellow);
            patternPoints.Add(leftHit.transform);
        }
        else
        {
            //patternPoints.Add(leftHit.point);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 5, Color.red);
        }

        status = EnemyState.Patrolling;
        
    }
*/


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
    }
        void Update()
    {
        /*if (status == EnemyState.Resting)
        {
            //TODO: Este metodo se encargará de castear raycast para ver en que direcciones puede moverse antes de entrar en modo
            CreatePatrolPattern();
        }
        */

       
        if (photonView.isMine)
        {
            if (status == EnemyState.Chase)
            {

                if (playertoChase != null)
                {
                    ChasePlayer();
                }
                else
                {
                    status = EnemyState.Patrolling;
                }

            }
            else if (status == EnemyState.Patrolling)
            {
                PatrolArea();
            }
        }
        if (HP <= 0)
        {
            animator.SetBool("death", true);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Death"))
            {
                this.gameObject.SetActive(false);
            }
            
        }
    }


}
