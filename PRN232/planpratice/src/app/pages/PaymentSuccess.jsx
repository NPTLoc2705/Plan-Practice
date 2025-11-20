// src/pages/PaymentSuccess.jsx
import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import PaymentAPI from '../components/APIService/PaymentAPI';

export default function PaymentSuccess() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  
  const [paymentStatus, setPaymentStatus] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Extract URL parameters - PayOS returns THEIR orderCode, not ours
  const code = searchParams.get('code');
  const paymentLinkId = searchParams.get('id'); // This is consistent!
  const status = searchParams.get('status');
  const orderCode = searchParams.get('orderCode'); // This is PayOS's orderCode

  useEffect(() => {
    const fetchPaymentStatus = async () => {
      // We need to get the payment by paymentLinkId instead of orderCode
      if (!paymentLinkId && !orderCode) {
        setError('No payment identifier found');
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        
     
        
        // Option 2: Get payment history and find the matching payment
        const history = await PaymentAPI.getPaymentHistory();
        
        // Find the payment that matches this paymentLinkId or is most recent
        const matchingPayment = history.find(p => 
          p.paymentLinkId === paymentLinkId || 
          (p.status === 'PAID' && !p.verified) // Find recently paid but unverified
        ) || history.filter(p => p.status === 'PAID')[0]; // Or get the most recent paid
        
        if (matchingPayment) {
          setPaymentStatus(matchingPayment);
        } else {
          // Fallback: Try with the orderCode from URL (might not work)
          try {
            const result = await PaymentAPI.getPaymentStatus(parseInt(orderCode));
            setPaymentStatus(result);
          } catch {
            setError('Payment not found. Please check your payment history.');
          }
        }
      } catch (err) {
        console.error('Error fetching payment status:', err);
        setError(err.message || 'Failed to verify payment');
      } finally {
        setLoading(false);
      }
    };

    fetchPaymentStatus();
  }, [paymentLinkId, orderCode]);

  const handleContinue = () => {
    navigate('/');
  };

  const handleViewHistory = () => {
      navigate('/profile');
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
        <div className="text-center">
          <div className="h-16 w-16 mx-auto animate-spin rounded-full border-4 border-blue-600 border-t-transparent"></div>
          <p className="mt-4 text-lg text-gray-700 font-medium">Verifying your payment...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-red-50 to-orange-100 p-4">
        <div className="max-w-md w-full bg-white rounded-2xl shadow-2xl p-8 text-center">
          <div className="mx-auto w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mb-6">
            <svg className="w-12 h-12 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>

          <h1 className="text-3xl font-bold text-gray-900 mb-4">Verification Issue</h1>
          <p className="text-gray-600 mb-4">{error}</p>
          <p className="text-sm text-gray-500 mb-8">
            Your payment may have been processed. Please check your payment history or contact support.
          </p>

          <div className="space-y-3">
            <button
              onClick={handleViewHistory}
              className="w-full bg-blue-600 text-white font-semibold py-3 rounded-lg hover:bg-blue-700 transition-colors"
            >
              View Payment History
            </button>
            <button
              onClick={() => navigate('/')}
              className="w-full bg-gray-200 text-gray-700 font-semibold py-3 rounded-lg hover:bg-gray-300 transition-colors"
            >
              Back to Home
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (paymentStatus && paymentStatus.status === 'PAID') {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-green-50 to-emerald-100 p-4">
        <div className="max-w-md w-full bg-white rounded-2xl shadow-2xl p-8 text-center">
          <div className="mx-auto w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mb-6 animate-bounce">
            <svg className="w-12 h-12 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>

          <h1 className="text-3xl font-bold text-gray-900 mb-4">Payment Successful! 🎉</h1>
          <p className="text-gray-600 mb-6">
            Thank you for your purchase. Your coins have been added to your account.
          </p>

          <div className="bg-gray-50 rounded-xl p-6 mb-6 text-left">
            <h3 className="font-semibold text-gray-900 mb-4">Payment Details</h3>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Order Code:</span>
                <span className="font-semibold text-gray-900">{paymentStatus.orderCode}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Status:</span>
                <span className="font-semibold text-green-600">PAID</span>
              </div>
              {paymentStatus.amount && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Amount:</span>
                  <span className="font-semibold text-gray-900">
                    {new Intl.NumberFormat('vi-VN', { 
                      style: 'currency', 
                      currency: 'VND' 
                    }).format(paymentStatus.amount)}
                  </span>
                </div>
              )}
              {paymentStatus.coinAmount && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Coins Received:</span>
                  <span className="font-semibold text-blue-600">
                    💰 {paymentStatus.coinAmount.toLocaleString()}
                  </span>
                </div>
              )}
            </div>
          </div>

          <div className="space-y-3">
            <button
              onClick={handleContinue}
              className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-semibold py-3 rounded-lg hover:from-blue-700 hover:to-indigo-700 transition-all shadow-lg"
            >
              Continue to Dashboard
            </button>
            <button
              onClick={handleViewHistory}
              className="w-full bg-gray-200 text-gray-700 font-semibold py-3 rounded-lg hover:bg-gray-300 transition-colors"
            >
              View Payment History
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-yellow-50 to-amber-100 p-4">
      <div className="max-w-md w-full bg-white rounded-2xl shadow-2xl p-8 text-center">
        <div className="mx-auto w-20 h-20 bg-yellow-100 rounded-full flex items-center justify-center mb-6">
          <svg className="w-12 h-12 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>

        <h1 className="text-3xl font-bold text-gray-900 mb-4">Payment Pending</h1>
        <p className="text-gray-600 mb-8">
          Your payment is being processed. Please check back later.
        </p>

        <button
          onClick={() => navigate('/')}
          className="w-full bg-blue-600 text-white font-semibold py-3 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Back to Home
        </button>
      </div>
    </div>
  );
}