using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoundingVolumeHierarchy;
using UnityEditor;

[ExecuteInEditMode]
public class TestObject : MonoBehaviour, IBVHClientObject
{
    [SerializeField]
    private Vector3 m_size = Vector3.one;

    [SerializeField]
    private Color m_color = Color.white;

    private Vector3 m_previousPosition;
    public Vector3 PreviousPosition => m_previousPosition;

    //public Vector3 Displacement => Position - PreviousPosition;

    public Vector3 Position => transform.position;

    private static IEnumerable<Vector3> EnumerateCorners(float x, float y, float z)
    {
        yield return new Vector3(-x, -y, -z);
        yield return new Vector3(x, -y, -z);
        yield return new Vector3(-x, y, -z);
        yield return new Vector3(x, y, -z);
        yield return new Vector3(-x, -y, z);
        yield return new Vector3(x, -y, z);
        yield return new Vector3(-x, y, z);
        yield return new Vector3(x, y, z);
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[] m_untransformedCubeBoundsCorners = new Vector3[8];

    [SerializeField]
    [HideInInspector]
    private MeshRenderer m_cubeRenderer;

    private Material m_cubeMaterial;

    private void RefreshCornersArray()
    {
        m_untransformedCubeBoundsCorners[0] = new Vector3(-m_size.x, -m_size.y, -m_size.z);
        m_untransformedCubeBoundsCorners[1] = new Vector3(m_size.x, -m_size.y, -m_size.z);
        m_untransformedCubeBoundsCorners[2] = new Vector3(-m_size.x, m_size.y, -m_size.z);
        m_untransformedCubeBoundsCorners[3] = new Vector3(m_size.x, m_size.y, -m_size.z);
        m_untransformedCubeBoundsCorners[4] = new Vector3(-m_size.x, -m_size.y, m_size.z);
        m_untransformedCubeBoundsCorners[5] = new Vector3(m_size.x, -m_size.y, m_size.z);
        m_untransformedCubeBoundsCorners[6] = new Vector3(-m_size.x, m_size.y, m_size.z);
        m_untransformedCubeBoundsCorners[7] = new Vector3(m_size.x, m_size.y, m_size.z);
    }

    private void OnEnable()
    {
        RefreshCornersArray();

        if (!Test.Instance)
            return;

        Test.Instance.Add(this);
    }

    private void OnDisable()
    {
        RefreshCornersArray();

        if (!Test.Instance)
            return;

        Test.Instance.Remove(this);
    }

    private bool m_isDirty = true;
    public bool IsDirty => m_isDirty;

    private void Update()
    {
        if (transform.hasChanged)
        {
            m_isDirty = true;
            transform.hasChanged = false;

            UpdateCube();
        }
    }

    public void SetClean()
    {
        m_isDirty = false;
        m_previousPosition = transform.position;
    }

    public Bounds GetBounds()
    {
        static Vector3 Min(Vector3 a, Vector3 b) => new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        static Vector3 Max(Vector3 a, Vector3 b) => new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));

        Vector3 min = Vector3.one * float.PositiveInfinity;
        Vector3 max = Vector3.one * float.NegativeInfinity;

        for (int i = 0; i < m_untransformedCubeBoundsCorners.Length; i++)
        {
            Vector3 transformed = transform.TransformPoint(m_untransformedCubeBoundsCorners[i]);
            min = Min(min, transformed);
            max = Max(max, transformed);
        }

        return new Bounds((min + max) * 0.5f, (max - min) * 0.5f);
    }

    private void Reset()
    {
        RefreshCornersArray();

        m_color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        m_isDirty = true;

        DestroyCube();
        UpdateCube();
    }

    private void OnValidate()
    {
        RefreshCornersArray();

        m_size = new Vector3(Mathf.Max(0f, m_size.x), Mathf.Max(0f, m_size.y), Mathf.Max(0f, m_size.z));
        m_isDirty = true;

        m_isOnValidate = true;
        UpdateCube();
        m_isOnValidate = false;
    }

    private void UpdateCube(bool newMaterial = false)
    {
        if (!m_cubeRenderer)
        {
            GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeObject.name = "Cube";
            cubeObject.transform.SetParent(transform);
            m_cubeRenderer = cubeObject.GetComponent<MeshRenderer>();

            Collider col = cubeObject.GetComponent<Collider>();

            SafeDestroy(col);
        }

        m_cubeRenderer.transform.localPosition = Vector3.zero;
        m_cubeRenderer.transform.localRotation = Quaternion.identity;
        m_cubeRenderer.transform.localScale = m_size;

        if (newMaterial)
            m_color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        if (!m_cubeMaterial || newMaterial)
        {
            m_cubeMaterial = new Material(Shader.Find("Standard"));
            m_cubeRenderer.material = m_cubeMaterial;
        }

        m_cubeMaterial.SetColor("_Color", m_color);
    }

    private void SafeDestroy(Object obj)
    {
        if (Application.isPlaying || m_isOnValidate)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }

    private void DestroyCube()
    {
        m_cubeRenderer = null;

        foreach (Transform child in transform)
            SafeDestroy(child.gameObject);

        SafeDestroy(m_cubeMaterial);
        m_cubeMaterial = null;
    }

    private bool m_isOnValidate = false;

#if UNITY_EDITOR
    //catch duplication of this GameObject
    [SerializeField]
    [HideInInspector]
    private int m_instanceID;

    void Awake()
    {
        if (Application.isPlaying)
            return;

        if (m_instanceID == 0)
        {
            m_instanceID = GetInstanceID();
            return;
        }

        if (m_instanceID != GetInstanceID() && GetInstanceID() < 0)
        {
            m_instanceID = GetInstanceID();
            UpdateCube(newMaterial: true);
        }
    }
#endif
}
