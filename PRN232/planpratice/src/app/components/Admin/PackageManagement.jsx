// src/components/Admin/PackageManagement.jsx
import React, { useState, useEffect } from 'react';
import AdminAPI from '../APIService/AdminAPI';
import {
  Package, Plus, Edit2, Trash2, Search, Filter, 
  RefreshCw, ChevronLeft, ChevronRight, DollarSign,
  Clock, Award, CheckCircle, XCircle, AlertCircle,
  Eye, EyeOff, Coins, Users, TrendingUp, Calendar,
  Shield, Star, Zap, Info, ShoppingCart, BarChart3,
  Receipt, Activity
} from 'lucide-react';

const PackageManagement = () => {
  // State Management
  const [packages, setPackages] = useState([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(9);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterActive, setFilterActive] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingPackage, setEditingPackage] = useState(null);
  const [viewingPackage, setViewingPackage] = useState(null);
  const [deleteConfirmation, setDeleteConfirmation] = useState(null);
  const [stats, setStats] = useState({
    totalPackages: 0,
    activePackages: 0,
    totalRevenue: 0,
    totalSales: 0
  });
  const [packageStats, setPackageStats] = useState({});
  const [toast, setToast] = useState(null);

  useEffect(() => {
    fetchPackagesWithStats();
  }, [currentPage, filterActive]);

  // Fetch packages and calculate their statistics from transactions
  const fetchPackagesWithStats = async () => {
    setLoading(true);
    try {
      // Fetch packages
      const packagesResponse = await AdminAPI.getPaginatedPackages(
        currentPage,
        pageSize,
        searchTerm,
        filterActive
      );

      let packagesData = [];
      
      if (packagesResponse?.data?.items) {
        packagesData = packagesResponse.data.items;
        setTotalPages(packagesResponse.data.totalPages || 1);
        setTotalCount(packagesResponse.data.totalCount || 0);
      } else if (packagesResponse?.data && Array.isArray(packagesResponse.data)) {
        packagesData = packagesResponse.data;
        setTotalCount(packagesResponse.data.length);
        setTotalPages(Math.ceil(packagesResponse.data.length / pageSize));
      }

      // Fetch all transactions to calculate package statistics
      try {
        const transactionsResponse = await AdminAPI.getPaginatedPaidTransactions(1, 1000);
        let allTransactions = [];
        
        if (transactionsResponse?.data?.items) {
          allTransactions = transactionsResponse.data.items;
        } else if (Array.isArray(transactionsResponse?.data)) {
          allTransactions = transactionsResponse.data;
        }

        // Calculate statistics for each package
        const statsMap = {};
        allTransactions.forEach(transaction => {
          const packageName = transaction.packageName;
          if (packageName) {
            if (!statsMap[packageName]) {
              statsMap[packageName] = {
                totalSales: 0,
                totalRevenue: 0,
                lastPurchaseDate: null,
                transactions: []
              };
            }
            statsMap[packageName].totalSales += 1;
            statsMap[packageName].totalRevenue += (transaction.amount || 0);
            statsMap[packageName].transactions.push(transaction);
            
            // Update last purchase date
            const transactionDate = new Date(transaction.paidAt || transaction.createdAt);
            if (!statsMap[packageName].lastPurchaseDate || 
                transactionDate > new Date(statsMap[packageName].lastPurchaseDate)) {
              statsMap[packageName].lastPurchaseDate = transaction.paidAt || transaction.createdAt;
            }
          }
        });

        // Merge statistics with package data
        const packagesWithStats = packagesData.map(pkg => {
          const stats = statsMap[pkg.name] || {};
          return {
            ...pkg,
            totalSales: stats.totalSales || 0,
            totalRevenue: stats.totalRevenue || 0,
            lastPurchaseDate: stats.lastPurchaseDate,
            recentTransactions: (stats.transactions || []).slice(0, 5)
          };
        });

        setPackages(packagesWithStats);
        setPackageStats(statsMap);
        
        // Calculate overall stats
        calculateOverallStats(packagesWithStats);
        
      } catch (error) {
        console.error('Failed to fetch transaction statistics:', error);
        // If transactions fail, just use the basic package data
        setPackages(packagesData);
        calculateOverallStats(packagesData);
      }
      
    } catch (error) {
      console.error('Failed to fetch packages:', error);
      showToast('Failed to load packages', 'error');
      setPackages([]);
    } finally {
      setLoading(false);
    }
  };

  // Calculate overall statistics from all packages
  const calculateOverallStats = (packagesData) => {
    if (packagesData && packagesData.length > 0) {
      const active = packagesData.filter(p => p.isActive).length;
      const totalSales = packagesData.reduce((sum, p) => sum + (p.totalSales || 0), 0);
      const totalRevenue = packagesData.reduce((sum, p) => {
        // Use actual totalRevenue if available, otherwise calculate from price
        if (p.totalRevenue) {
          return sum + p.totalRevenue;
        }
        return sum + ((p.price || 0) * (p.totalSales || 0));
      }, 0);
      
      setStats({
        totalPackages: packagesData.length,
        activePackages: active,
        totalRevenue: totalRevenue,
        totalSales: totalSales
      });
    } else {
      setStats({
        totalPackages: 0,
        activePackages: 0,
        totalRevenue: 0,
        totalSales: 0
      });
    }
  };

  // Fetch detailed package information
  const fetchPackageDetail = async (packageId) => {
    try {
      const response = await AdminAPI.getPackageDetail(packageId);
      if (response?.data) {
        // Add the transaction data we already have
        const packageName = response.data.name;
        const stats = packageStats[packageName] || {};
        
        setViewingPackage({
          ...response.data,
          totalSales: stats.totalSales || 0,
          totalRevenue: stats.totalRevenue || 0,
          recentTransactions: stats.transactions || []
        });
      }
    } catch (error) {
      console.error('Failed to fetch package details:', error);
      showToast('Failed to load package details', 'error');
    }
  };

  const handleCreatePackage = async (packageData) => {
    try {
      await AdminAPI.createPackage(packageData);
      showToast('Package created successfully', 'success');
      setShowCreateModal(false);
      fetchPackagesWithStats();
    } catch (error) {
      console.error('Failed to create package:', error);
      showToast('Failed to create package', 'error');
    }
  };

  const handleUpdatePackage = async (packageId, packageData) => {
    try {
      await AdminAPI.updatePackage(packageId, packageData);
      showToast('Package updated successfully', 'success');
      setEditingPackage(null);
      fetchPackagesWithStats();
    } catch (error) {
      console.error('Failed to update package:', error);
      showToast('Failed to update package', 'error');
    }
  };

  const handleDeletePackage = async (packageId) => {
    try {
      await AdminAPI.deletePackage(packageId);
      showToast('Package deleted successfully', 'success');
      setDeleteConfirmation(null);
      fetchPackagesWithStats();
    } catch (error) {
      console.error('Failed to delete package:', error);
      showToast('Failed to delete package', 'error');
    }
  };

  const handleSearch = () => {
    setCurrentPage(1);
    fetchPackagesWithStats();
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

  const getPackageIcon = (packageName) => {
    const name = packageName?.toLowerCase() || '';
    if (name.includes('basic')) return <Star className="h-6 w-6" />;
    if (name.includes('premium')) return <Award className="h-6 w-6" />;
    if (name.includes('enterprise')) return <Shield className="h-6 w-6" />;
    if (name.includes('pro')) return <Zap className="h-6 w-6" />;
    return <Package className="h-6 w-6" />;
  };

  const getPackageColor = (packageName) => {
    const name = packageName?.toLowerCase() || '';
    if (name.includes('basic')) return 'from-blue-400 to-blue-600';
    if (name.includes('premium')) return 'from-purple-400 to-purple-600';
    if (name.includes('enterprise')) return 'from-indigo-400 to-indigo-600';
    if (name.includes('pro')) return 'from-green-400 to-green-600';
    return 'from-gray-400 to-gray-600';
  };

  // Calculate growth percentage
  const calculateGrowth = (current, previous) => {
    if (!previous || previous === 0) return 100;
    return ((current - previous) / previous * 100).toFixed(1);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 p-4 sm:p-6 lg:p-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2 flex items-center gap-3">
          <div className="h-10 w-10 rounded-lg bg-gradient-to-r from-purple-500 to-pink-600 flex items-center justify-center">
            <Package className="h-6 w-6 text-white" />
          </div>
          Package Management
        </h1>
        <p className="text-gray-600 ml-13">Create and manage subscription packages</p>
      </div>

      {/* Enhanced Stats Cards with Real Data */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-blue-400 to-blue-500 rounded-lg shadow-lg">
              <Package className="h-6 w-6 text-white" />
            </div>
            <span className="text-xs text-blue-600 font-semibold flex items-center bg-blue-50 px-2 py-1 rounded-full">
              <Activity className="h-3 w-3 mr-1" />
              Total
            </span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Total Packages</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalPackages}</p>
          <p className="text-xs text-gray-500 mt-2">
            {stats.activePackages} active, {stats.totalPackages - stats.activePackages} inactive
          </p>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-green-400 to-green-500 rounded-lg shadow-lg">
              <CheckCircle className="h-6 w-6 text-white" />
            </div>
            <span className="text-xs text-green-600 font-semibold flex items-center bg-green-50 px-2 py-1 rounded-full">
              <TrendingUp className="h-3 w-3 mr-1" />
              Active
            </span>
          </div>
          <h3 className="text-sm font-medium text-gray-600">Active Packages</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.activePackages}</p>
          <p className="text-xs text-gray-500 mt-2">
            {stats.totalPackages > 0 ? ((stats.activePackages / stats.totalPackages) * 100).toFixed(0) : 0}% of total
          </p>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-purple-400 to-purple-500 rounded-lg shadow-lg">
              <ShoppingCart className="h-6 w-6 text-white" />
            </div>
            {stats.totalSales > 0 && (
              <span className="text-xs text-purple-600 font-semibold flex items-center bg-purple-50 px-2 py-1 rounded-full">
                <TrendingUp className="h-3 w-3 mr-1" />
                Sales
              </span>
            )}
          </div>
          <h3 className="text-sm font-medium text-gray-600">Total Sales</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalSales.toLocaleString()}</p>
          <p className="text-xs text-gray-500 mt-2">
            All-time package purchases
          </p>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
          <div className="flex items-center justify-between mb-4">
            <div className="p-3 bg-gradient-to-r from-yellow-400 to-yellow-500 rounded-lg shadow-lg">
              <DollarSign className="h-6 w-6 text-white" />
            </div>
            {stats.totalRevenue > 0 && (
              <span className="text-xs text-green-600 font-semibold flex items-center bg-green-50 px-2 py-1 rounded-full">
                <TrendingUp className="h-3 w-3 mr-1" />
                Revenue
              </span>
            )}
          </div>
          <h3 className="text-sm font-medium text-gray-600">Total Revenue</h3>
          <p className="text-2xl font-bold text-gray-900 mt-1">{formatCurrency(stats.totalRevenue)}</p>
          <p className="text-xs text-gray-500 mt-2">
            Lifetime revenue from packages
          </p>
        </div>
      </div>

      {/* Filters Section - Same as before */}
      <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-6 mb-6">
        <div className="flex flex-col lg:flex-row gap-4">
          {/* Search */}
          <div className="flex-1">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input
                type="text"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                placeholder="Search packages by name..."
                className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all"
              />
            </div>
          </div>

          {/* Status Filter */}
          <select
            value={filterActive === null ? 'all' : filterActive ? 'active' : 'inactive'}
            onChange={(e) => {
              const value = e.target.value;
              setFilterActive(value === 'all' ? null : value === 'active');
              setCurrentPage(1);
            }}
            className="px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 bg-white"
          >
            <option value="all">All Status</option>
            <option value="active">Active Only</option>
            <option value="inactive">Inactive Only</option>
          </select>

          {/* Actions */}
          <button
            onClick={handleSearch}
            className="flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-purple-500 to-purple-600 text-white rounded-lg hover:from-purple-600 hover:to-purple-700 transition-all shadow-lg"
          >
            <Filter className="h-5 w-5" />
            Apply Filters
          </button>

          <button
            onClick={() => {
              setSearchTerm('');
              setFilterActive(null);
              setCurrentPage(1);
              fetchPackagesWithStats();
            }}
            className="px-4 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
          >
            Clear
          </button>

          <button
            onClick={fetchPackagesWithStats}
            className="flex items-center gap-2 px-4 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
          >
            <RefreshCw className={`h-5 w-5 ${loading ? 'animate-spin' : ''}`} />
            Refresh
          </button>

          <button
            onClick={() => setShowCreateModal(true)}
            className="flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-green-500 to-green-600 text-white rounded-lg hover:from-green-600 hover:to-green-700 transition-all shadow-lg"
          >
            <Plus className="h-5 w-5" />
            Add Package
          </button>
        </div>
      </div>

      {/* Enhanced Packages Grid with Real Sales Data */}
      {loading ? (
        <div className="flex justify-center items-center py-20">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
        </div>
      ) : packages.length === 0 ? (
        <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-20 text-center">
          <Package className="h-16 w-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No packages found</h3>
          <p className="text-gray-600 mb-6">Get started by creating your first package</p>
          <button
            onClick={() => setShowCreateModal(true)}
            className="inline-flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-purple-500 to-purple-600 text-white rounded-lg hover:from-purple-600 hover:to-purple-700 transition-all shadow-lg"
          >
            <Plus className="h-5 w-5" />
            Create First Package
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {packages.map((pkg) => (
            <div
              key={pkg.id}
              className="bg-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 overflow-hidden group"
            >
              {/* Package Header */}
              <div className={`p-6 bg-gradient-to-r ${getPackageColor(pkg.name)} text-white relative overflow-hidden`}>
                <div className="absolute top-0 right-0 -mt-4 -mr-4 h-24 w-24 rounded-full bg-white opacity-10"></div>
                <div className="absolute bottom-0 left-0 -mb-4 -ml-4 h-20 w-20 rounded-full bg-white opacity-10"></div>
                
                <div className="relative z-10">
                  <div className="flex items-start justify-between mb-4">
                    <div className="p-3 bg-white/20 backdrop-blur-sm rounded-lg">
                      {getPackageIcon(pkg.name)}
                    </div>
                    <span className={`px-3 py-1 text-xs font-semibold rounded-full ${
                      pkg.isActive 
                        ? 'bg-green-400/30 text-white border border-green-300/50' 
                        : 'bg-red-400/30 text-white border border-red-300/50'
                    }`}>
                      {pkg.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                  
                  <h3 className="text-xl font-bold mb-2">{pkg.name}</h3>
                  <div className="flex items-baseline gap-1">
                    <span className="text-3xl font-bold">{formatCurrency(pkg.price)}</span>
                    <span className="text-sm opacity-90">/ {pkg.duration} days</span>
                  </div>

                  {pkg.coinAmount > 0 && (
                    <div className="mt-2 flex items-center gap-2">
                      <Coins className="h-4 w-4" />
                      <span className="text-sm">{pkg.coinAmount} coins included</span>
                    </div>
                  )}
                </div>
              </div>

              {/* Package Body with Enhanced Statistics */}
              <div className="p-6">
                {pkg.description && (
                  <p className="text-gray-600 text-sm mb-4 line-clamp-2">{pkg.description}</p>
                )}

                {/* Enhanced Statistics Section - Always Visible with Real Data */}
                <div className="space-y-3 mb-4">
                  {/* Total Sales Card */}
                  <div className="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg p-3 border border-blue-100">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <div className="p-1.5 bg-blue-500 rounded">
                          <ShoppingCart className="h-3.5 w-3.5 text-white" />
                        </div>
                        <span className="text-xs font-medium text-gray-700">Total Sales</span>
                      </div>
                      <div className="text-right">
                        <span className="text-lg font-bold text-blue-600">
                          {pkg.totalSales || 0}
                        </span>
                        <span className="text-xs text-gray-500 block">
                          purchases
                        </span>
                      </div>
                    </div>
                  </div>

                  {/* Total Revenue Card */}
                  <div className="bg-gradient-to-r from-green-50 to-emerald-50 rounded-lg p-3 border border-green-100">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <div className="p-1.5 bg-green-500 rounded">
                          <DollarSign className="h-3.5 w-3.5 text-white" />
                        </div>
                        <span className="text-xs font-medium text-gray-700">Total Revenue</span>
                      </div>
                      <div className="text-right">
                        <span className="text-lg font-bold text-green-600">
                          {formatCurrency(pkg.totalRevenue || 0)}
                        </span>
                        <span className="text-xs text-gray-500 block">
                          lifetime
                        </span>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Performance Indicator */}
                {pkg.totalSales > 0 && (
                  <div className="mb-4">
                    <div className="flex items-center justify-between text-xs text-gray-500 mb-1">
                      <span>Performance Score</span>
                      <span>{Math.min((pkg.totalSales / 10) * 100, 100).toFixed(0)}%</span>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-1.5">
                      <div 
                        className={`h-1.5 rounded-full transition-all duration-500 ${
                          pkg.totalSales >= 10 
                            ? 'bg-gradient-to-r from-green-500 to-emerald-500'
                            : pkg.totalSales >= 5
                            ? 'bg-gradient-to-r from-blue-500 to-indigo-500'
                            : pkg.totalSales >= 2
                            ? 'bg-gradient-to-r from-purple-500 to-pink-500'
                            : 'bg-gradient-to-r from-gray-400 to-gray-500'
                        }`}
                        style={{ width: `${Math.min((pkg.totalSales / 10) * 100, 100)}%` }}
                      ></div>
                    </div>
                  </div>
                )}

                {/* Additional Info */}
                <div className="flex items-center justify-between text-xs text-gray-500 mb-4">
                 
                  {pkg.lastPurchaseDate && (
                    <div className="flex items-center gap-1">
                      <Clock className="h-3.5 w-3.5" />
                      <span>Last: {new Date(pkg.lastPurchaseDate).toLocaleDateString()}</span>
                    </div>
                  )}
                </div>

                {/* Actions */}
                <div className="flex gap-2">
                  <button
                    onClick={() => fetchPackageDetail(pkg.id)}
                    className="flex-1 flex items-center justify-center gap-2 px-3 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    <Eye className="h-4 w-4" />
                    View
                  </button>
                  <button
                    onClick={() => setEditingPackage(pkg)}
                    className="flex-1 flex items-center justify-center gap-2 px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
                  >
                    <Edit2 className="h-4 w-4" />
                    Edit
                  </button>
                  <button
                    onClick={() => setDeleteConfirmation(pkg)}
                    className="p-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination - Same as before */}
      {totalPages > 1 && (
        <div className="mt-8 flex items-center justify-center gap-2">
          <button
            onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
            disabled={currentPage === 1}
            className="p-2 rounded-lg border border-gray-300 bg-white hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
          >
            <ChevronLeft className="h-5 w-5" />
          </button>

          {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
            const pageNumber = currentPage <= 3 
              ? i + 1 
              : currentPage >= totalPages - 2
                ? totalPages - 4 + i
                : currentPage - 2 + i;
            
            if (pageNumber < 1 || pageNumber > totalPages) return null;
            
            return (
              <button
                key={pageNumber}
                onClick={() => setCurrentPage(pageNumber)}
                className={`px-4 py-2 rounded-lg font-medium transition-all ${
                  currentPage === pageNumber
                    ? 'bg-gradient-to-r from-purple-500 to-purple-600 text-white shadow-lg'
                    : 'bg-white border border-gray-300 text-gray-700 hover:bg-gray-100'
                }`}
              >
                {pageNumber}
              </button>
            );
          })}

          <button
            onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
            disabled={currentPage === totalPages}
            className="p-2 rounded-lg border border-gray-300 bg-white hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
          >
            <ChevronRight className="h-5 w-5" />
          </button>
        </div>
      )}

      {/* Modals - Keep same as before */}
      {(showCreateModal || editingPackage) && (
        <PackageModal
          packageData={editingPackage}
          onClose={() => {
            setShowCreateModal(false);
            setEditingPackage(null);
          }}
          onSave={editingPackage 
            ? (data) => handleUpdatePackage(editingPackage.id, data)
            : handleCreatePackage
          }
        />
      )}

      {viewingPackage && (
        <PackageDetailModal
          packageData={viewingPackage}
          onClose={() => setViewingPackage(null)}
        />
      )}

      {deleteConfirmation && (
        <DeleteConfirmationModal
          packageName={deleteConfirmation.name}
          onConfirm={() => handleDeletePackage(deleteConfirmation.id)}
          onCancel={() => setDeleteConfirmation(null)}
        />
      )}

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
            {toast.type === 'info' && <Info className="h-5 w-5" />}
            <span>{toast.message}</span>
          </div>
        </div>
      )}
    </div>
  );
};

