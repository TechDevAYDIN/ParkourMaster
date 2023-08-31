using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrailTest : MonoBehaviour
{
    public bool isPrint;
    [SerializeField] SkinnedMeshRenderer[] bodyRenderers;
    [SerializeField] SkinnedMeshRenderer[] itemRenderers;
    [SerializeField] Material bodyMat, itemMat;
    [SerializeField] Material mat;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PrintMesh());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPrint = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isPrint = false;
        }
    }
    public void PrintOne()
    {
        GameObject objP = new GameObject();
        objP.transform.position = transform.position;

        foreach (SkinnedMeshRenderer smr in bodyRenderers)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = objP.transform;
            obj.transform.localPosition = smr.transform.localPosition;
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            MeshFilter mf = obj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);
            mf.mesh = mesh;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.material = bodyMat;
        }
        foreach (SkinnedMeshRenderer smr in itemRenderers)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = objP.transform;
            obj.transform.localPosition = smr.transform.localPosition;
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            MeshFilter mf = obj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);
            mf.mesh = mesh;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.material = itemMat;
            //OptimizeMesh om = obj.AddComponent<OptimizeMesh>();
        }
        objP.AddComponent<FadeAway>().opRate = (PlayerController.singleton.runSpeed * .008f) * PlayerController.singleton.runSpeed;
    }
    IEnumerator PrintMesh()
    {
        while (true)
        {
            if (isPrint)
            {
                GameObject objP = new GameObject();
                objP.transform.position = transform.position;

                foreach (SkinnedMeshRenderer smr in bodyRenderers)
                {
                    GameObject obj = new GameObject();
                    obj.transform.parent = objP.transform;
                    obj.transform.localPosition = smr.transform.localPosition;
                    MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                    MeshFilter mf = obj.AddComponent<MeshFilter>();
                    
                    Mesh mesh = new Mesh();
                    smr.BakeMesh(mesh);
                    mf.mesh = mesh;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;
                    mr.material = bodyMat;
                }
                foreach (SkinnedMeshRenderer smr in itemRenderers)
                {
                    GameObject obj = new GameObject();
                    obj.transform.parent = objP.transform;
                    obj.transform.localPosition = smr.transform.localPosition;
                    MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                    MeshFilter mf = obj.AddComponent<MeshFilter>();

                    Mesh mesh = new Mesh();
                    smr.BakeMesh(mesh);
                    mf.mesh = mesh;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;
                    mr.material = itemMat;
                    //OptimizeMesh om = obj.AddComponent<OptimizeMesh>();
                }
                objP.AddComponent<FadeAway>().opRate = .7f;
            }
            yield return new WaitForSeconds(.3f);
        }
    }
    IEnumerator DeleteMesh(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(obj);
    }
}
