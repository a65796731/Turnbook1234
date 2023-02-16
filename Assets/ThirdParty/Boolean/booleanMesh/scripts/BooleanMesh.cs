using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BooleanMesh
{

    MeshCollider ObjectA;
    MeshCollider ObjectB;

    Triangulation triangulationA;
    Triangulation triangulationB;

    float distance, customDistance;



    public static Mesh doCut(MeshCollider A, MeshCollider B)
    {
        BooleanMesh bm = new BooleanMesh(A, B);
        return bm.Difference();

    }


    public BooleanMesh(MeshCollider A, MeshCollider B)
    {

        this.ObjectA = A;
        this.ObjectB = B;
        this.triangulationA = new Triangulation(A);
        this.triangulationB = new Triangulation(B);
        this.distance = 100f;

    }

    class intersectionDATA
    {

        public Triangulation A, B;
        public MeshCollider meshColliderB;
        public int triangleA;
        public float customDistance;
        public Ray r1, r2;
        public RaycastHit hit;

        public intersectionDATA(Triangulation a, Triangulation b, MeshCollider m)
        {

            this.A = a;
            this.B = b;
            this.meshColliderB = m;
            this.r1 = new Ray();
            this.r2 = new Ray();
            this.hit = new RaycastHit();

        }

    }

    void intersectionPoint(intersectionDATA var)
    {

        var.A.AddWorldPointOnTriangle(var.hit.point, var.triangleA);
        var.B.AddWorldPointOnTriangle(var.hit);

    }

    void intersectionRay(int originVertice, int toVertice, intersectionDATA var)
    {

        var.r1.origin = var.A.vertices[var.A.triangles[var.triangleA].indexVertice[originVertice]].pos;
        var.r2.origin = var.A.vertices[var.A.triangles[var.triangleA].indexVertice[toVertice]].pos;
        var.r1.direction = (var.r2.origin - var.r1.origin).normalized;
        var.r2.direction = (var.r1.origin - var.r2.origin).normalized;

        var.customDistance = Vector3.Distance(var.r1.origin, var.r2.origin);

        if (var.A.vertices[var.A.triangles[var.triangleA].indexVertice[originVertice]].type == 0) if (var.meshColliderB.Raycast(var.r1, out var.hit, var.customDistance)) intersectionPoint(var);
        if (var.A.vertices[var.A.triangles[var.triangleA].indexVertice[toVertice]].type == 0) if (var.meshColliderB.Raycast(var.r2, out var.hit, var.customDistance)) intersectionPoint(var);

    }

    void AInToB(intersectionDATA var)
    {

        // Vertices A In MeshCollider B
        for (int i = 0; i < var.A.vertices.Count; i++)
        {

            if (In(var.meshColliderB, var.A.vertices[i].pos)) var.A.vertices[i].type = -1; //In
            else var.A.vertices[i].type = 0; //Out

        }

    }

    void intersectionsAtoB(intersectionDATA var)
    {

        for (int i = 0; i < var.A.triangles.Count; i++)
        {

            var.triangleA = i;
            intersectionRay(0, 1, var);
            intersectionRay(0, 2, var);
            intersectionRay(1, 2, var);

        }

    }

    void clearVertices(Triangulation triangulation, int t)
    {

        int i, w;

        for (i = triangulation.triangles.Count - 1; i > -1; i--)
        {
            for (w = triangulation.triangles[i].indexVertice.Count - 1; w > -1; w--)
            {

                if (triangulation.vertices[triangulation.triangles[i].indexVertice[w]].type == t) triangulation.triangles[i].indexVertice.RemoveAt(w);

            }

            if (triangulation.triangles[i].indexVertice.Count < 3) triangulation.triangles.RemoveAt(i);

        }

    }

    void recalculateTriangles(Vector3[] vertices, Vector3[] normals, int[] triangles)
    {

        Vector3 a, b, c;
        int v1, v2, v3;

        for (int i = 0; i < triangles.Length; i += 3)
        {

            v1 = triangles[i];
            v2 = triangles[i + 1];
            v3 = triangles[i + 2];

            a = vertices[v1];
            b = vertices[v2];
            c = vertices[v3];

            if (Vector3.Dot(normals[v1] + normals[v2] + normals[v3], Vector3.Cross((b - a), (c - a))) < 0f)
            {

                triangles[i + 2] = v1;
                triangles[i] = v3;

            }

        }

    }

    Mesh triangulationMesh()
    {

        //by zwj 
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //原来串行计算，复杂模型效率低
        //this.triangulationA.Calculate();
        //this.triangulationB.Calculate(); 
        //by zwj 此处采用并行计算可以使得效率极大提高-可能会导致破面（不知道为什么很神奇）
        //Parallel.For(0, 1, (k) =>
        //{
        //    if (k == 0)
        //    {
        //        this.triangulationA.Calculate();
        //    }
        //    else
        //    {
        //        this.triangulationB.Calculate();
        //    }
        //});
        //改成多任务线程计算，效率提高,貌似没有问题
        Task ta = Task.Factory.StartNew(() => { triangulationA.Calculate(); });
        Task tb = Task.Factory.StartNew(() => { triangulationB.Calculate(); });
        Task.WaitAll(ta, tb);
        //Debug.Log(" triangulationA triangulationB .Calculate:" + sw.ElapsedMilliseconds);
        //sw.Restart();
        int i;
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;

        int tA = this.triangulationA.triangles.Count;
        int tB = this.triangulationB.triangles.Count;

        int[] trianglesA = new int[tA * 3];
        int[] trianglesB = new int[tB * 3];

        this.triangulationA.AddTriangles(triangulationB.vertices.ToArray(), triangulationB.triangles.ToArray());
        this.triangulationA.updateLocalPosition(ObjectA.transform);

        Vector3[] vertices = new Vector3[triangulationA.vertices.Count];
        Vector3[] normals = new Vector3[triangulationA.vertices.Count];
        Vector2[] uv = new Vector2[triangulationA.vertices.Count];

        for (i = 0; i < triangulationA.vertices.Count; i++)
        {

            vertices[i] = triangulationA.vertices[i].localPos;
            normals[i] = triangulationA.vertices[i].normal.normalized;
            uv[i] = triangulationA.vertices[i].uv;

        }

        for (i = 0; i < tA; i++)
        {

            trianglesA[i * 3] = triangulationA.triangles[i].indexVertice[0];
            trianglesA[i * 3 + 1] = triangulationA.triangles[i].indexVertice[1];
            trianglesA[i * 3 + 2] = triangulationA.triangles[i].indexVertice[2];

        }

        for (i = 0; i < tB; i++)
        {

            trianglesB[i * 3] = triangulationA.triangles[tA + i].indexVertice[0];
            trianglesB[i * 3 + 1] = triangulationA.triangles[tA + i].indexVertice[1];
            trianglesB[i * 3 + 2] = triangulationA.triangles[tA + i].indexVertice[2];

        }

        recalculateTriangles(vertices, normals, trianglesA);
        recalculateTriangles(vertices, normals, trianglesB);
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.SetTriangles(trianglesA, 0);
        mesh.SetTriangles(trianglesB, 1);

        return mesh;

    }

    public Mesh Union()
    {

        intersections();
        clearVertices(this.triangulationA, -1);
        clearVertices(this.triangulationB, -1);
        return triangulationMesh();

    }

    public Mesh Intersection()
    {

        intersections();
        clearVertices(this.triangulationA, 0);
        clearVertices(this.triangulationB, 0);
        return triangulationMesh();

    }

    public Mesh Difference()
    {

        intersections();
        clearVertices(this.triangulationA, -1);
        clearVertices(this.triangulationB, 0);
        this.triangulationB.invertNormals();
        return triangulationMesh();

    }

    void intersections()
    {

        //Update world position vertices
        this.triangulationA.updateWorldPosition(ObjectA.transform);
        this.triangulationB.updateWorldPosition(ObjectB.transform);

        //IntersectionDATA
        intersectionDATA varA = new intersectionDATA(this.triangulationA, this.triangulationB, this.ObjectB);
        intersectionDATA varB = new intersectionDATA(this.triangulationB, this.triangulationA, this.ObjectA);

        //In/Out Points
        AInToB(varA);
        AInToB(varB);

        //Intersections
        intersectionsAtoB(varA);
        intersectionsAtoB(varB);

    }




    ///////////////////////////////////////////////////////////////////////////////

    bool r, l, u, d, f, b;

    RaycastHit rightHit = new RaycastHit();
    RaycastHit leftHit = new RaycastHit();
    RaycastHit upHit = new RaycastHit();
    RaycastHit downHit = new RaycastHit();
    RaycastHit forwardHit = new RaycastHit();
    RaycastHit backHit = new RaycastHit();
    RaycastHit tempHit = new RaycastHit();

    Ray right = new Ray(Vector3.zero, -Vector3.right);
    Ray left = new Ray(Vector3.zero, -Vector3.left);
    Ray up = new Ray(Vector3.zero, -Vector3.up);
    Ray down = new Ray(Vector3.zero, -Vector3.down);
    Ray forward = new Ray(Vector3.zero, -Vector3.forward);
    Ray back = new Ray(Vector3.zero, -Vector3.back);
    Ray tempRay = new Ray();

    bool ConcaveHull(MeshCollider meshCollider, Vector3 position, Ray ray, RaycastHit hit)
    {


        tempRay.origin = position;
        tempRay.direction = -ray.direction;
        customDistance = distance - hit.distance;

        while (meshCollider.Raycast(tempRay, out tempHit, customDistance))
        {

            if (tempHit.triangleIndex == hit.triangleIndex) break;
            ray.origin = -ray.direction * customDistance + position;

            if (!meshCollider.Raycast(ray, out hit, customDistance)) return true;

            if (tempHit.triangleIndex == hit.triangleIndex) break;
            customDistance -= hit.distance;

        }

        return false;

    }

    //start --by zwj 高效检查点是否在模型内部
    bool In(MeshCollider meshCollider, Vector3 position)
    {
        Mesh m = meshCollider.sharedMesh;
        int[] trians = m.triangles;
        Vector3[] vexes = m.vertices;
        Matrix4x4 l2w = meshCollider.transform.localToWorldMatrix;
        Parallel.For(0, vexes.Length, (i) =>
        {
            vexes[i] = l2w.MultiplyPoint3x4(vexes[i]);

        });
        //
        int cntTri = trians.Length / 3;
        Vector3 vOri = position;
        Vector3 vDir = (meshCollider.bounds.center - vOri).normalized;
        object objLocker = new object();
        int nInCnt = 0;
        List<Vector3> dots = new List<Vector3>();
        Parallel.For(0, cntTri, (j) =>
        {

            Vector3 v0 = vexes[trians[j * 3 + 0]];
            Vector3 v1 = vexes[trians[j * 3 + 1]];
            Vector3 v2 = vexes[trians[j * 3 + 2]];
            Vector3 vn = Vector3.Cross(v1 - v0, v2 - v0);
            Vector3 vcross;
            bool b = _intersetionRayWithPlane(vOri, vDir, v0, vn, out vcross);
            if (b)
            {

                bool bIn = _pointInTriangle(vcross, v0, v1, v2);
                if (bIn)
                {

                    //相交一次
                    lock (objLocker)
                    {
                        // Debug.Log("-------相交一次--------" + vcross);
                        // Debug.Log(v0);
                        // Debug.Log(v1);
                        // Debug.Log(v2);
                        if (_findDots(dots, vcross))
                        {
                            //  Debug.Log("-------重复交点--------");
                        }
                        else
                        {
                            nInCnt++;
                            dots.Add(vcross);
                        }
                    }
                }

            }
        });
        if (nInCnt % 2 == 0)
        {
            //在外边
            // Debug.Log("---------------out:" + nInCnt);
            return false;
        }

        //在内部
        // Debug.Log("in++++++++++++++++++:" + nInCnt);
        return true;


    }

    private bool _findDots(List<Vector3> list, Vector3 pnt)
    {
        foreach (Vector3 v in list)
        {
            if (v == pnt)
            {
                return true;
            }
        }
        return false;
    }

    private bool _pointInTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 pa = point - a;
        Vector3 pb = point - b;
        Vector3 pc = point - c;

        if ((pa.normalized + pb.normalized) == Vector3.zero ||
           (pc.normalized + pb.normalized) == Vector3.zero ||
           (pa.normalized + pc.normalized) == Vector3.zero)
        {
            //点在三角形边上，算在内
            return true;
            //
        }

        Vector3 pab = Vector3.Cross(pa, pb);
        Vector3 pbc = Vector3.Cross(pb, pc);
        Vector3 pca = Vector3.Cross(pc, pa);


        float d1 = Vector3.Dot(pab, pbc);
        float d2 = Vector3.Dot(pab, pca);
        float d3 = Vector3.Dot(pbc, pca);
        //   Debug.Log(d1 + "," + d2 + "," + d3);
        if (d1 > 0 && d2 > 0 && d3 > 0)
        {
            return true;
        }
        return false;

    }

    private bool _intersetionRayWithPlane(Vector3 rayOri, Vector3 rayDir, Vector3 planePoint, Vector3 planeNormal, out Vector3 result)
    {
        float d = Vector3.Dot(planePoint - rayOri, planeNormal);
        if (d == 0)
        {
            //射线源在平面上,交点就是射线源
            result = rayOri;
            return true;
        }
        float d2 = Vector3.Dot(rayDir.normalized, planeNormal);
        if (d2 == 0)
        {
            //射线方向和平面平行了
            result = Vector3.zero;
            return false;
        }

        //求出直线交点
        d = d / d2;
        Vector3 vcro = d * rayDir.normalized + rayOri;
        //如果交点是沿着射线的方向，则判断相交，如果交点在射线的反向则不交
        float d3 = Vector3.Dot(rayDir, vcro - rayOri);
        if (d3 < 0)
        {
            result = Vector3.zero;
            return false;
        }
        result = vcro;
        return true;
    }





    //---end by zwj
    //原来的检查点十分在模型内部的方法，特殊情况会卡死
    bool In_old(MeshCollider meshCollider, Vector3 position)
    {

        right.origin = -right.direction * distance + position;
        left.origin = -left.direction * distance + position;
        up.origin = -up.direction * distance + position;
        down.origin = -down.direction * distance + position;
        forward.origin = -forward.direction * distance + position;
        back.origin = -back.direction * distance + position;

        r = meshCollider.Raycast(right, out rightHit, distance);
        l = meshCollider.Raycast(left, out leftHit, distance);
        u = meshCollider.Raycast(up, out upHit, distance);
        d = meshCollider.Raycast(down, out downHit, distance);
        f = meshCollider.Raycast(forward, out forwardHit, distance);
        b = meshCollider.Raycast(back, out backHit, distance);

        if (r && l && u && d && f && b)
        {

            if (!ConcaveHull(meshCollider, position, right, rightHit))
                if (!ConcaveHull(meshCollider, position, left, leftHit))
                    if (!ConcaveHull(meshCollider, position, up, upHit))
                        if (!ConcaveHull(meshCollider, position, down, downHit))
                            if (!ConcaveHull(meshCollider, position, forward, forwardHit))
                                if (!ConcaveHull(meshCollider, position, back, backHit)) return true;

        }

        return false;

    }

    ///////////////////////////////////////////////////////////////////////////////


}
