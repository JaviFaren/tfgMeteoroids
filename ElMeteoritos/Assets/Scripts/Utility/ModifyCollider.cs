using System.Collections;
using UnityEngine;

public class ModifyCollider : MonoBehaviour
{
    [SerializeField] private Collider targetCollider;
    [SerializeField] private float expandValue;
    [SerializeField] private float expandDuration;

    private enum ColliderType { UNKNOWN, SPHERE, BOX, CAPSULE }
    private ColliderType type = ColliderType.UNKNOWN;

    // Valores originales
    private float originalFloat1;
    private float originalFloat2;
    private Vector3 originalVector;

    private Coroutine expandRoutine;

    private void OnEnable() => CacheColliderInfo();

    private void CacheColliderInfo()
    {
        switch (targetCollider)
        {
            case SphereCollider sphere:
                type = ColliderType.SPHERE;
                originalFloat1 = sphere.radius;
                break;

            case BoxCollider box:
                type = ColliderType.BOX;
                originalVector = box.size;
                break;

            case CapsuleCollider capsule:
                type = ColliderType.CAPSULE;
                originalFloat1 = capsule.radius;
                originalFloat2 = capsule.height;
                break;

            default:
                type = ColliderType.UNKNOWN;
                Debug.LogWarning($"{name}: Tipo de collider no soportado en ModifyCollider.");
                break;
        }
    }

    public void ExpandCollider()
    {
        switch (type)
        {
            case ColliderType.SPHERE:
                ((SphereCollider)targetCollider).radius = expandValue;
                break;
            case ColliderType.BOX:
                ((BoxCollider)targetCollider).size = Vector3.one * expandValue;
                break;
            case ColliderType.CAPSULE:
                var cap = (CapsuleCollider)targetCollider;
                cap.radius = expandValue;
                cap.height = expandValue * 2f;
                break;
        }
    }

    public void ShrinkCollider()
    {
        if (expandRoutine != null) StopCoroutine(expandRoutine);

        switch (type)
        {
            case ColliderType.SPHERE:
                ((SphereCollider)targetCollider).radius = originalFloat1;
                break;
            case ColliderType.BOX:
                ((BoxCollider)targetCollider).size = originalVector;
                break;
            case ColliderType.CAPSULE:
                CapsuleCollider cap = (CapsuleCollider)targetCollider;
                cap.radius = originalFloat1;
                cap.height = originalFloat2;
                break;
        }
    }

    public void ExpandColliderOverTime()
    {
        if (expandRoutine != null) StopCoroutine(expandRoutine);
        expandRoutine = StartCoroutine(ExpandColliderOverTime(expandDuration));
    }
    private IEnumerator ExpandColliderOverTime(float duration)
    {
        float time = 0f;

        switch (type)
        {
            case ColliderType.SPHERE:
                var sphere = (SphereCollider)targetCollider;
                float startRadius = sphere.radius;
                while (time < duration)
                {
                    float t = time / duration;
                    sphere.radius = Mathf.Lerp(startRadius, expandValue, t);
                    time += Time.deltaTime;
                    yield return null;
                }
                sphere.radius = expandValue;
                break;

            case ColliderType.BOX:
                var box = (BoxCollider)targetCollider;
                Vector3 startSize = box.size;
                Vector3 targetSize = Vector3.one * expandValue;
                while (time < duration)
                {
                    float t = time / duration;
                    box.size = Vector3.Lerp(startSize, targetSize, t);
                    time += Time.deltaTime;
                    yield return null;
                }
                box.size = targetSize;
                break;

            case ColliderType.CAPSULE:
                var capsule = (CapsuleCollider)targetCollider;
                float startRad = capsule.radius;
                float startHeight = capsule.height;
                while (time < duration)
                {
                    float t = time / duration;
                    capsule.radius = Mathf.Lerp(startRad, expandValue, t);
                    capsule.height = Mathf.Lerp(startHeight, expandValue * 2f, t);
                    time += Time.deltaTime;
                    yield return null;
                }
                capsule.radius = expandValue;
                capsule.height = expandValue * 2f;
                break;
        }
    }

}
