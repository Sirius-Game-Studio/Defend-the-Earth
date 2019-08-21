#if UNITY_PURCHASING
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.Purchasing
{
    [AddComponentMenu("Unity IAP/IAP Listener")]
    [HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
    public class IAPListener : MonoBehaviour
    {
        [System.Serializable]
        public class OnPurchaseCompletedEvent : UnityEvent<Product>
        {
        };

        [System.Serializable]
        public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason>
        {
        };

        [Tooltip("Consume successful purchases immediately")]
        public bool consumePurchase = true;

        [Tooltip("Preserve this GameObject when a new scene is loaded")]
        public bool dontDestroyOnLoad = true;

        [Tooltip("Event fired after a successful purchase of this product")]
        public OnPurchaseCompletedEvent onPurchaseComplete;

        [Tooltip("Event fired after a failed purchase of this product")]
        public OnPurchaseFailedEvent onPurchaseFailed;
        [SerializeField] private Text purchaseNotification = null;

        void OnEnable()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            CodelessIAPStoreListener.Instance.AddListener(this);
        }

        void OnDisable()
        {
            CodelessIAPStoreListener.Instance.RemoveListener(this);
        }

        /**
         *  Invoked to process a purchase of the product associated with this button
         */
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log(string.Format("IAPListener.ProcessPurchase(PurchaseEventArgs {0} - {1})", e,
                e.purchasedProduct.definition.id));

            onPurchaseComplete.Invoke(e.purchasedProduct);

            return (consumePurchase) ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
        }

        /**
         *  Invoked on a failed purchase of the product associated with this button
         */
        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.Log(string.Format("IAPListener.OnPurchaseFailed(Product {0}, PurchaseFailureReason {1})", product,
                reason));
            if (reason == PurchaseFailureReason.UserCancelled)
            {
                purchaseNotification.text = "Purchase canceled.";
            } else if (reason == PurchaseFailureReason.PaymentDeclined)
            {
                purchaseNotification.text = "There was a problem with the payment.";
            } else if (reason == PurchaseFailureReason.ExistingPurchasePending)
            {
                purchaseNotification.text = "A purchase is already in progress.";
            } else if (reason == PurchaseFailureReason.ProductUnavailable)
            {
                purchaseNotification.text = "This product is unavailable for purchase.";
            } else if (reason == PurchaseFailureReason.PurchasingUnavailable)
            {
                purchaseNotification.text = "Purchasing is currently unavailable.";
            } else if (reason == PurchaseFailureReason.SignatureInvalid)
            {
                purchaseNotification.text = "Signature validation of the purchase's receipt has failed.";
            } else if (reason == PurchaseFailureReason.Unknown)
            {
                purchaseNotification.text = "There was a problem with the purchase due to unknown reasons.";
            }
            onPurchaseFailed.Invoke(product, reason);
        }
    }
}
#endif