// Keep the modal components the same as before
const PackageModal = ({ packageData, onClose, onSave }) => {
  // ... same implementation as before
  const [formData, setFormData] = useState({
    name: packageData?.name || '',
    price: packageData?.price || '',
    duration: packageData?.duration || 30,
    coinAmount: packageData?.coinAmount || 0,
    description: packageData?.description || '',
    isActive: packageData?.isActive !== undefined ? packageData.isActive : true
  });

  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name.trim()) newErrors.name = 'Package name is required';
    if (!formData.price || formData.price <= 0) newErrors.price = 'Valid price is required';
    if (!formData.duration || formData.duration <= 0) newErrors.duration = 'Valid duration is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validateForm()) {
      onSave({
        ...formData,
        price: parseFloat(formData.price),
        duration: parseInt(formData.duration),
        coinAmount: parseInt(formData.coinAmount) || 0
      });
    }
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4">
        <div className="fixed inset-0 bg-black opacity-50" onClick={onClose}></div>
        
        <div className="relative bg-white rounded-2xl max-w-2xl w-full p-6 shadow-2xl">
          <h2 className="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-3">
            <div className="h-10 w-10 rounded-lg bg-gradient-to-r from-purple-500 to-pink-600 flex items-center justify-center">
              <Package className="h-6 w-6 text-white" />
            </div>
            {packageData ? 'Edit Package' : 'Create New Package'}
          </h2>
          
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Package Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData({...formData, name: e.target.value})}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-purple-500 ${
                    errors.name ? 'border-red-500' : 'border-gray-300'
                  }`}
                  placeholder="e.g., Premium Package"
                />
                {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name}</p>}
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Price (VND) <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  step="1000"
                  value={formData.price}
                  onChange={(e) => setFormData({...formData, price: e.target.value})}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-purple-500 ${
                    errors.price ? 'border-red-500' : 'border-gray-300'
                  }`}
                  placeholder="100000"
                />
                {errors.price && <p className="text-red-500 text-xs mt-1">{errors.price}</p>}
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Duration (days) <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  value={formData.duration}
                  onChange={(e) => setFormData({...formData, duration: e.target.value})}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-purple-500 ${
                    errors.duration ? 'border-red-500' : 'border-gray-300'
                  }`}
                  placeholder="30"
                />
                {errors.duration && <p className="text-red-500 text-xs mt-1">{errors.duration}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Coin Amount
                </label>
                <div className="relative">
                  <Coins className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <input
                    type="number"
                    value={formData.coinAmount}
                    onChange={(e) => setFormData({...formData, coinAmount: e.target.value})}
                    className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                    placeholder="100"
                  />
                </div>
              </div>
            </div>

            <div className="mb-6">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Description
              </label>
              <textarea
                value={formData.description}
                onChange={(e) => setFormData({...formData, description: e.target.value})}
                rows="4"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                placeholder="Describe what this package offers..."
              />
            </div>

            <div className="mb-6">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={formData.isActive}
                  onChange={(e) => setFormData({...formData, isActive: e.target.checked})}
                  className="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
                />
                <span className="text-sm font-medium text-gray-700">
                  Package is active
                </span>
              </label>
            </div>
            
            <div className="flex justify-end gap-3">
              <button
                type="button"
                onClick={onClose}
                className="px-5 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                className="px-5 py-2.5 bg-gradient-to-r from-purple-500 to-purple-600 text-white rounded-lg hover:from-purple-600 hover:to-purple-700 transition-all shadow-lg"
              >
                {packageData ? 'Update Package' : 'Create Package'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

// Enhanced Package Detail Modal with Transaction History
const PackageDetailModal = ({ packageData, onClose }) => {
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount || 0);
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4">
        <div className="fixed inset-0 bg-black opacity-50" onClick={onClose}></div>
        
        <div className="relative bg-white rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto p-6 shadow-2xl">
          <h2 className="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-3">
            <div className="h-10 w-10 rounded-lg bg-gradient-to-r from-purple-500 to-pink-600 flex items-center justify-center">
              <Package className="h-6 w-6 text-white" />
            </div>
            Package Details
          </h2>

          <div className="space-y-6">
            {/* Basic Info */}
            <div className="grid grid-cols-2 gap-6">
              <div>
                <p className="text-sm text-gray-600 mb-1">Package Name</p>
                <p className="font-semibold text-gray-900">{packageData.name}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600 mb-1">Status</p>
                <span className={`inline-flex items-center gap-1 px-3 py-1 text-xs font-semibold rounded-full ${
                  packageData.isActive 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-red-100 text-red-800'
                }`}>
                  {packageData.isActive ? 'Active' : 'Inactive'}
                </span>
              </div>
              <div>
                <p className="text-sm text-gray-600 mb-1">Price</p>
                <p className="font-semibold text-gray-900">{formatCurrency(packageData.price)}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600 mb-1">Duration</p>
                <p className="font-semibold text-gray-900">{packageData.duration} days</p>
              </div>
              {packageData.coinAmount > 0 && (
                <div>
                  <p className="text-sm text-gray-600 mb-1">Coin Amount</p>
                  <p className="font-semibold text-gray-900 flex items-center gap-1">
                    <Coins className="h-4 w-4 text-yellow-500" />
                    {packageData.coinAmount} coins
                  </p>
                </div>
              )}
              <div>
                <p className="text-sm text-gray-600 mb-1">Created Date</p>
                <p className="font-semibold text-gray-900">
                  {packageData.createdDate ? new Date(packageData.createdDate).toLocaleDateString() : 'N/A'}
                </p>
              </div>
            </div>

            {/* Description */}
            {packageData.description && (
              <div className="border-t pt-6">
                <h3 className="font-semibold text-gray-900 mb-2">Description</h3>
                <p className="text-gray-600">{packageData.description}</p>
              </div>
            )}

            {/* Statistics */}
            <div className="border-t pt-6">
              <h3 className="font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <BarChart3 className="h-5 w-5 text-purple-600" />
                Statistics
              </h3>
              <div className="grid grid-cols-3 gap-4">
                <div className="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg p-4 border border-blue-100">
                  <p className="text-sm text-gray-600 mb-1">Total Sales</p>
                  <p className="font-bold text-2xl text-blue-600">{packageData.totalSales || 0}</p>
                </div>
                <div className="bg-gradient-to-r from-green-50 to-emerald-50 rounded-lg p-4 border border-green-100">
                  <p className="text-sm text-gray-600 mb-1">Total Revenue</p>
                  <p className="font-bold text-2xl text-green-600">
                    {formatCurrency(packageData.totalRevenue || 0)}
                  </p>
                </div>
                <div className="bg-gradient-to-r from-purple-50 to-pink-50 rounded-lg p-4 border border-purple-100">
                  <p className="text-sm text-gray-600 mb-1">Active Users</p>
                  <p className="font-bold text-2xl text-purple-600">{packageData.activeUsers || 0}</p>
                </div>
              </div>
            </div>

            {/* Recent Transactions */}
            {packageData.recentTransactions && packageData.recentTransactions.length > 0 && (
              <div className="border-t pt-6">
                <h3 className="font-semibold text-gray-900 mb-4 flex items-center gap-2">
                  <Receipt className="h-5 w-5 text-purple-600" />
                  Recent Transactions
                </h3>
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-4 py-2 text-left text-xs font-semibold text-gray-600">Order</th>
                        <th className="px-4 py-2 text-left text-xs font-semibold text-gray-600">Customer</th>
                        <th className="px-4 py-2 text-right text-xs font-semibold text-gray-600">Amount</th>
                        <th className="px-4 py-2 text-left text-xs font-semibold text-gray-600">Date</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                      {packageData.recentTransactions.slice(0, 5).map((transaction, index) => (
                        <tr key={index} className="hover:bg-gray-50">
                          <td className="px-4 py-2 text-sm">#{transaction.orderCode}</td>
                          <td className="px-4 py-2 text-sm">{transaction.userName}</td>
                          <td className="px-4 py-2 text-sm text-right font-semibold">
                            {formatCurrency(transaction.amount)}
                          </td>
                          <td className="px-4 py-2 text-sm">{formatDate(transaction.paidAt)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {/* Close Button */}
            <div className="flex justify-end pt-6 border-t">
              <button
                onClick={onClose}
                className="px-5 py-2.5 bg-gradient-to-r from-purple-500 to-purple-600 text-white rounded-lg hover:from-purple-600 hover:to-purple-700 transition-all shadow-lg"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

// Keep DeleteConfirmationModal the same
const DeleteConfirmationModal = ({ packageName, onConfirm, onCancel }) => {
  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4">
        <div className="fixed inset-0 bg-black opacity-50" onClick={onCancel}></div>
        
        <div className="relative bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl">
          <div className="flex items-center gap-3 mb-4">
            <div className="h-12 w-12 rounded-full bg-red-100 flex items-center justify-center">
              <AlertCircle className="h-6 w-6 text-red-600" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-gray-900">Delete Package</h3>
              <p className="text-sm text-gray-600">This action cannot be undone</p>
            </div>
          </div>

          <p className="text-gray-700 mb-6">
            Are you sure you want to delete <span className="font-semibold">{packageName}</span>?
            This will remove the package and all associated data.
          </p>
          
          <div className="flex justify-end gap-3">
            <button
              onClick={onCancel}
              className="px-5 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
            >
              Cancel
            </button>
            <button
              onClick={onConfirm}
              className="px-5 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
            >
              Delete Package
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PackageManagement;