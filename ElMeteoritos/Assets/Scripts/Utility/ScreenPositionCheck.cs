using UnityEngine;
using Photon.Pun;

public class ScreenPositionCheck : MonoBehaviourPun
{
    [Header("Ajustes")]
    [SerializeField][Range(0.01f, 0.1f)] private float marginPercentage;
    public float margin;

    [Header("Flags")]
    [SerializeField] private bool canRelocate;

    private void Update()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient) return;

        Vector3 currentPosition = transform.position;
        Vector3 newPosition;
        bool needsTeleport;

        if (canRelocate)
            needsTeleport = TryGetWrappedPosition(currentPosition, out newPosition);
        else
            needsTeleport = TryGetClampedPosition(currentPosition, out newPosition);

        if (needsTeleport)
            photonView.RPC(nameof(Teleport), RpcTarget.All, newPosition);
    }

    private bool TryGetWrappedPosition(Vector3 position, out Vector3 result)
    {
        result = position;

        if (GameManager.Instance == null) return false;

        Vector3 bottomLeft = GameManager.Instance.cameraBounds.BottomLeft;
        Vector3 topRight = GameManager.Instance.cameraBounds.TopRight;

        //float marginX = (topRight.x - bottomLeft.x) * marginPercentage;
        //float marginY = (topRight.y - bottomLeft.y) * marginPercentage;

        //float spawnMarginX = marginX / 2;
        //float spawnMarginY = marginY / 2;

        float spawnMargin = margin - margin / 2;

        bool changed = false;

        if (position.x > topRight.x + margin)
        {
            result.x = bottomLeft.x - spawnMargin;
            changed = true;
        }
        else if (position.x < bottomLeft.x - margin)
        {
            result.x = topRight.x + spawnMargin;
            changed = true;
        }

        if (position.y > topRight.y + margin)
        {
            result.y = bottomLeft.y - spawnMargin;
            changed = true;
        }
        else if (position.y < bottomLeft.y - margin)
        {
            result.y = topRight.y + spawnMargin;
            changed = true;
        }

        return changed;
    }

    private bool TryGetClampedPosition(Vector3 position, out Vector3 result)
    {
        result = position;

        if (GameManager.Instance == null) return false;

        Vector3 bottomLeft = GameManager.Instance.cameraBounds.BottomLeft;
        Vector3 topRight = GameManager.Instance.cameraBounds.TopRight;

        float marginX = (topRight.x - bottomLeft.x) * marginPercentage;
        float marginY = (topRight.y - bottomLeft.y) * marginPercentage;

        float clampedX = Mathf.Clamp(position.x, bottomLeft.x + marginX, topRight.x - marginX);
        float clampedY = Mathf.Clamp(position.y, bottomLeft.y + marginY, topRight.y - marginY);

        bool changed = !Mathf.Approximately(position.x, clampedX) || !Mathf.Approximately(position.y, clampedY);

        result = new Vector3(clampedX, clampedY, position.z);
        return changed;
    }

    [PunRPC]
    private void Teleport(Vector3 newPosition)
    {
        if (TryGetComponent(out PhotonTransformView view))
            view.enabled = false;

        transform.position = newPosition;

        if (TryGetComponent(out PhotonTransformView viewAfter))
            viewAfter.enabled = true;
    }

    public void SetCanRelocate(bool active, bool sync = true)
    {
        canRelocate = active;
        if (sync && photonView.IsMine)
            photonView.RPC(nameof(SyncCanRelocate), RpcTarget.Others, active);
    }

    [PunRPC]
    private void SyncCanRelocate(bool active) => canRelocate = active;
}


//public class ScreenPositionCheck : MonoBehaviourPun
//{
//    [Header("Ajustes")]
//    [SerializeField][Range(0.01f, 0.1f)] private float marginPercentage = 0.05f;

//    [Header("Flags")]
//    [SerializeField] private bool canRelocate;

//    private void Update()
//    {
//        if (!photonView.IsMine) return;

//        Vector3 newPosition = transform.position;

//        if (canRelocate)
//            newPosition = GetWrappedPosition(newPosition);
//        else
//            newPosition = GetClampedPosition(newPosition);

//        if (newPosition != transform.position)
//            photonView.RPC(nameof(Teleport), RpcTarget.All, newPosition);
//    }

//    private Vector3 GetWrappedPosition(Vector3 position)
//    {
//        if (GameManager.Instance == null) return position;

//        Vector3 bottomLeftBorder = GameManager.Instance.GetCameraBottomLeftBorder();
//        Vector3 topRightBorder = GameManager.Instance.GetCameraTopRightBorder();

//        float horizontalWrapMargin = (topRightBorder.x - bottomLeftBorder.x) * marginPercentage;
//        float verticalWrapMargin = (topRightBorder.y - bottomLeftBorder.y) * marginPercentage;

//        // Manejo del eje X
//        if (position.x > topRightBorder.x + horizontalWrapMargin)
//        {
//            position.x = bottomLeftBorder.x - horizontalWrapMargin;
//        }
//        else if (position.x < bottomLeftBorder.x - horizontalWrapMargin)
//        {
//            position.x = topRightBorder.x + horizontalWrapMargin;
//        }

//        // Manejo del eje Y
//        if (position.y > topRightBorder.y + verticalWrapMargin)
//        {
//            position.y = bottomLeftBorder.y - verticalWrapMargin;
//        }
//        else if (position.y < bottomLeftBorder.y - verticalWrapMargin)
//        {
//            position.y = topRightBorder.y + verticalWrapMargin;
//        }

//        return position;
//    }

//    private Vector3 GetClampedPosition(Vector3 position)
//    {
//        if (GameManager.Instance == null) return position;

//        Vector3 bottomLeftBorder = GameManager.Instance.GetCameraBottomLeftBorder();
//        Vector3 topRightBorder = GameManager.Instance.GetCameraTopRightBorder();

//        float horizontalClampMargin = (topRightBorder.x - bottomLeftBorder.x) * marginPercentage;
//        float verticalClampMargin = (topRightBorder.y - bottomLeftBorder.y) * marginPercentage;

//        position.x = Mathf.Clamp(position.x,
//                               bottomLeftBorder.x + horizontalClampMargin,
//                               topRightBorder.x - horizontalClampMargin);

//        position.y = Mathf.Clamp(position.y,
//                               bottomLeftBorder.y + verticalClampMargin,
//                               topRightBorder.y - verticalClampMargin);

//        return position;
//    }

//    [PunRPC]
//    private void Teleport(Vector3 newPosition)
//    {
//        var photonTransformView = GetComponent<PhotonTransformView>();
//        if (photonTransformView != null)
//            photonTransformView.enabled = false;

//        transform.position = newPosition;

//        if (photonTransformView != null)
//            photonTransformView.enabled = true;
//    }

//    public void SetCanRelocate(bool active, bool sync = true)
//    {
//        canRelocate = active;
//        if (sync && photonView.IsMine)
//        {
//            photonView.RPC(nameof(SyncCanRelocate), RpcTarget.Others, active);
//        }
//    }

//    [PunRPC]
//    private void SyncCanRelocate(bool active) => canRelocate = active;
//}