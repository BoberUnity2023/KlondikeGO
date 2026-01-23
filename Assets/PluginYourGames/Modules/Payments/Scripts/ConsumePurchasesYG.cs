using UnityEngine;

namespace YG
{
    public class ConsumePurchasesYG : MonoBehaviour
    {
        private static bool consumed;

        private void Start()
        {
#if Yandex
            if (!consumed)
            {
                consumed = true;
                YG2.ConsumePurchases();
            }
# endif
        }
    }
}