// src/components/GoogleSignIn.jsx
import { useEffect } from 'react';
import { Eye, EyeOff, Mail, Lock, User, LogIn, UserPlus, Loader2, AlertCircle, CheckCircle, ArrowLeft } from 'lucide-react';
import { GOOGLE_CLIENT_ID } from '../APIService/AuthAPI';

const GoogleSignIn = ({ onSuccess, onError }) => {
  useEffect(() => {
    if (GOOGLE_CLIENT_ID === 'YOUR_GOOGLE_CLIENT_ID') return;

    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    document.body.appendChild(script);

    script.onload = () => {
      if (window.google) {
        window.google.accounts.id.initialize({
          client_id: GOOGLE_CLIENT_ID,
          callback: handleCredentialResponse,
        });
        
        window.google.accounts.id.renderButton(
          document.getElementById('googleSignInDiv'),
          { 
            theme: 'filled_blue',
            size: 'large',
            width: '100%',
            text: 'continue_with'
          }
        );
      }
    };

    return () => {
      if (document.body.contains(script)) {
        document.body.removeChild(script);
      }
    };
  }, []);

  const handleCredentialResponse = async (response) => {
    try {
      const result = await AuthAPI.googleLogin(response.credential);
      onSuccess(result);
    } catch (error) {
      onError(error.message);
    }
  };

  if (GOOGLE_CLIENT_ID === 'YOUR_GOOGLE_CLIENT_ID') {
    return (
      <div className="p-3 bg-gray-100 rounded-lg text-center text-sm text-gray-600">
        Google Sign-In requires configuration
      </div>
    );
  }

  return <div id="googleSignInDiv" className="w-full"></div>;
};

export default GoogleSignIn;