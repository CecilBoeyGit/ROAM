using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    public float HeightOffset;
    public Vector3 zeroOffsetHeight;
    public Vector3 groundRayHitPos;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    public int edgeIteration;
    public float edgeDistanceThreshold;

    public float FOVMeshResolution;
    public MeshFilter FOVMeshFilter;
    Mesh FOVMesh;

    protected virtual void OnEnable ()
    {
        FOVMesh = new Mesh();
        FOVMesh.name = "FOVMesh";
        FOVMeshFilter.mesh = FOVMesh;

        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    protected virtual void LateUpdate()
    {
        RayCastGround();
        zeroOffsetHeight = new Vector3(transform.position.x, groundRayHitPos.y + HeightOffset, transform.position.z);
        DrawFOVMesh();
    }
    void RayCastGround()
    {
        RaycastHit hit;
        int groundLayerIndex = LayerMask.NameToLayer("Ground");
        LayerMask groundLayerMask = 1 << groundLayerIndex;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            groundRayHitPos = hit.point;
        }
    }
    IEnumerator FindTargetsWithDelay(float delayTime)
    {
        while(true)
        {
            yield return new WaitForSeconds(delayTime);
            FindTarget();
        }
    }

    public virtual void FindTarget()
    {
        visibleTargets.Clear();

        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInView.Length; i++)
        {
            Transform target = targetsInView[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < viewAngle /2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    if(target.gameObject.GetComponent<EnemyBehavior>() != null) //Ensuring that only the enemy core component gameobject is added, instead of adding the children of the enemy core gameobjects as well
                        visibleTargets.Add(target);
                }
            }
        }

        if (visibleTargets.Count != 0)
            TriggerTarget();
    }
    public virtual void TriggerTarget()
    {
        foreach(Transform child in visibleTargets)
        {
            child.GetComponent<EnemyBehavior>().ScannedBehaviors();
        }
    }  
    protected virtual void DrawFOVMesh()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * FOVMeshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for(int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(zeroOffsetHeight, zeroOffsetHeight + DirectionFromAngle(angle, true) * viewRadius, Color.red);
            
            ViewCastInfo newViewCast = ViewCast(angle);
            if(i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        //Tesselating the traingles
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = transform.InverseTransformPoint(zeroOffsetHeight);
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

        FOVMesh.Clear();
        FOVMesh.vertices = vertices;
        FOVMesh.triangles = triangles;
        FOVMesh.RecalculateNormals();
    }
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < edgeIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
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

    ViewCastInfo ViewCast(float globalAngle) //Ray casting individual rays according to the mesh resolution to detect obstacles
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(zeroOffsetHeight, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, zeroOffsetHeight + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
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
}
