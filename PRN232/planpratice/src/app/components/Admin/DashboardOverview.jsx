// src/components/Admin/DashboardOverview.jsx
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
  Users, GraduationCap, BookOpen, DollarSign, TrendingUp, TrendingDown,
  Activity, Package, Shield, Calendar, Clock, Award, Target,
  ShoppingCart, CreditCard, Wallet, BarChart3, PieChart,
  ArrowUpRight, ArrowDownRight, RefreshCw, Download, Settings,
  UserPlus, Plus, FileText, AlertCircle, CheckCircle, XCircle,
  ChevronRight, Eye, Zap, Sparkles
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

const DashboardOverview = () => {
  // State Management
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalStudents: 0,
    totalTeachers: 0,
    totalAdmins: 0,
    bannedUsers: 0,
    activeUsers: 0,
    todayRevenue: 0,
    todayTransactions: 0,
    monthRevenue: 0,
    monthTransactions: 0,
    yearRevenue: 0,
    yearTransactions: 0,
    last30DaysRevenue: 0,
    totalPackages: 0,
    activePackages: 0,
    newUsersToday: 0,
    growthRate: 0
  });
  const [revenueData, setRevenueData] = useState({
    dailyRevenue: [],
    packageBreakdown: []
  });
  const [recentActivity, setRecentActivity] = useState([]);
  const [toast, setToast] = useState(null);

  // Lifecycle
  useEffect(() => {
    fetchDashboardData();
  }, []);

  // Data Fetching
  const fetchDashboardData = async () => {
    setLoading(true);
    try {
      // Fetch all data in parallel
      const [
        users,
        students,
        teachers,
        todayRev,
        monthRev,
        yearRev,
        last30Rev,
        packages
      ] = await Promise.all([
        AdminAPI.getUsers(),
        AdminAPI.getStudents(),
        AdminAPI.getTeachers(),
        AdminAPI.getTodayRevenue(),
        AdminAPI.getMonthRevenue(),
        AdminAPI.getYearRevenue(),
        AdminAPI.getLast30DaysRevenue(),
        AdminAPI.getAllPackages()
      ]);

      // Calculate stats
      const allUsers = users?.data || [];
      const studentsList = students?.data || [];
      const teachersList = teachers?.data || [];
      const packagesList = packages?.data || [];

      const bannedCount = [...studentsList, ...teachersList].filter(u => u.isBanned).length;
      const activeCount = allUsers.length - bannedCount;
      const adminCount = allUsers.filter(u => u.role === 'Admin').length;

      // Calculate new users today
      const today = new Date().toDateString();
      const newToday = allUsers.filter(u => 
        new Date(u.createdAt).toDateString() === today
      ).length;

      // Calculate growth rate (mock - you'd calculate this from historical data)
      const growthRate = 12.5;

      setStats({
        totalUsers: allUsers.length,
        totalStudents: studentsList.length,
        totalTeachers: teachersList.length,
        totalAdmins: adminCount,
        bannedUsers: bannedCount,
        activeUsers: activeCount,
        todayRevenue: todayRev?.data?.totalRevenue || 0,
        todayTransactions: todayRev?.data?.totalTransactions || 0,
        monthRevenue: monthRev?.data?.totalRevenue || 0,
        monthTransactions: monthRev?.data?.totalTransactions || 0,
        yearRevenue: yearRev?.data?.totalRevenue || 0,
        yearTransactions: yearRev?.data?.totalTransactions || 0,
        last30DaysRevenue: last30Rev?.data?.totalRevenue || 0,
        totalPackages: packagesList.length,
        activePackages: packagesList.filter(p => p.isActive).length,
        newUsersToday: newToday,
        growthRate: growthRate
      });

      setRevenueData({
        dailyRevenue: last30Rev?.data?.dailyRevenue || [],
        packageBreakdown: last30Rev?.data?.revenueByPackage || []
      });

      // Mock recent activity
      setRecentActivity([
        { id: 1, type: 'user', message: 'New student registered', time: '5 min ago', icon: UserPlus, color: 'text-green-600' },
        { id: 2, type: 'payment', message: 'Payment received: 100,000 VND', time: '15 min ago', icon: CreditCard, color: 'text-blue-600' },
        { id: 3, type: 'package', message: 'Premium package purchased', time: '1 hour ago', icon: Package, color: 'text-purple-600' },
        { id: 4, type: 'user', message: 'Teacher account created', time: '2 hours ago', icon: GraduationCap, color: 'text-yellow-600' },
        { id: 5, type: 'system', message: 'System backup completed', time: '3 hours ago', icon: Shield, color: 'text-gray-600' }
      ]);

    } catch (error) {
      console.error('Failed to fetch dashboard data:', error);
      showToast('Failed to load dashboard data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleRefresh = async () => {
    setRefreshing(true);
    await fetchDashboardData();
    setRefreshing(false);
    showToast('Dashboard refreshed', 'success');
  };

  const showToast = (message, type) => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 3000);
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount || 0);
  };

  const formatNumber = (num) => {
    if (num >= 1000000) return `${(num / 1000000).toFixed(1)}M`;
    if (num >= 1000) return `${(num / 1000).toFixed(1)}K`;
    return num.toString();
  };

  // Chart Data
  const revenueChartData = {
    labels: revenueData.dailyRevenue.length > 0 
      ? revenueData.dailyRevenue.slice(-7).map(item => 
          new Date(item.date).toLocaleDateString('en-US', { weekday: 'short' })
        )
      : ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
    datasets: [{
      label: 'Daily Revenue',
      data: revenueData.dailyRevenue.length > 0 
        ? revenueData.dailyRevenue.slice(-7).map(item => item.revenue)
        : [0, 0, 0, 0, 0, 0, 0],
      borderColor: 'rgb(99, 102, 241)',
      backgroundColor: 'rgba(99, 102, 241, 0.1)',
      tension: 0.4,
      fill: true
    }]
  };

  const userDistributionData = {
    labels: ['Students', 'Teachers', 'Admins'],
    datasets: [{
      data: [stats.totalStudents, stats.totalTeachers, stats.totalAdmins],
      backgroundColor: [
        'rgba(59, 130, 246, 0.8)',
        'rgba(168, 85, 247, 0.8)',
        'rgba(251, 146, 60, 0.8)'
      ],
      borderWidth: 0
    }]
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      },
      tooltip: {
        callbacks: {
          label: function(context) {
            return formatCurrency(context.parsed.y || context.parsed);
          }
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        grid: {
          display: false
        },
        ticks: {
          callback: function(value) {
            return formatNumber(value);
          }
        }
      },
      x: {
        grid: {
          display: false
        }
      }
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 p-4 sm:p-6 lg:p-8">
      {/* Header */}
      <div className="mb-8">
        <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2 flex items-center gap-3">
              <div className="h-10 w-10 rounded-lg bg-gradient-to-r from-indigo-500 to-purple-600 flex items-center justify-center">
                <Activity className="h-6 w-6 text-white" />
              </div>
              Dashboard Overview
            </h1>
            <p className="text-gray-600 ml-13">
              Welcome back! Here's what's happening with your platform today.
            </p>
          </div>
          <div className="flex gap-3">
            <button
              onClick={handleRefresh}
              className="flex items-center gap-2 px-4 py-2.5 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-all"
              disabled={refreshing}
            >
              <RefreshCw className={`h-4 w-4 ${refreshing ? 'animate-spin' : ''}`} />
              Refresh
            </button>
           
          </div>
        </div>
      </div>

      {/* Welcome Banner */}
     <div className="bg-gradient-to-r from-green-500 to-cyan-600 rounded-2xl p-8 text-white mb-8 relative overflow-hidden">
        <div className="absolute top-0 right-0 -mt-10 -mr-10 h-40 w-40 rounded-full bg-white opacity-10"></div>
        <div className="absolute bottom-0 left-0 -mb-10 -ml-10 h-32 w-32 rounded-full bg-white opacity-10"></div>
        <div className="relative z-10">
          <div className="flex items-center gap-2 mb-2">
            <Sparkles className="h-5 w-5" />
            <span className="text-sm font-medium">Today's Highlights</span>
          </div>
          <h2 className="text-3xl font-bold mb-4">
            Your platform is growing!
          </h2>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
            <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4">
              <p className="text-white/80 text-sm mb-1">New Users Today</p>
              <p className="text-2xl font-bold">+{stats.newUsersToday}</p>
            </div>
            <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4">
              <p className="text-white/80 text-sm mb-1">Today's Revenue</p>
              <p className="text-2xl font-bold">{formatCurrency(stats.todayRevenue)}</p>
            </div>
            <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4">
              <p className="text-white/80 text-sm mb-1">Growth Rate</p>
              <p className="text-2xl font-bold flex items-center gap-2">
                <TrendingUp className="h-5 w-5" />
                {stats.growthRate}%
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Main Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {/* Users Card */}
        <div className="bg-white rounded-xl shadow-sm hover:shadow-lg transition-all p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-blue-400 to-blue-500 rounded-lg">
              <Users className="h-6 w-6 text-white" />
            </div>
            <span className="text-xs text-green-600 font-semibold flex items-center bg-green-50 px-2 py-1 rounded-full">
              <ArrowUpRight className="h-3 w-3 mr-1" />
              {stats.growthRate}%
            </span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Total Users</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalUsers}</p>
          <div className="mt-4 flex items-center text-sm">
            <span className="text-green-600 font-medium">{stats.activeUsers} active</span>
            <span className="mx-2 text-gray-400">•</span>
            <span className="text-red-600 font-medium">{stats.bannedUsers} banned</span>
          </div>
        </div>

        {/* Students Card */}
        <div className="bg-white rounded-xl shadow-sm hover:shadow-lg transition-all p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-green-400 to-green-500 rounded-lg">
              <GraduationCap className="h-6 w-6 text-white" />
            </div>
            <span className="text-xs text-green-600 font-semibold flex items-center bg-green-50 px-2 py-1 rounded-full">
              <ArrowUpRight className="h-3 w-3 mr-1" />
              8.2%
            </span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Students</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalStudents}</p>
          <div className="mt-4">
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div 
                className="bg-gradient-to-r from-green-400 to-green-500 h-2 rounded-full"
                style={{ width: `${(stats.totalStudents / stats.totalUsers) * 100}%` }}
              ></div>
            </div>
          </div>
        </div>

        {/* Teachers Card */}
        <div className="bg-white rounded-xl shadow-sm hover:shadow-lg transition-all p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-purple-400 to-purple-500 rounded-lg">
              <BookOpen className="h-6 w-6 text-white" />
            </div>
            <span className="text-xs text-purple-600 font-semibold flex items-center bg-purple-50 px-2 py-1 rounded-full">
              <ArrowUpRight className="h-3 w-3 mr-1" />
              5.4%
            </span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Teachers</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalTeachers}</p>
          <div className="mt-4">
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div 
                className="bg-gradient-to-r from-purple-400 to-purple-500 h-2 rounded-full"
                style={{ width: `${(stats.totalTeachers / stats.totalUsers) * 100}%` }}
              ></div>
            </div>
          </div>
        </div>

        {/* Packages Card */}
        <div className="bg-white rounded-xl shadow-sm hover:shadow-lg transition-all p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-yellow-400 to-orange-400 rounded-lg">
              <Package className="h-6 w-6 text-white" />
            </div>
            <Eye className="h-4 w-4 text-gray-400" />
          </div>
          <h3 className="text-sm font-medium text-gray-600">Packages</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalPackages}</p>
          <div className="mt-4 flex items-center text-sm">
            <span className="text-green-600 font-medium">{stats.activePackages} active</span>
          </div>
        </div>
      </div>

      {/* Revenue Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-gradient-to-br from-green-50 to-emerald-100 rounded-xl p-6 border border-green-200">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-white rounded-lg shadow-sm">
              <DollarSign className="h-6 w-6 text-green-600" />
            </div>
            <span className="text-xs text-green-600 font-semibold">Today</span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Daily Revenue</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{formatCurrency(stats.todayRevenue)}</p>
          <p className="text-sm text-gray-600 mt-2">{stats.todayTransactions} transactions</p>
        </div>

        <div className="bg-gradient-to-br from-blue-50 to-indigo-100 rounded-xl p-6 border border-blue-200">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-white rounded-lg shadow-sm">
              <Wallet className="h-6 w-6 text-blue-600" />
            </div>
            <span className="text-xs text-blue-600 font-semibold">This Month</span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Monthly Revenue</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{formatCurrency(stats.monthRevenue)}</p>
          <p className="text-sm text-gray-600 mt-2">{stats.monthTransactions} transactions</p>
        </div>

        <div className="bg-gradient-to-br from-purple-50 to-pink-100 rounded-xl p-6 border border-purple-200">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-white rounded-lg shadow-sm">
              <Target className="h-6 w-6 text-purple-600" />
            </div>
            <span className="text-xs text-purple-600 font-semibold">This Year</span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Yearly Revenue</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{formatCurrency(stats.yearRevenue)}</p>
          <p className="text-sm text-gray-600 mt-2">{stats.yearTransactions} transactions</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        {/* Revenue Chart */}
        <div className="lg:col-span-2 bg-white rounded-xl shadow-sm p-6">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-gray-800">Revenue Trend</h3>
            <div className="flex items-center gap-2">
              <span className="text-xs text-gray-500">Last 7 days</span>
            </div>
          </div>
          <div className="h-64">
            <Line data={revenueChartData} options={chartOptions} />
          </div>
        </div>

        {/* User Distribution */}
        <div className="bg-white rounded-xl shadow-sm p-6">
          <h3 className="text-lg font-semibold text-gray-800 mb-4">User Distribution</h3>
          <div className="h-48 mb-4">
            <Doughnut 
              data={userDistributionData} 
              options={{
                ...chartOptions,
                plugins: {
                  legend: {
                    position: 'bottom'
                  }
                }
              }} 
            />
          </div>
        </div>
      </div>

      {/* Quick Actions & Recent Activity */}
      <div className="grid grid-cols-1 lg:grid-cols-1 gap-12">
        {/* Quick Actions */}
       

        {/* Recent Activity */}
        <div className="bg-white rounded-xl shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-800 mb-4">Recent Activity</h2>
          <div className="space-y-3">
            {recentActivity.map(activity => {
              const Icon = activity.icon;
              return (
                <div key={activity.id} className="flex items-start gap-3 p-3 hover:bg-gray-50 rounded-lg transition-colors">
                  <div className={`p-2 rounded-lg bg-gray-100`}>
                    <Icon className={`h-4 w-4 ${activity.color}`} />
                  </div>
                  <div className="flex-1">
                    <p className="text-sm text-gray-900">{activity.message}</p>
                    <p className="text-xs text-gray-500 mt-1">{activity.time}</p>
                  </div>
                  <ChevronRight className="h-4 w-4 text-gray-400" />
                </div>
              );
            })}
          </div>
        </div>
      </div>

      {/* Toast Notification */}
      {toast && (
        <div className="fixed bottom-4 right-4 z-50">
          <div className={`flex items-center gap-2 px-4 py-3 rounded-lg shadow-lg text-white ${
            toast.type === 'success' ? 'bg-green-500' :
            toast.type === 'error' ? 'bg-red-500' :
            'bg-blue-500'
          }`}>
            {toast.type === 'success' && <CheckCircle className="h-5 w-5" />}
            {toast.type === 'error' && <XCircle className="h-5 w-5" />}
            {toast.type === 'info' && <AlertCircle className="h-5 w-5" />}
            <span>{toast.message}</span>
          </div>
        </div>
      )}
    </div>
  );
};

export default DashboardOverview;