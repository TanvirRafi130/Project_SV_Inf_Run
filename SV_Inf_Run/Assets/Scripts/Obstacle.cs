using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class Obstacle : MonoBehaviour
{
    [SerializeField] ObstacleType obstacleType = ObstacleType.Static;

    [Header("Movement Settings")]
    [SerializeField, ShowIf("obstacleType", ObstacleType.Moving)] float moveTime = 3f;
    [SerializeField, ShowIf("obstacleType", ObstacleType.Moving)] GameObject spike;
    [SerializeField, ShowIf("obstacleType", ObstacleType.Moving)] Transform pointA;
    [SerializeField, ShowIf("obstacleType", ObstacleType.Moving)] Transform pointB;

    Vector3 localA;
    Vector3 localB;
    Sequence seq;
    Tween rotateTween;
    private void Start()
    {
        GameManager.Instance.tweenPause += OnPause;
        GameManager.Instance.tweenResume += OnResume;
        switch (obstacleType)
        {
            case ObstacleType.Moving:
                MovingObstacle();
                break;
            case ObstacleType.Rotating:
                RotatingObstacle();
                break;
            case ObstacleType.Static:
                break;
        }
    }
    private void OnDestroy()
    {
        GameManager.Instance.tweenPause -= OnPause;
        GameManager.Instance.tweenResume -= OnResume;

    }

    public void OnPause()
    {
        rotateTween?.Pause();
        seq?.Pause();
    }
    public void OnResume()
    {
        rotateTween?.Play();
        seq?.Play();
    }



    void MovingObstacle()
    {
        seq = DOTween.Sequence();
        seq.Append(spike.transform.DOLocalMove(pointA.localPosition, moveTime / 2f).SetEase(Ease.InOutSine));
        //seq.Append(spike.transform.DOMove(pointB.position, moveTime / 2f).SetEase(Ease.InOutSine));
        seq.SetLoops(-1, LoopType.Yoyo);
    }

    void RotatingObstacle()
    {
        rotateTween = transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
              .SetLoops(-1, LoopType.Incremental)
              .SetEase(Ease.Linear);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (obstacleType == ObstacleType.Moving)
        {
            if (pointA == null || pointB == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }

    void OnValidate()
    {
        if (obstacleType == ObstacleType.Moving && spike != null && pointA != null)
        {
            // Scene view এ auto reset হবে
            spike.transform.localPosition = pointB.localPosition;
        }
    }


#endif
}


public enum ObstacleType
{
    Static,
    Moving,
    Rotating
}
