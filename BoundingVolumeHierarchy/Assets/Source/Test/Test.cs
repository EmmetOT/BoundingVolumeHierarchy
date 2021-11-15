using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using BoundingVolumeHierarchy;
using UnityEditor;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    private static Test s_instance;
    public static Test Instance => s_instance ? s_instance : s_instance = FindObjectOfType<Test>();

    [SerializeField]
    private Transform m_raycastSource;

    [SerializeField]
    private Color[] m_depthColours;

    private List<TestObject> m_objects;
    private bool m_initialized = false;
    private BoundingVolumeHierarchy<TestObject> m_bvh;

    private void OnEnable() => Init();

    private void Init()
    {
        m_initialized = true;
        m_bvh = new BoundingVolumeHierarchy<TestObject>();
        m_objects = new List<TestObject>();

        TestObject[] objs = FindObjectsOfType<TestObject>();

        for (int i = 0; i < objs.Length; i++)
        {
            Add(objs[i]);
        }
    }

    private void Update()
    {
        if (!m_initialized)
            Init();

        if (m_bvh == null)
            return;

        List<TestObject> objects = new List<TestObject>(m_objects);

        foreach (TestObject obj in objects)
        {
            if (obj.IsDirty)
            {
                m_bvh.Update(obj);
                obj.SetClean();
            }
        }
    }

    public void Add(TestObject obj)
    {
        if (!m_initialized)
            Init();

        if (m_objects.Contains(obj))
            return;

        m_objects.Add(obj);

        if (m_bvh != null)
            m_bvh.Add(obj);
    }

    public void Remove(TestObject obj)
    {
        if (!m_initialized)
            Init();

        if (!m_objects.Contains(obj))
            return;

        m_objects.Remove(obj);

        if (m_bvh != null)
            m_bvh.Remove(obj);
    }

    [ContextMenu("Rebuild Tree")]
    public void RebuildTree()
    {
        if (m_bvh == null)
            return;

        m_bvh.RebuildBottomUp();
    }

    public void OnDrawGizmos()
    {
        if (m_bvh == null || m_bvh.RootIsNull)
            return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        foreach ((BoundingVolumeHierarchy<TestObject>.Node node, int depth) in m_bvh.EnumerateNodes())
        {
            int modDepth = depth % m_depthColours.Length;

            Color col = m_depthColours[modDepth];

            Handles.color = col;
            Handles.DrawWireCube(node.AABB.center, node.AABB.size);
        }

        if (m_raycastSource)
        {
            if (m_bvh.Raycast(new Ray(m_raycastSource.position, m_raycastSource.forward), out BoundingVolumeHierarchy<TestObject>.Node hit))
            {
                Handles.color = Color.white;
                Handles.DrawWireCube(hit.AABB.center, hit.AABB.size - Vector3.one * 0.05f);
            }
            else
            {
                Handles.color = Color.black;
            }

            Handles.DrawLine(m_raycastSource.position, m_raycastSource.position + m_raycastSource.forward * 100f);
        }
    }
}