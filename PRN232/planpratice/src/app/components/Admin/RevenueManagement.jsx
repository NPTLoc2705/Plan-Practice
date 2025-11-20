// src/components/Admin/RevenueManagement.jsx
import React, { useState, useEffect } from 'react';
import AdminAPI from '../APIService/AdminAPI';
import { Line, Bar, Doughnut } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js';
import {
  Calendar, Filter, RefreshCw, Search, TrendingUp, TrendingDown,
  DollarSign, Package, User, Clock, CheckCircle, XCircle, AlertCircle,
  Download, ChevronLeft, ChevronRight, CreditCard, Eye, FileText,
  BarChart3, Activity, Wallet, Coins, ArrowUpRight, ArrowDownRight,
  CalendarDays, Receipt
} from 'lucide-react';

// Register ChartJS components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
  Filler
);

const RevenueManagement = () => {
  // State Management
  const [activeTab, setActiveTab] = useState('overview');
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState(null);

  // Revenue Data State
  const [revenueData, setRevenueData] = useState({
    today: { revenue: 0, transactions: 0, coins: 0 },
    month: { revenue: 0, transactions: 0, coins: 0 },
    year: { revenue: 0, transactions: 0, coins: 0 },
    last30Days: { revenue: 0, transactions: 0, coins: 0 },
    dailyRevenue: [],
    packageBreakdown: []
  });

  // Transaction State
  const [transactions, setTransactions] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  // Date Range State
  const [dateRange, setDateRange] = useState({
    startDate: '',
    endDate: ''
  });
  const [customRevenue, setCustomRevenue] = useState(null);

  // Tabs Configuration
  const tabs = [
    { id: 'overview', label: 'Revenue Overview', icon: BarChart3 },
    { id: 'transactions', label: 'Transaction History', icon: Receipt },
  ];

  // Lifecycle
  useEffect(() => {
    if (activeTab === 'overview' || activeTab === 'analytics') {
      fetchRevenueData();
    } else if (activeTab === 'transactions') {
      fetchTransactions();
    }
  }, [activeTab, currentPage]);

  // Revenue Data Fetching
  const fetchRevenueData = async () => {
    setLoading(true);
    try {
      const [today, month, year, last30] = await Promise.all([
        AdminAPI.getTodayRevenue(),
        AdminAPI.getMonthRevenue(),
        AdminAPI.getYearRevenue(),
        AdminAPI.getLast30DaysRevenue()
      ]);

      setRevenueData({
        today: {
          revenue: today?.data?.totalRevenue || 0,
          transactions: today?.data?.totalTransactions || 0,
          coins: today?.data?.totalCoins || 0
        },
        month: {
          revenue: month?.data?.totalRevenue || 0,
          transactions: month?.data?.totalTransactions || 0,
          coins: month?.data?.totalCoins || 0
        },
        year: {
          revenue: year?.data?.totalRevenue || 0,
          transactions: year?.data?.totalTransactions || 0,
          coins: year?.data?.totalCoins || 0
        },
        last30Days: {
          revenue: last30?.data?.totalRevenue || 0,
          transactions: last30?.data?.totalTransactions || 0,
          coins: last30?.data?.totalCoins || 0
        },
        dailyRevenue: last30?.data?.dailyRevenue || [],
        packageBreakdown: last30?.data?.revenueByPackage || []
      });
    } catch (error) {
      console.error('Failed to fetch revenue data:', error);
      showToast('Failed to load revenue data', 'error');
    } finally {
      setLoading(false);
    }
  };

  // Transaction Fetching
  const fetchTransactions = async () => {
    setLoading(true);
    try {
      const response = await AdminAPI.getPaginatedPaidTransactions(
        currentPage,
        pageSize,
        dateRange.startDate || null,
        dateRange.endDate || null
      );

      // Handle response structure
      let transactionData = [];
      let pagination = { totalPages: 1, totalCount: 0 };
      
      if (response?.data) {
        if (Array.isArray(response.data)) {
          transactionData = response.data;
        } else if (response.data.items) {
          transactionData = response.data.items;
        }
        
        pagination = {
          totalPages: response.data.totalPages || response.pagination?.totalPages || 1,
          totalCount: response.data.totalCount || response.pagination?.totalCount || transactionData.length
        };
      }
      
      setTransactions(transactionData);
      setTotalPages(pagination.totalPages);
      setTotalCount(pagination.totalCount);
      
    } catch (error) {
      console.error('Failed to fetch transactions:', error);
      showToast('Failed to load transactions', 'error');
      setTransactions([]);
    } finally {
      setLoading(false);
    }
  };

  // Custom Date Range Revenue
  const fetchCustomRangeRevenue = async () => {
    if (!dateRange.startDate || !dateRange.endDate) {
      showToast('Please select both start and end dates', 'warning');
      return;
    }

    setLoading(true);
    try {
      const response = await AdminAPI.getTotalRevenue(dateRange.startDate, dateRange.endDate);
      
      setCustomRevenue({
        total: response?.data?.totalRevenue || 0,
        transactionCount: response?.data?.totalTransactions || 0,
        totalCoins: response?.data?.totalCoins || 0,
        revenueByPackage: response?.data?.revenueByPackage || [],
        dailyRevenue: response?.data?.dailyRevenue || []
      });

      // Also fetch transactions for the custom range
      if (activeTab === 'transactions') {
        fetchTransactions();
      }
      
      showToast('Custom range data loaded', 'success');
    } catch (error) {
      console.error('Failed to fetch custom range revenue:', error);
      showToast('Failed to load custom range data', 'error');
    } finally {
      setLoading(false);
    }
  };

  // Utility Functions
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount || 0);
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      return new Date(dateString).toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch (error) {
      return 'Invalid Date';
    }
  };

  const showToast = (message, type) => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 3000);
  };

  const getStatusIcon = (status) => {
    switch (status?.toLowerCase()) {
      case 'completed':
      case 'paid':
        return <CheckCircle className="h-4 w-4" />;
      case 'pending':
        return <Clock className="h-4 w-4" />;
      case 'failed':
      case 'cancelled':
        return <XCircle className="h-4 w-4" />;
      default:
        return <AlertCircle className="h-4 w-4" />;
    }
  };

  const getStatusColor = (status) => {
    switch (status?.toLowerCase()) {
      case 'completed':
      case 'paid':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'failed':
      case 'cancelled':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  // Filter transactions
  const filteredTransactions = Array.isArray(transactions) ? transactions.filter(transaction => {
    const matchesSearch = searchTerm === '' ||
      transaction.orderCode?.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      transaction.userName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      transaction.userEmail?.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus = statusFilter === 'all' || 
      transaction.status?.toLowerCase() === statusFilter.toLowerCase();

    return matchesSearch && matchesStatus;
  }) : [];

  // Calculate revenue growth
  const calculateGrowth = (current, previous) => {
    if (!previous || previous === 0) return 0;
    return ((current - previous) / previous * 100).toFixed(1);
  };

  // Chart Data
  const revenueChartData = {
    labels: revenueData.dailyRevenue.length > 0
      ? revenueData.dailyRevenue.map(item => 
          new Date(item.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
        )
      : ['No Data'],
    datasets: [{
      label: 'Daily Revenue',
      data: revenueData.dailyRevenue.length > 0
        ? revenueData.dailyRevenue.map(item => item.revenue)
        : [0],
      borderColor: 'rgb(99, 102, 241)',
      backgroundColor: 'rgba(99, 102, 241, 0.1)',
      tension: 0.4,
      fill: true
    }]
  };

  const packageChartData = {
    labels: revenueData.packageBreakdown.map(p => p.packageName || 'Unknown'),
    datasets: [{
      data: revenueData.packageBreakdown.map(p => p.totalRevenue),
      backgroundColor: [
        'rgba(99, 102, 241, 0.8)',
        'rgba(168, 85, 247, 0.8)',
        'rgba(236, 72, 153, 0.8)',
        'rgba(251, 146, 60, 0.8)',
        'rgba(34, 197, 94, 0.8)',
      ],
      borderWidth: 0
    }]
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom',
      },
      tooltip: {
        callbacks: {
          label: function(context) {
            if (context.dataset.label) {
              return context.dataset.label + ': ' + formatCurrency(context.parsed.y);
            }
            return formatCurrency(context.parsed);
          }
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: function(value) {
            return formatCurrency(value);
          }
        }
      }
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 p-4 sm:p-6 lg:p-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2 flex items-center gap-3">
          <div className="h-10 w-10 rounded-lg bg-gradient-to-r from-indigo-500 to-purple-600 flex items-center justify-center">
            <DollarSign className="h-6 w-6 text-white" />
          </div>
          Revenue & Transactions
        </h1>
        <p className="text-gray-600 ml-13">Monitor revenue performance and transaction history</p>
      </div>

      {/* Tabs */}
      <div className="bg-white rounded-t-xl shadow-sm border border-gray-200 border-b-0">
        <div className="flex flex-wrap">
          {tabs.map(tab => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex items-center gap-2 px-6 py-4 font-medium transition-all ${
                activeTab === tab.id
                  ? 'text-indigo-600 border-b-2 border-indigo-600 bg-indigo-50/50'
                  : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
              }`}
            >
              <tab.icon className="h-5 w-5" />
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Date Range Filter (Common for all tabs) */}
      <div className="bg-white shadow-sm border border-gray-200 border-t-0 p-6 rounded-b-xl mb-6">
        <div className="flex flex-col lg:flex-row gap-4">
          <div className="flex-1 flex items-center gap-4">
            <div className="flex items-center gap-2">
              <CalendarDays className="h-5 w-5 text-gray-400" />
              <input
                type="date"
                value={dateRange.startDate}
                onChange={(e) => setDateRange({...dateRange, startDate: e.target.value})}
                className="px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
              />
            </div>
            <span className="text-gray-500">to</span>
            <input
              type="date"
              value={dateRange.endDate}
              onChange={(e) => setDateRange({...dateRange, endDate: e.target.value})}
              className="px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          
          <div className="flex gap-3">
            <button 
              onClick={fetchCustomRangeRevenue}
              className="flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-indigo-500 to-purple-600 text-white rounded-lg hover:from-indigo-600 hover:to-purple-700 transition-all shadow-lg"
            >
              <Filter className="h-5 w-5" />
              Apply Date Range
            </button>
            
            {(dateRange.startDate || dateRange.endDate) && (
              <button 
                onClick={() => {
                  setDateRange({ startDate: '', endDate: '' });
                  setCustomRevenue(null);
                  if (activeTab === 'transactions') fetchTransactions();
                  if (activeTab === 'overview') fetchRevenueData();
                }}
                className="px-4 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
              >
                Clear
              </button>
            )}

            <button
              onClick={() => activeTab === 'transactions' ? fetchTransactions() : fetchRevenueData()}
              className="flex items-center gap-2 px-4 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
            >
              <RefreshCw className={`h-5 w-5 ${loading ? 'animate-spin' : ''}`} />
              Refresh
            </button>
          </div>
        </div>

        {/* Show custom range summary if available */}
        {customRevenue && (
          <div className="mt-4 p-4 bg-gradient-to-r from-indigo-50 to-purple-50 rounded-lg border border-indigo-200">
            <p className="text-sm text-gray-600 mb-2">
              Custom Range: {new Date(dateRange.startDate).toLocaleDateString()} - {new Date(dateRange.endDate).toLocaleDateString()}
            </p>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <p className="text-sm text-gray-600">Total Revenue</p>
                <p className="text-xl font-bold text-indigo-600">{formatCurrency(customRevenue.total)}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600">Transactions</p>
                <p className="text-xl font-bold text-indigo-600">{customRevenue.transactionCount}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600">Coins Sold</p>
                <p className="text-xl font-bold text-indigo-600">{customRevenue.totalCoins?.toLocaleString()}</p>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Content Based on Active Tab */}
      {loading ? (
        <div className="flex justify-center items-center py-20">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
        </div>
      ) : (
        <>
          {/* Revenue Overview Tab */}
          {activeTab === 'overview' && (
            <div className="space-y-6">
              {/* Revenue Cards */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div className="bg-white rounded-xl shadow-lg overflow-hidden">
                  <div className="p-6">
                    <div className="flex items-center justify-between mb-4">
                      <div className="p-3 bg-gradient-to-r from-green-400 to-green-500 rounded-lg">
                        <DollarSign className="h-6 w-6 text-white" />
                      </div>
                      <span className="text-xs text-green-600 font-semibold flex items-center bg-green-50 px-2 py-1 rounded-full">
                        <TrendingUp className="h-3 w-3 mr-1" />
                        +12.5%
                      </span>
                    </div>
                    <h3 className="text-sm font-medium text-gray-600 mb-1">Today's Revenue</h3>
                    <p className="text-2xl font-bold text-gray-900">{formatCurrency(revenueData.today.revenue)}</p>
                    <p className="text-sm text-gray-500 mt-2">{revenueData.today.transactions} transactions</p>
                  </div>
                  <div className="bg-gradient-to-r from-green-400 to-green-500 h-1"></div>
                </div>

                <div className="bg-white rounded-xl shadow-lg overflow-hidden">
                  <div className="p-6">
                    <div className="flex items-center justify-between mb-4">
                      <div className="p-3 bg-gradient-to-r from-blue-400 to-blue-500 rounded-lg">
                        <Wallet className="h-6 w-6 text-white" />
                      </div>
                      <span className="text-xs text-blue-600 font-semibold flex items-center bg-blue-50 px-2 py-1 rounded-full">
                        <TrendingUp className="h-3 w-3 mr-1" />
                        +8.3%
                      </span>
                    </div>
                    <h3 className="text-sm font-medium text-gray-600 mb-1">Monthly Revenue</h3>
                    <p className="text-2xl font-bold text-gray-900">{formatCurrency(revenueData.month.revenue)}</p>
                    <p className="text-sm text-gray-500 mt-2">{revenueData.month.transactions} transactions</p>
                  </div>
                  <div className="bg-gradient-to-r from-blue-400 to-blue-500 h-1"></div>
                </div>

                <div className="bg-white rounded-xl shadow-lg overflow-hidden">
                  <div className="p-6">
                    <div className="flex items-center justify-between mb-4">
                      <div className="p-3 bg-gradient-to-r from-purple-400 to-purple-500 rounded-lg">
                        <BarChart3 className="h-6 w-6 text-white" />
                      </div>
                      <span className="text-xs text-purple-600 font-semibold flex items-center bg-purple-50 px-2 py-1 rounded-full">
                        <TrendingUp className="h-3 w-3 mr-1" />
                        +15.7%
                      </span>
                    </div>
                    <h3 className="text-sm font-medium text-gray-600 mb-1">Yearly Revenue</h3>
                    <p className="text-2xl font-bold text-gray-900">{formatCurrency(revenueData.year.revenue)}</p>
                    <p className="text-sm text-gray-500 mt-2">{revenueData.year.transactions} transactions</p>
                  </div>
                  <div className="bg-gradient-to-r from-purple-400 to-purple-500 h-1"></div>
                </div>

                <div className="bg-white rounded-xl shadow-lg overflow-hidden">
                  <div className="p-6">
                    <div className="flex items-center justify-between mb-4">
                      <div className="p-3 bg-gradient-to-r from-yellow-400 to-orange-400 rounded-lg">
                        <Activity className="h-6 w-6 text-white" />
                      </div>
                      <span className="text-xs text-orange-600 font-semibold flex items-center bg-orange-50 px-2 py-1 rounded-full">
                        <TrendingDown className="h-3 w-3 mr-1" />
                        -2.3%
                      </span>
                    </div>
                    <h3 className="text-sm font-medium text-gray-600 mb-1">Last 30 Days</h3>
                    <p className="text-2xl font-bold text-gray-900">{formatCurrency(revenueData.last30Days.revenue)}</p>
                    <p className="text-sm text-gray-500 mt-2">{revenueData.last30Days.transactions} transactions</p>
                  </div>
                  <div className="bg-gradient-to-r from-yellow-400 to-orange-400 h-1"></div>
                </div>
              </div>

              {/* Charts */}
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <div className="bg-white rounded-xl shadow-lg p-6">
                  <h3 className="text-lg font-semibold text-gray-800 mb-4">Revenue Trend</h3>
                  <div className="h-80">
                    {revenueData.dailyRevenue.length > 0 ? (
                      <Line data={revenueChartData} options={chartOptions} />
                    ) : (
                      <div className="flex items-center justify-center h-full text-gray-500">
                        No data available
                      </div>
                    )}
                  </div>
                </div>

                <div className="bg-white rounded-xl shadow-lg p-6">
                  <h3 className="text-lg font-semibold text-gray-800 mb-4">Revenue Distribution</h3>
                  <div className="h-80">
                    {revenueData.packageBreakdown.length > 0 ? (
                      <Doughnut data={packageChartData} options={{...chartOptions, scales: undefined}} />
                    ) : (
                      <div className="flex items-center justify-center h-full text-gray-500">
                        No data available
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Transactions Tab */}
          {activeTab === 'transactions' && (
            <div className="space-y-6">
              {/* Transaction Filters */}
              <div className="bg-white rounded-xl shadow-lg p-6">
                <div className="flex flex-col lg:flex-row gap-4">
                  <div className="flex-1">
                    <div className="relative">
                      <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                      <input
                        type="text"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        placeholder="Search transactions..."
                        className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
                      />
                    </div>
                  </div>

                  <select
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value)}
                    className="px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 bg-white"
                  >
                    <option value="all">All Status</option>
                    <option value="paid">Paid</option>
                    <option value="pending">Pending</option>
                    <option value="failed">Failed</option>
                    <option value="cancelled">Cancelled</option>
                  </select>

                  <button 
                    onClick={() => console.log('Export')}
                    className="flex items-center gap-2 px-4 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
                  >
                    <Download className="h-5 w-5" />
                    Export
                  </button>
                </div>
              </div>

              {/* Transactions Table */}
              <div className="bg-white rounded-xl shadow-lg overflow-hidden">
                {filteredTransactions.length === 0 ? (
                  <div className="p-20 text-center">
                    <FileText className="h-16 w-16 text-gray-400 mx-auto mb-4" />
                    <h3 className="text-lg font-medium text-gray-900 mb-2">No transactions found</h3>
                    <p className="text-gray-600">Try adjusting your filters or date range</p>
                  </div>
                ) : (
                  <>
                    <div className="overflow-x-auto">
                      <table className="w-full">
                        <thead className="bg-gray-50 border-b border-gray-200">
                          <tr>
                            <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Transaction</th>
                            <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">User</th>
                            <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Package</th>
                            <th className="px-6 py-4 text-right text-xs font-semibold text-gray-600 uppercase">Amount</th>
                            <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Status</th>
                            <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Date</th>
                            <th className="px-6 py-4 text-center text-xs font-semibold text-gray-600 uppercase">Actions</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                          {filteredTransactions.map((transaction, index) => (
                            <tr key={transaction.id || index} className="hover:bg-gray-50 transition-colors">
                              <td className="px-6 py-4">
                                <div className="flex items-center">
                                  <div className="h-10 w-10 rounded-lg bg-gradient-to-r from-indigo-100 to-purple-100 flex items-center justify-center">
                                    <Receipt className="h-5 w-5 text-indigo-600" />
                                  </div>
                                  <div className="ml-3">
                                    <p className="text-sm font-medium text-gray-900">
                                      #{transaction.orderCode || transaction.id || 'N/A'}
                                    </p>
                                    {transaction.id && (
                                      <p className="text-xs text-gray-500">
                                        ID: {transaction.id.slice(0, 8)}...
                                      </p>
                                    )}
                                  </div>
                                </div>
                              </td>
                              <td className="px-6 py-4">
                                <div className="flex items-center">
                                  <div className="h-8 w-8 rounded-full bg-gradient-to-r from-indigo-400 to-purple-400 flex items-center justify-center text-white text-sm font-semibold">
                                    {(transaction.userName || transaction.username || 'U').charAt(0).toUpperCase()}
                                  </div>
                                  <div className="ml-3">
                                    <p className="text-sm font-medium text-gray-900">
                                      {transaction.userName || transaction.username || 'Unknown'}
                                    </p>
                                    <p className="text-xs text-gray-500">
                                      {transaction.userEmail || 'No email'}
                                    </p>
                                  </div>
                                </div>
                              </td>
                              <td className="px-6 py-4">
                                <div className="flex items-center gap-2">
                                  <Package className="h-4 w-4 text-gray-400" />
                                  <span className="text-sm font-medium text-gray-900">
                                    {transaction.packageName || 'N/A'}
                                  </span>
                                </div>
                              </td>
                              <td className="px-6 py-4 text-right">
                                <p className="text-sm font-bold text-gray-900">
                                  {formatCurrency(transaction.amount)}
                                </p>
                              </td>
                              <td className="px-6 py-4">
                                <span className={`px-3 py-1.5 inline-flex items-center gap-1.5 text-xs font-semibold rounded-lg border ${getStatusColor(transaction.status)}`}>
                                  {getStatusIcon(transaction.status)}
                                  {transaction.status || 'Unknown'}
                                </span>
                              </td>
                              <td className="px-6 py-4">
                                <div className="flex items-center gap-1 text-sm text-gray-600">
                                  <Clock className="h-4 w-4 text-gray-400" />
                                  <span>{formatDate(transaction.paidAt || transaction.createdAt)}</span>
                                </div>
                              </td>
                              <td className="px-6 py-4 text-center">
                                <button className="text-indigo-600 hover:text-indigo-800 hover:bg-indigo-50 p-2 rounded-lg transition-all">
                                  <Eye className="h-5 w-5" />
                                </button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>

                    {/* Pagination */}
                    <div className="px-6 py-4 flex items-center justify-between border-t border-gray-200 bg-gray-50">
                      <div className="flex items-center gap-2">
                        <button 
                          onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                          disabled={currentPage === 1}
                          className="p-2 rounded-lg border border-gray-300 bg-white hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                          <ChevronLeft className="h-5 w-5" />
                        </button>
                        
                        {Array.from({ length: Math.min(5, totalPages) }, (_, i) => i + 1).map(pageNumber => (
                          <button
                            key={pageNumber}
                            onClick={() => setCurrentPage(pageNumber)}
                            className={`px-4 py-2 rounded-lg font-medium transition-all ${
                              currentPage === pageNumber
                                ? 'bg-gradient-to-r from-indigo-500 to-purple-600 text-white'
                                : 'bg-white border border-gray-300 text-gray-700 hover:bg-gray-100'
                            }`}
                          >
                            {pageNumber}
                          </button>
                        ))}
                        
                        <button 
                          onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                          disabled={currentPage === totalPages}
                          className="p-2 rounded-lg border border-gray-300 bg-white hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                          <ChevronRight className="h-5 w-5" />
                        </button>
                      </div>
                      
                      <span className="text-sm text-gray-700">
                        Showing <span className="font-medium">{((currentPage - 1) * pageSize) + 1}</span> to{' '}
                        <span className="font-medium">{Math.min(currentPage * pageSize, totalCount)}</span> of{' '}
                        <span className="font-medium">{totalCount}</span> transactions
                      </span>
                    </div>
                  </>
                )}
              </div>
            </div>
          )}

          {/* Analytics Tab */}
          
        </>
      )}

      {/* Toast Notification */}
      {toast && (
        <div className="fixed bottom-4 right-4 z-50">
          <div className={`flex items-center gap-2 px-4 py-3 rounded-lg shadow-lg text-white ${
            toast.type === 'success' ? 'bg-green-500' :
            toast.type === 'error' ? 'bg-red-500' :
            toast.type === 'warning' ? 'bg-yellow-500' :
            'bg-blue-500'
          }`}>
            {toast.type === 'success' && <CheckCircle className="h-5 w-5" />}
            {toast.type === 'error' && <XCircle className="h-5 w-5" />}
            {toast.type === 'warning' && <AlertCircle className="h-5 w-5" />}
            <span>{toast.message}</span>
          </div>
        </div>
      )}
    </div>
  );
};

export default RevenueManagement;