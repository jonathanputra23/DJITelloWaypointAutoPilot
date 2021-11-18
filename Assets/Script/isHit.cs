using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class isHit : MonoBehaviour
{
    public GameObject agent;
    public GameObject guide;

    private MovePlayerDestination MPD;
    private MoveGuideDestination MGD;

    private NavMeshAgent agentNM;
    private NavMeshAgent guideNM;

    public bool agentHitStep = false;
    public bool agentHitGuide = false;
    public bool guideHitStep = false;

    private Collider agentCol;
    private Collider guideCol;

    private GameObject willBeDestroyed;

    private Vector3 temp = Vector3.zero;

    void Start()
    {
        MPD = agent.GetComponent<MovePlayerDestination>();
        MGD = guide.GetComponent<MoveGuideDestination>();

        agentNM = agent.GetComponent<NavMeshAgent>();
        guideNM = guide.GetComponent<NavMeshAgent>();

        agentCol = agent.GetComponent<Collider>();
        guideCol = guide.GetComponent<Collider>();

        agentNM.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        guideNM.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

    }
    void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.tag == "player" && other.gameObject.tag == "Guide")
        {
            //guideCol.enabled = true;
            agentCol.enabled = false;
            agentHitGuide = true;
            StartCoroutine(guideRouting());
            Debug.Log("Guide hit player");
            agentHitGuide = false;

        }
        else if (this.gameObject.tag == "player" && other.gameObject.tag == "step")
        {
            Debug.Log("player hit step");

            agentHitStep = true;
            willBeDestroyed = other.gameObject;
            DestroyStep();
            agentHitStep = false;
        }
        else if (this.gameObject.tag == "Guide" && other.gameObject.tag == "step2")
        {

            guideHitStep = true;
            StartCoroutine(playerRouting());
            //guideCol.enabled = false;
            agentCol.enabled = true;
            Debug.Log("Guide hit step");
            guideHitStep = false;

            //guideCol.enabled = true;
            //agentCol.enabled = false;
        }
    }

    IEnumerator playerRouting()
    {
        MGD.destroyAll();
        yield return new WaitForSeconds(1f);
        MPD.Mulai();
    }

    IEnumerator guideRouting()
    {
        MPD.destroyAll();
        yield return new WaitForSeconds(1f);
        MGD.Mulai();
    }

    void OnTriggerExit(Collider other)
    {
    }

    public void DestroyStep()
    {
        Destroy(willBeDestroyed);
    }
}
