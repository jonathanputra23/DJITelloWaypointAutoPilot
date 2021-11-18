//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class MoveDestination : MonoBehaviour
//{

//    public Transform playerGoal;
//    public Transform guideGoal;

//    private LineRenderer line;

//    private NavMeshAgent agent;
//    public NavMeshAgent guide;

//    public isHit checkHit;
//    public playerHit checkPlayerHit;
//    public isHit checkGuideHit;

//    public ObjectData data = new ObjectData();
//    public List<GameObject> targetLoc;

//    public GameObject playerPrefabs;
//    public GameObject guidePrefabs;

//    private bool stepHit;
//    private bool playerHit;
//    private bool status = false;

//    public int count = 0;


//    void Start()
//    {
//        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
//        line = GetComponent<LineRenderer>();
//        line.startWidth = 0.15f;
//        line.endWidth = 0.15f;
//        line.positionCount = 0;
//        StartCoroutine(getPath(agent, playerPrefabs, playerGoal));

//    }

//    void Update()
//    {
//        StartCoroutine(redrawPath());
//        //agent.destination = goal.position;
//        //agent.CalculatePath(goal.position, agent.path);
//        //if (agent.hasPath)
//        //{
//        //    DrawPath();
//        //    //if(agent.)
//        //}
//    }

//    IEnumerator redrawPath()
//    {
//        if (checkPlayerHit.guideHit)
//        {
//            destroyAll();
//            StartCoroutine(getPath(guide, guidePrefabs, guideGoal));
//            yield return new WaitForEndOfFrame();

//            //yield return new WaitUntil(() => checkGuideHit.stepHit);
//            //Debug.Log("hello");
//            //destroyAll();
//            //StartCoroutine(getPath(agent, playerPrefabs, playerGoal));
//        }
//        else if (checkHit.stepHit && agent.hasPath)
//        {

//            checkHit.DestroyStep();
//            checkHit.stepHit = false;
//        }
//    }

//    IEnumerator getPath(NavMeshAgent nav, GameObject prefabs, Transform tPos)
//    {
//        yield return new WaitForEndOfFrame();
//        //line.SetPosition(0, agent.path.corners[0]);

//        nav.SetDestination(tPos.position);

//        StartCoroutine(DrawPath(nav.path, prefabs));
//        nav.Stop();
//    }

//    IEnumerator waitFor()
//    {
//        for (int i = 1; i < data.objects.Count; i++)
//        {
//            Vector3 objectPos = new Vector3(data.objects[i].posX, data.objects[i].posY, data.objects[i].posZ);
//            yield return new WaitUntil(() => checkHit.stepHit);
//            if (checkHit.stepHit)
//            {
//                checkHit.stepHit = false;
//                yield return new WaitUntil(() => checkPlayerHit.guideHit);
//            }
//            if (i + 1 == data.objects.Count)
//            {
//                destroyAll();
//            }
//        }
//    }

//    IEnumerator DrawPath(NavMeshPath path, GameObject prefab)
//    {
//        yield return new WaitForEndOfFrame();
//        path = agent.path;

//        int count = path.corners.Length;
//        line.SetVertexCount(path.corners.Length);

//        Vector3 previousCorner = path.corners[0];
//        for (int i = 1; i < count; i++)
//        {
//            Vector3 currentCorner = path.corners[i];
//            //line.SetPosition(i, path.corners[i]);
//            Vector3 middle = Vector3.Lerp(previousCorner, currentCorner, i * 0.25f);
//            GameObject go = Instantiate(prefab, middle, Quaternion.identity) as GameObject;
//            go.transform.LookAt(previousCorner);
//            previousCorner = currentCorner;
//        }
//        DrawPathLine();
//        getStep();
//        //StartCoroutine(waitFor());
//    }

//    public void getStep()
//    {
//        GameObject[] stepPoint = GameObject.FindGameObjectsWithTag("step");
//        foreach (GameObject point in stepPoint)
//        {
//            Transform pointTransform = point.transform;
//            Vector3 pos = pointTransform.position;
//            StepLocData SLD = new StepLocData(pos.x, pos.y, pos.z);
//            data.objects.Add(SLD);
//        }
//    }

//    void DrawPathLine()
//    {
//        line.positionCount = agent.path.corners.Length;
//        line.SetPosition(0, transform.position);

//        if (agent.path.corners.Length < 2)
//        {
//            return;
//        }

//        for (int i = 1; i < agent.path.corners.Length; i++)
//        {
//            Vector3 pointPosition = new Vector3(agent.path.corners[i].x, agent.path.corners[i].y, agent.path.corners[i].z);
//            line.SetPosition(i, pointPosition);
//        }
//    }

//    public class ObjectData
//    {
//        public List<StepLocData> objects;
//        public ObjectData()
//        {
//            this.objects = new List<StepLocData>();
//        }
//    }

//    public class StepLocData
//    {
//        public float posX;
//        public float posY;
//        public float posZ;
//        public StepLocData(float px, float py, float pz)
//        {
//            this.posX = px;
//            this.posY = py;
//            this.posZ = pz;
//        }
//    }

//    public void destroyAll()
//    {
//        GameObject[] willDelete = GameObject.FindGameObjectsWithTag("step");
//        foreach (GameObject objDel in willDelete)
//        {
//            Destroy(objDel);
//        }
//        data.objects.Clear();
//        line.positionCount = 0;
//        agent.ResetPath();
//    }
//}