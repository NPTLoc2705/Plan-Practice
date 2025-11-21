import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useState, useRef, useEffect } from 'react';
import { AuthAPI } from '../components/APIService/AuthAPI';
import { UserAPI } from '../components/APIService/UserAPI';
import PackageModal from '../pages/PackageModal';

const HeaderBar = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const isAuthenticated = AuthAPI.isAuthenticated();
  const isTeacher = AuthAPI.isTeacher();
  const isStudent = AuthAPI.isStudent();
  const isAdmin = AuthAPI.isAdmin();

  const [showPackages, setShowPackages] = useState(false);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [coinBalance, setCoinBalance] = useState(0);
  const [loadingCoins, setLoadingCoins] = useState(true);
  const dropdownRef = useRef(null);

  const fetchCoinBalance = async () => {
    if (isAuthenticated) {
      try {
        setLoadingCoins(true);
        const balance = await UserAPI.getCoinBalance();
        setCoinBalance(balance);
      } catch (error) {
        console.error('Failed to fetch coin balance:', error);
        setCoinBalance(0);
      } finally {
        setLoadingCoins(false);
      }
    }
  };

  // Fetch coin balance when component mounts (only for authenticated users)
  useEffect(() => {
    fetchCoinBalance();

    // Listen for coin balance update events
    const handleBalanceUpdate = () => {
      fetchCoinBalance();
    };

    window.addEventListener('refreshCoinBalance', handleBalanceUpdate);

    return () => {
      window.removeEventListener('refreshCoinBalance', handleBalanceUpdate);
    };
  }, [isAuthenticated]);

  const handleProfileClick = (e) => {
    e.preventDefault();
    if (!isAuthenticated) {
      navigate('/login');
    } else {
      setIsDropdownOpen((prev) => !prev);
    }
  };

  const handleLogout = () => {
    AuthAPI.logout?.();
    navigate('/login');
  };

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setIsDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Define navigation links based on user role
  const getNavLinks = () => {
    const links = [
      { path: '/', label: 'Home', roles: ['all'] },
    ];

    if (isTeacher) {
      links.push(
        { path: '/teacher', label: 'Dashboard', roles: ['teacher'] },
        { path: '/teacher/Lesson/planner', label: 'Lesson Planner', roles: ['teacher'] },
        { path: '/teacher/Lesson/planner/settings', label: 'Settings', roles: ['teacher'] },
        { path: '/teacher/quiz', label: 'Quiz Management', roles: ['teacher'] },
        { path: '/teacher/otp-manager', label: 'OTP Manager', roles: ['teacher'] },
      );
    }

    if (isStudent) {
      links.push(
        { path: '/student/history', label: 'Quiz History', roles: ['student'] },
        { path: '/student/quiz/otp', label: 'Take Quiz (OTP)', roles: ['student'] },
      );
    }

    if (isAdmin) {
      links.push(
        { path: '/admin/dashboard', label: 'Admin Dashboard', roles: ['admin'] },
      );
    }

    return links;
  };

  const navLinks = getNavLinks();

  const getLinkClasses = (path) =>
    `px-4 py-2 rounded-md font-medium transition-colors duration-200 ${
      location.pathname === path
        ? 'text-blue-600 bg-blue-50'
        : 'text-gray-700 hover:bg-blue-50 hover:text-blue-600'
    }`;

  return (
    <>
      <header className="bg-white shadow-md sticky top-0 z-50 py-4">
        <div className="container mx-auto px-6 flex flex-col md:flex-row items-center justify-between gap-4">
          {/* Logo and tagline */}
          <div className="flex flex-col items-start">
            <h1 className="text-3xl font-extrabold bg-gradient-to-r from-blue-600 to-cyan-500 bg-clip-text text-transparent">
              PlanPractice
            </h1>
            <span className="text-sm text-gray-500 font-medium -mt-1">
              Learn • Teach • Excel
            </span>
          </div>

          {/* Navigation links */}
          <nav className="flex flex-wrap items-center gap-3 md:gap-5">
            {navLinks.map((link) => (
              <Link key={link.path} to={link.path} className={getLinkClasses(link.path)}>
                {link.label}
              </Link>
            ))}
            
            {/* Coin Balance Display - Show for authenticated users */}
            {isAuthenticated && (
              <div className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-yellow-50 to-amber-50 border border-yellow-300 rounded-lg">
                <svg 
                  className="w-5 h-5 text-yellow-600" 
                  fill="currentColor" 
                  viewBox="0 0 20 20"
                >
                  <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clipRule="evenodd" />
                </svg>
                {loadingCoins ? (
                  <span className="text-sm font-semibold text-gray-600">...</span>
                ) : (
                  <span className="text-sm font-semibold text-gray-800">
                    {coinBalance.toLocaleString()} coins
                  </span>
                )}
              </div>
            )}
            
            {/* Packages Button - Only show for teachers */}
            {isTeacher && (
              <button
                onClick={() => setShowPackages(true)}
                className="px-4 py-2 rounded-md font-medium text-gray-700 border border-gray-300 hover:bg-gray-50 transition-colors"
              >
                Packages
              </button>
            )}
            
            {/* Profile/Login button or dropdown */}
            {!isAuthenticated ? (
              <button
                onClick={handleProfileClick}
                className="px-4 py-2 rounded-md font-medium text-blue-600 border border-blue-600 hover:bg-blue-50 transition-all duration-200"
              >
                Login
              </button>
            ) : (
              <div className="relative" ref={dropdownRef}>
                <button
                  onClick={handleProfileClick}
                  className="px-4 py-2 rounded-md font-medium bg-blue-600 text-white hover:bg-blue-700 transition-all duration-200 flex items-center gap-2"
                >
                  <span>Profile</span>
                  <svg
                    className={`w-4 h-4 transform transition-transform duration-200 ${
                      isDropdownOpen ? 'rotate-180' : ''
                    }`}
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                    viewBox="0 0 24 24"
                  >
                    <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>

                {isDropdownOpen && (
                  <div className="absolute right-0 mt-2 w-48 bg-white border border-gray-200 rounded-lg shadow-lg py-2">
                    {/* User role badge */}
                    {isAuthenticated && (
                      <div className="px-4 py-2 border-b border-gray-200">
                        <span className={`inline-block px-2 py-1 text-xs font-semibold rounded ${
                          isTeacher ? 'bg-blue-100 text-blue-800' :
                          isStudent ? 'bg-green-100 text-green-800' :
                          isAdmin ? 'bg-purple-100 text-purple-800' :
                          'bg-gray-100 text-gray-800'
                        }`}>
                          {isTeacher ? '👨‍🏫 Teacher' :
                           isStudent ? '👨‍🎓 Student' :
                           isAdmin ? '👑 Admin' :
                           'User'}
                        </span>
                      </div>
                    )}
                    <button
                      onClick={() => {
                        navigate('/profile');
                        setIsDropdownOpen(false);
                      }}
                      className="block w-full text-left px-4 py-2 text-gray-700 hover:bg-blue-50 hover:text-blue-600"
                    >
                      View Profile
                    </button>
                    <button
                      onClick={handleLogout}
                      className="block w-full text-left px-4 py-2 text-gray-700 hover:bg-red-50 hover:text-red-600"
                    >
                      Logout
                    </button>
                  </div>
                )}
              </div>
            )}
          </nav>
        </div>
      </header>

      {/* Package Modal - Only render for teachers */}
      {isTeacher && (
        <PackageModal 
          isOpen={showPackages} 
          onClose={() => setShowPackages(false)} 
        />
      )}
    </>
  );
};

export default HeaderBar;