import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useState, useRef, useEffect } from 'react';
import { AuthAPI } from '../components/APIService/AuthAPI';

const HeaderBar = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const isAuthenticated = AuthAPI.isAuthenticated();

  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);

  const handleProfileClick = (e) => {
    e.preventDefault();
    if (!isAuthenticated) {
      navigate('/login');
    } else {
      setIsDropdownOpen((prev) => !prev);
    }
  };

  const handleLogout = () => {
    AuthAPI.logout?.(); // Optional: If your API has a logout method
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

  const navLinks = [
    { path: '/landing', label: 'Home' },
    { path: '/quizmanagement', label: 'Quiz Management' },
    { path: '/teacher', label: 'Teacher Dashboard' },
  ];

  const getLinkClasses = (path) =>
    `px-4 py-2 rounded-md font-medium transition-colors duration-200 ${
      location.pathname === path
        ? 'text-blue-600 bg-blue-50'
        : 'text-gray-700 hover:bg-blue-50 hover:text-blue-600'
    }`;

  return (
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
                <div className="absolute right-0 mt-2 w-40 bg-white border border-gray-200 rounded-lg shadow-lg py-2">
                  <button
                    onClick={() => navigate('/profile')}
                    className="block w-full text-left px-4 py-2 text-gray-700 hover:bg-blue-50 hover:text-blue-600"
                  >
                    View Profile
                  </button>
                  <button
                    onClick={handleLogout}
                    className="block w-full text-left px-4 py-2 text-gray-700 hover:bg-blue-50 hover:text-blue-600"
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
  );
};

export default HeaderBar;
