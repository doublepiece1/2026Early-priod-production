using Newtonsoft.Json.Bson;
using UnityEngine;

namespace Kounosuke
{
    public class ChangeImage : MonoBehaviour
    {
        [SerializeField]private Sprite BeginImage;
        [SerializeField]private Sprite EndImage;

        public Sprite GetBeginImage()
        {
            return BeginImage;
        }
        public Sprite GetEndImage()
        {
            return EndImage;
        }
    }
}