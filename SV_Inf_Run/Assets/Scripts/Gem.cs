using DG.Tweening;
using UnityEngine;

public class Gem : MonoBehaviour
{

    public Land myLand; // Reference to the Land this gem belongs to
    Vector3 pos = new Vector3(0, 0, 0);
    private void Start()
    {
        pos = UiManager.Instance.GemCanvasPosition;
    }
    private void OnEnable()
    {
        transform.DORotate(new Vector3(0, 360 * 2, 0), 2f, RotateMode.FastBeyond360)
          .SetLoops(-1, LoopType.Incremental)
          .SetEase(Ease.Linear);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {

            myLand.RemoveGem(this.gameObject);
            transform.parent = null;
//            Debug.LogError(pos);
            transform.DOLocalMove(pos, 0.25f).SetEase(Ease.Linear)
            .OnStart(() =>
            {
                transform.DOScale(Vector3.zero, .25f);
            })
            .OnComplete(() =>
            {
                GameManager.Instance.onGemCollect?.Invoke();
                PoolManager.Instance.ReturnGemToPool(this.gameObject);
            });
        }
    }
}
