using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class PooledObject : MonoBehaviour
{
    public enum RecycleBehaviour
    {
        AsIs,
        OwnValues,
        ShallowValues,
        DeepValues
    }

    [Tooltip("How are instances recycled. AsIs returns the instance unaltered from its recycled state. " +
             "OwnValues resets only the pooled object itself by copying public values from the original prefab. " +
             "ShallowValues copies from all components on the prefab gameobject. " +
             "DeepValues does a recursive value copy of all components in the entire prefab tree.")]
    public RecycleBehaviour _recycleBehaviour;

    public ObjectPool pool { get; set; }

    public virtual void OnRecycled()
    {
        switch (_recycleBehaviour)
        {
            case RecycleBehaviour.AsIs: break;
            case RecycleBehaviour.OwnValues:
                transform.localScale = pool.prefab.transform.localScale;
                transform.localRotation = pool.prefab.transform.localRotation;
                CopyPublicValues(pool.prefab, this);
                break;
            case RecycleBehaviour.ShallowValues:
                CopyComponentValues(pool.prefab.gameObject, gameObject);
                break;
            case RecycleBehaviour.DeepValues:
                CopyComponentValuesRecursively(pool.prefab.transform, transform);
                break;
        }
    }

    public void Destroy(GameObject go)
    {
        if (go == gameObject)
            throw new Exception("Do not call Destroy() on PooledObject - used ObjectPool.Recycle() to recycle or Object.Destroy() if you really want to destroy the object");
        Object.Destroy(go);
    }

    /**
* Utility functions to ease resetting a pooled object
**/
    protected void CopyComponentValuesRecursively(Transform source, Transform destination)
    {
        CopyComponentValues(source.gameObject, destination.gameObject);
        for (int c = 0; c < source.childCount; c++)
            CopyComponentValuesRecursively(source.transform.GetChild(c), destination.transform.GetChild(c));
    }

    protected void CopyComponentValues(GameObject source, GameObject destination)
    {
        var src_comps = source.GetComponents<Component>();
        var dst_comps = destination.GetComponents<Component>();
        for (int c = 0; c < src_comps.Length; c++)
        {
            CopyPublicValues(src_comps[c], dst_comps[c]);
            if (dst_comps[c] is ParticleSystem)
            {
                ParticleSystem sys = (ParticleSystem)dst_comps[c];
                if (sys.main.playOnAwake) // && !sys.isPlaying)
                {
                    sys.Clear();
                    sys.Play();
                }
            }

            if (dst_comps[c] is TrailRenderer)
            {
                TrailRenderer tr = (TrailRenderer)dst_comps[c];
                tr.Clear();
            }
        }
    }

    protected void CopyPublicValues<T>(T source, T destination)
    {
        var type = source.GetType();
        var fields = type.GetFields();
        for (int f = 0; f < fields.Length; f++)
        {
            if (fields[f].IsPublic && fields[f].FieldType.IsValueType)
            {
                fields[f].SetValue(destination, fields[f].GetValue(source));
            }
        }
    }
}

