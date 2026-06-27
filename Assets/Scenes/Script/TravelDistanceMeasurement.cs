using UnityEngine;

namespace Kounosuke
{
    public class TravelDistanceMeasurement : MonoBehaviour
    {
        [SerializeField,Header("ˆÚ“®‹——£")] private float Distance_ = 0;
        [SerializeField, Header("Œv‘ª•ûŒü")] private Vector3 Direction_ = Vector3.zero;
        Vector3 StartPosition_ = Vector3.zero;
        void Start()
        {
            StartPosition_ = gameObject.transform.position;
            Direction_ = Direction_.normalized;
        }

        void Update()
        {
            var length = gameObject.transform.position - StartPosition_;
            Distance_ = Vector3.Dot(length, Direction_);
        }
    }
}