using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private NPCController npc;
    private bool detectPlayer = false;


    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float MeshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter fovMesh;
    Mesh viewMesh;


    // Define a dictionary to store the original color of each target
    private Dictionary<Transform, Color> originalColors = new Dictionary<Transform, Color>();

    private void Awake()
    {
        npc = GetComponent<NPCController>();
    }

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "Field of View";
        fovMesh.mesh = viewMesh;

        //StartCoroutine("FindTargetsWithDelay", .2f);
        StartCoroutine("FindPlayerWithDelay", .2f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }


    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * MeshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();


        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        int
         vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();


    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - maxViewCast.dst) > edgeDstThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    void FindPlayer()
    {
        // Check if player is within view radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            // Check if the collider's game object is on the target layer
            if (targetCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector3 dirToTarget = (targetCollider.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, targetCollider.transform.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        // Change material color of fovMesh to red
                        ChangeFovMeshColor(Color.red);
                        detectPlayer = true;
                        npc.DetectedPlayer(detectPlayer);
                        
                        return;
                    }
                }
            }
        }

        // If player is not found, change fovMesh color back to default
        detectPlayer = !true;
        npc.DetectedPlayer(detectPlayer);
        ChangeFovMeshColor(Color.white);

    }


    IEnumerator FindPlayerWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindPlayer();
        }
    }

    void ChangeFovMeshColor(Color color)
    {
        MeshRenderer meshRenderer = fovMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            color.a = 0.1f;
            meshRenderer.material.color = color;
        }
    }
}
