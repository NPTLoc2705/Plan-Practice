// src/pages/PaymentCancel.jsx
import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import PaymentAPI from '../components/APIService/PaymentAPI';

export default function PaymentCancel() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [syncing, setSyncing] = useState(true);
  const [syncError, setSyncError] = useState(null);

  // Extract URL parameters
  const orderCode = searchParams.get('orderCode');
  const code = searchParams.get('code');
  const paymentLinkId = searchParams.get('id'); // PayOS payment link ID

  useEffect(() => {
    const syncCancelledPayment = async () => {
      console.log('Payment cancelled:', { orderCode, code, paymentLinkId });

      // Try to sync the payment status from PayOS
      if (paymentLinkId || orderCode) {
        try {
          setSyncing(true);
          
          // Sync all pending payments to update their statuses
          await PaymentAPI.syncPendingPayments();
          
          // Optionally, you can also try to get the specific payment status
          if (orderCode) {
            try {
              await PaymentAPI.getPaymentStatus(parseInt(orderCode));
            } catch (err) {
              console.log('Could not get specific payment status:', err);
              // This is OK - the payment might not exist with this orderCode
            }
          }
        } catch (err) {
          console.error('Error syncing cancelled payment:', err);
          setSyncError(err.message);
        } finally {
          setSyncing(false);
        }
      } else {
        setSyncing(false);
      }
    };

    syncCancelledPayment();
  }, [orderCode, code, paymentLinkId]);

  const handleTryAgain = () => {
    navigate('/'); // Go back to home where they can access packages
  };

  const handleGoHome = () => {
    navigate('/');
  };

  if (syncing) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-red-50 to-orange-100 p-4">
        <div className="text-center">
          <div className="h-16 w-16 mx-auto animate-spin rounded-full border-4 border-red-600 border-t-transparent"></div>
          <p className="mt-4 text-lg text-gray-700 font-medium">Updating payment status...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-red-50 to-orange-100 p-4">
      <div className="max-w-md w-full bg-white rounded-2xl shadow-2xl p-8 text-center">
        {/* Cancel Icon */}
        <div className="mx-auto w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mb-6">
          <svg 
            className="w-12 h-12 text-red-600" 
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path 
              strokeLinecap="round" 
              strokeLinejoin="round" 
              strokeWidth={2} 
              d="M6 18L18 6M6 6l12 12" 
            />
          </svg>
        </div>

        <h1 className="text-3xl font-bold text-gray-900 mb-4">Payment Cancelled</h1>
        
        <p className="text-gray-600 mb-2">
          Your payment was cancelled and no charges were made.
        </p>
        
        {orderCode && (
          <p className="text-sm text-gray-500 mb-4">
            Reference: <span className="font-mono font-semibold">{orderCode}</span>
          </p>
        )}

        {syncError && (
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3 mb-4">
            <p className="text-sm text-yellow-800">
              Note: Payment status update encountered an issue, but no charges were made.
            </p>
          </div>
        )}

        {/* Info Box */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-8 text-left">
          <h3 className="font-semibold text-blue-900 mb-2 flex items-center gap-2">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            What happened?
          </h3>
          <p className="text-sm text-blue-800">
            The payment process was cancelled before completion. You can try again anytime, and your payment information is secure.
          </p>
        </div>

        {/* Action Buttons */}
        <div className="space-y-3">
          <button
            onClick={handleTryAgain}
            className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-semibold py-3 rounded-lg hover:from-blue-700 hover:to-indigo-700 transition-all shadow-lg"
          >
            Try Again
          </button>
          <button
            onClick={handleGoHome}
            className="w-full bg-gray-200 text-gray-700 font-semibold py-3 rounded-lg hover:bg-gray-300 transition-colors"
          >
            Back to Home
          </button>
        </div>

        {/* Help Text */}
        <p className="text-xs text-gray-500 mt-6">
          Need help? Contact our support team for assistance.
        </p>
      </div>
    </div>
  );
}