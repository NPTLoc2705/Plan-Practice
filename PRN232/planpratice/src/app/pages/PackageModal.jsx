// src/components/PackageModal.jsx
import { useState, useEffect } from 'react';
import PackageAPI from '../components/APIService/PackageAPI';
import PaymentAPI from '../components/APIService/PaymentAPI';

export default function PackageModal({ isOpen, onClose }) {
  const [packages, setPackages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedPackage, setSelectedPackage] = useState(null);
  const [processingPayment, setProcessingPayment] = useState(false);

  useEffect(() => {
    if (!isOpen) return;

    const fetchPackages = async () => {
      try {
        setLoading(true);
        const data = await PackageAPI.getAll();
        setPackages(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchPackages();
  }, [isOpen]);

  // Reset selected package when modal closes
  useEffect(() => {
    if (!isOpen) {
      setSelectedPackage(null);
    }
  }, [isOpen]);

  const handleSelectPackage = (pkg) => {
    setSelectedPackage(pkg);
  };

  const handleBackToList = () => {
    setSelectedPackage(null);
  };

  const handleCheckout = async () => {
    try {
      setProcessingPayment(true);
      
      // Call your payment API with the selected package ID
      const response = await PaymentAPI.createCoinPayment(
        selectedPackage.id,
        `Purchase ${selectedPackage.name}`
      );
      
      // Redirect to the payment link returned from API
      if (response.checkoutUrl) {
        window.location.href = response.checkoutUrl;
      } else {
        throw new Error('No checkout URL received');
      }
    } catch (error) {
      console.error('Payment error:', error);
      alert(error.message || 'Failed to create payment. Please try again.');
      setProcessingPayment(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
      {/* Clickable backdrop */}
      <div className="absolute inset-0" onClick={onClose} aria-hidden="true" />

      {/* Modal container */}
      <div className="relative w-full max-w-5xl bg-white rounded-2xl shadow-2xl p-6 md:p-8 max-h-[90vh] overflow-y-auto">
        {/* Show Package List or Package Detail */}
        {!selectedPackage ? (
          <>
            {/* Header */}
            <div className="flex items-center justify-between mb-6 border-b pb-3">
              <h2 className="text-3xl font-extrabold text-gray-900">✨ Choose Your Package</h2>
              <button
                onClick={onClose}
                className="p-2 rounded-full hover:bg-gray-100 transition"
                aria-label="Close modal"
              >
                <svg className="h-6 w-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>

            {/* Content States */}
            {loading ? (
              <div className="text-center py-20">
                <div className="h-10 w-10 mx-auto animate-spin rounded-full border-4 border-blue-600 border-t-transparent"></div>
                <p className="mt-4 text-gray-600 font-medium">Loading packages...</p>
              </div>
            ) : error ? (
              <p className="text-center text-red-600 py-8">{error}</p>
            ) : packages.length === 0 ? (
              <p className="text-center text-gray-500 py-8">No packages available at this time.</p>
            ) : (
              <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                {packages.map((pkg) => (
                  <div
                    key={pkg.id}
                    className="group border rounded-xl p-6 transition-all bg-gradient-to-b from-white to-gray-50 hover:shadow-xl hover:-translate-y-1"
                  >
                    {/* Package Header */}
                    <h3 className="font-semibold text-xl text-gray-900 mb-2 group-hover:text-blue-600">
                      {pkg.name}
                    </h3>

                    {/* Price */}
                    <p className="text-lg font-semibold text-gray-700 mb-1">
                      💰 {pkg.coinAmount?.toLocaleString() || 0} Coins
                    </p>
                    <p className="text-3xl font-bold text-blue-600 mb-3">
                      {new Intl.NumberFormat('vi-VN', { 
                        style: 'currency', 
                        currency: 'VND', 
                        maximumFractionDigits: 0 
                      }).format(pkg.price)}
                    </p>

                    {/* Description */}
                    {pkg.description && (
                      <p className="text-sm text-gray-600 mb-4 line-clamp-3">{pkg.description}</p>
                    )}

                    {/* Features */}
                    {pkg.features && (
                      <ul className="space-y-2 mb-6 text-sm text-gray-700">
                        {(Array.isArray(pkg.features)
                          ? pkg.features
                          : pkg.features.split(',')
                        ).map((f, i) => (
                          <li key={i} className="flex items-center gap-2">
                            <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                            </svg>
                            {f.trim()}
                          </li>
                        ))}
                      </ul>
                    )}

                    {/* CTA Button */}
                    <button 
                      onClick={() => handleSelectPackage(pkg)}
                      className="w-full rounded-lg bg-blue-600 py-2.5 text-white font-semibold hover:bg-blue-700 active:scale-[0.98] transition-transform"
                    >
                      Select Package
                    </button>
                  </div>
                ))}
              </div>
            )}
          </>
        ) : (
          <>
            {/* Package Detail View */}
            <div className="flex items-center justify-between mb-6 border-b pb-3">
              <div className="flex items-center gap-3">
                <button
                  onClick={handleBackToList}
                  className="p-2 rounded-full hover:bg-gray-100 transition"
                  aria-label="Back to package list"
                >
                  <svg className="h-6 w-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                  </svg>
                </button>
                <h2 className="text-3xl font-extrabold text-gray-900">Package Details</h2>
              </div>
              <button
                onClick={onClose}
                className="p-2 rounded-full hover:bg-gray-100 transition"
                aria-label="Close modal"
              >
                <svg className="h-6 w-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>

            {/* Package Detail Content */}
            <div className="max-w-2xl mx-auto">
              <div className="bg-gradient-to-br from-blue-50 to-indigo-50 rounded-2xl p-8 mb-6">
                <h3 className="text-2xl font-bold text-gray-900 mb-4">{selectedPackage.name}</h3>
                
                {/* Price Display */}
                <div className="flex items-baseline gap-3 mb-4">
                  <span className="text-5xl font-extrabold text-blue-600">
                    {new Intl.NumberFormat('vi-VN', { 
                      style: 'currency', 
                      currency: 'VND', 
                      maximumFractionDigits: 0 
                    }).format(selectedPackage.price)}
                  </span>
                </div>

                {/* Coin Amount */}
                <div className="inline-flex items-center gap-2 bg-white rounded-full px-4 py-2 mb-4">
                  <span className="text-2xl">💰</span>
                  <span className="font-semibold text-gray-900">
                    {selectedPackage.coinAmount?.toLocaleString() || 0} Coins
                  </span>
                </div>

                {/* Description */}
                {selectedPackage.description && (
                  <p className="text-gray-700 mb-6 leading-relaxed">
                    {selectedPackage.description}
                  </p>
                )}

                {/* Features */}
                {selectedPackage.features && (
                  <div className="bg-white rounded-xl p-6 mb-6">
                    <h4 className="font-semibold text-lg text-gray-900 mb-4">What's Included:</h4>
                    <ul className="space-y-3">
                      {(Array.isArray(selectedPackage.features)
                        ? selectedPackage.features
                        : selectedPackage.features.split(',')
                      ).map((f, i) => (
                        <li key={i} className="flex items-start gap-3">
                          <svg className="w-6 h-6 text-green-500 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                          </svg>
                          <span className="text-gray-700">{f.trim()}</span>
                        </li>
                      ))}
                    </ul>
                  </div>
                )}

                {/* Checkout Button */}
                <button
                  onClick={handleCheckout}
                  disabled={processingPayment}
                  className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-bold py-4 rounded-xl hover:from-blue-700 hover:to-indigo-700 active:scale-[0.98] transition-all shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {processingPayment ? (
                    <span className="flex items-center justify-center gap-2">
                      <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                      Processing...
                    </span>
                  ) : (
                    'Proceed to Checkout 🚀'
                  )}
                </button>

                {/* Payment Info */}
                <p className="text-center text-sm text-gray-600 mt-4">
                  🔒 Secure payment powered by PayOS
                </p>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}