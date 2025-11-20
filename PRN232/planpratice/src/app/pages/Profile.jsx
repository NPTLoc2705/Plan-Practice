import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { AuthAPI } from '../components/APIService/AuthAPI';
import { UserAPI } from '../components/APIService/UserAPI';
import PaymentAPI from '../components/APIService/PaymentAPI';

const Profile = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(AuthAPI.getUser());
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    userName: user?.username || '',
    phone: user?.phone || '',
    oldPassword: '',
    newPassword: '',
  });
  const [loading, setLoading] = useState(false);
  const [filterStatus, setFilterStatus] = useState('all');
  const [message, setMessage] = useState('');

  // Payment history state
  const [history, setHistory] = useState([]);
  const [historyLoading, setHistoryLoading] = useState(true);
  const [historyError, setHistoryError] = useState('');

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;

  // Fetch payment history
  useEffect(() => {
    const fetchHistory = async () => {
      try {
        setHistoryLoading(true);
        const data = await PaymentAPI.getPaymentHistory();
        setHistory(data);
      } catch (error) {
        setHistoryError(error.message || 'Failed to load payment history');
      } finally {
        setHistoryLoading(false);
      }
    };
    fetchHistory();
  }, []);

  const handleEditToggle = () => {
    setIsEditing((prev) => !prev);
    if (isEditing) {
      setFormData({
        userName: user?.username || '',
        phone: user?.phone || '',
        oldPassword: '',
        newPassword: '',
      });
      setMessage('');
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      setMessage('');

      const updateData = {};
      if (formData.userName !== user.username) updateData.userName = formData.userName || '';
      if (formData.phone !== user.phone) updateData.phone = formData.phone || '';
      if (formData.newPassword) {
        updateData.oldPassword = formData.oldPassword;
        updateData.newPassword = formData.newPassword;
      }

      if (Object.keys(updateData).length === 0) {
        setIsEditing(false);
        return;
      }

      const result = await UserAPI.updateProfile(updateData);
      setMessage(result.message || 'Profile updated successfully');
      const updatedUser = AuthAPI.getUser();
      setUser(updatedUser);
      setIsEditing(false);
      setFormData((prev) => ({ ...prev, oldPassword: '', newPassword: '' }));
    } catch (error) {
      setMessage(error.message || 'Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return 'Invalid Date';
    }
  };

  const getRoleDisplay = (role) => {
    switch (role) {
      case 1: return 'teacher';
      case 2: return 'admin';
      default: return 'student';
    }
  };

  const getStatusBadge = (status) => {
    const base = 'inline-block px-2 py-1 text-xs font-semibold rounded';
    switch (status?.toLowerCase()) {
      case 'paid': return `${base} bg-green-100 text-green-800`;
      case 'cancelled': return `${base} bg-red-100 text-red-800`;
      default: return `${base} bg-gray-100 text-gray-800`;
    }
  };

  // Filter + sort + paginate
  const filteredHistory = history
    .filter((payment) =>
      filterStatus === 'all' || payment.status?.toLowerCase() === filterStatus
    )
    .sort((a, b) => new Date(b.createdAt || b.paymentDate) - new Date(a.createdAt || a.paymentDate));

  const totalPages = Math.ceil(filteredHistory.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentItems = filteredHistory.slice(startIndex, startIndex + itemsPerPage);

  const handlePrev = () => setCurrentPage((p) => Math.max(p - 1, 1));
  const handleNext = () => setCurrentPage((p) => Math.min(p + 1, totalPages));

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-cyan-50 to-white flex flex-col">
      <main className="flex-1 flex items-center justify-center px-4 py-12">
        <div className="w-full max-w-2xl bg-white rounded-2xl shadow-2xl p-8 relative overflow-hidden">
          <div className="absolute inset-0 opacity-5 pointer-events-none bg-[radial-gradient(circle_at_50%_50%,#3b82f6_0%,transparent_70%)]" />

          <div className="flex items-center justify-between mb-8">
            <h2 className="text-2xl font-bold text-gray-800">User Profile</h2>
            <button
              onClick={handleEditToggle}
              className="px-4 py-2 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition"
            >
              {isEditing ? 'Cancel' : 'Edit Profile'}
            </button>
          </div>

          {message && (
            <div
              className={`mb-6 p-4 rounded-lg ${
                message.includes('success')
                  ? 'bg-green-50 text-green-700'
                  : 'bg-red-50 text-red-700'
              }`}
            >
              {message}
            </div>
          )}

          {/* Profile Info */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
            <div className="bg-blue-50 rounded-lg p-4">
              <span className="font-semibold text-gray-700">Username:</span>{' '}
              {isEditing ? (
                <input
                  type="text"
                  name="userName"
                  value={formData.userName}
                  onChange={handleChange}
                  className="ml-1 border border-gray-300 rounded-md px-2 py-1 text-sm focus:outline-none focus:ring focus:ring-blue-200"
                />
              ) : (
                <span className="ml-1 text-gray-900">{user?.username || 'N/A'}</span>
              )}
            </div>

            <div className="bg-cyan-50 rounded-lg p-4">
              <span className="font-semibold text-gray-700">Email:</span>{' '}
              <span className="ml-1 text-gray-900">{user?.email || 'N/A'}</span>
            </div>

            <div className="bg-green-50 rounded-lg p-4">
              <span className="font-semibold text-gray-700">Role:</span>{' '}
              <span className="ml-1 text-gray-900 capitalize">{getRoleDisplay(user?.role)}</span>
            </div>
          </div>

          {isEditing && (
            <div className="mt-6 flex justify-end">
              <button
                onClick={handleSave}
                disabled={loading}
                className="px-6 py-2 bg-green-600 text-white font-semibold rounded-lg hover:bg-green-700 transition disabled:opacity-50"
              >
                {loading ? 'Saving...' : 'Save Changes'}
              </button>
            </div>
          )}

          {/* Payment History */}
          <div className="mt-12">
            <h3 className="text-xl font-bold text-gray-800 mb-4">Payment History</h3>

            {/* Filter */}
            <div className="flex justify-end mb-4">
              <select
                value={filterStatus}
                onChange={(e) => {
                  setFilterStatus(e.target.value);
                  setCurrentPage(1);
                }}
                className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring focus:ring-blue-200"
              >
                <option value="all">All</option>
                <option value="paid">Paid</option>
                <option value="cancelled">Cancelled</option>
              </select>
            </div>

            {historyLoading ? (
              <div className="text-center py-8">
                <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-blue-600 border-t-transparent"></div>
                <p className="mt-2 text-gray-600">Loading history...</p>
              </div>
            ) : historyError ? (
              <div className="bg-red-50 text-red-700 p-4 rounded-lg">{historyError}</div>
            ) : filteredHistory.length === 0 ? (
              <div className="bg-gray-50 text-gray-600 p-4 rounded-lg">
                No payment history found.
              </div>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full border-collapse">
                    <thead>
                      <tr className="bg-gray-100">
                        <th className="p-3 text-left text-sm font-semibold text-gray-700">Date</th>
                        <th className="p-3 text-left text-sm font-semibold text-gray-700">Amount</th>
                        <th className="p-3 text-left text-sm font-semibold text-gray-700">Status</th>
                        <th className="p-3 text-left text-sm font-semibold text-gray-700">Description</th>
                        <th className="p-3 text-left text-sm font-semibold text-gray-700">Order Code</th>
                      </tr>
                    </thead>
                    <tbody>
                      {currentItems.map((payment, index) => (
                        <tr
                          key={payment.orderCode || index}
                          className={index % 2 === 0 ? 'bg-white' : 'bg-gray-50'}
                        >
                          <td className="p-3 text-sm text-gray-900">
                            {formatDate(payment.createdAt || payment.paymentDate)}
                          </td>
                          <td className="py-2 px-4 text-right">
                            {new Intl.NumberFormat('vi-VN', {
                              style: 'currency',
                              currency: 'VND',
                              maximumFractionDigits: 0
                            }).format(payment.amount || 0)}
                          </td>
                          <td className="p-3">
                            <span className={getStatusBadge(payment.status)}>
                              {payment.status || 'Unknown'}
                            </span>
                          </td>
                          <td className="p-3 text-sm text-gray-900">
                            {payment.description || 'N/A'}
                          </td>
                          <td className="p-3 text-sm text-gray-900">
                            {payment.orderCode || 'N/A'}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Pagination */}
                <div className="flex justify-between items-center mt-4">
                  <button
                    onClick={handlePrev}
                    disabled={currentPage === 1}
                    className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50 hover:bg-gray-300"
                  >
                    Previous
                  </button>
                  <span className="text-sm text-gray-600">
                    Page {currentPage} of {totalPages}
                  </span>
                  <button
                    onClick={handleNext}
                    disabled={currentPage === totalPages}
                    className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50 hover:bg-gray-300"
                  >
                    Next
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default Profile;