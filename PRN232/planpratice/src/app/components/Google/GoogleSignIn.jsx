// src/components/Google/GoogleSignIn.jsx
import { useEffect, useRef } from 'react';
import { AuthAPI, GOOGLE_CLIENT_ID } from '../APIService/AuthAPI';

const GoogleSignIn = ({ onSuccess, onError }) => {
  const googleButtonRef = useRef(null);

  useEffect(() => {
    // Load Google Identity Services
    const loadGoogleScript = () => {
      if (window.google) {
        initializeGoogleSignIn();
        return;
      }

      const script = document.createElement('script');
      script.src = 'https://accounts.google.com/gsi/client';
      script.async = true;
      script.defer = true;
      script.onload = initializeGoogleSignIn;
      script.onerror = () => onError('Failed to load Google Sign-In');
      document.head.appendChild(script);
    };

    const initializeGoogleSignIn = () => {
      if (!window.google || !GOOGLE_CLIENT_ID) {
        onError('Google configuration not found');
        return;
      }

      try {
        window.google.accounts.id.initialize({
          client_id: GOOGLE_CLIENT_ID,
          callback: handleGoogleResponse,
          auto_select: false,
          cancel_on_tap_outside: true
        });

        // Render the button
        window.google.accounts.id.renderButton(
          googleButtonRef.current,
          {
            theme: 'outline',
            size: 'large',
            width: '100%',
            text: 'continue_with',
            shape: 'rectangular',
            logo_alignment: 'left'
          }
        );
      } catch (error) {
        console.error('Google Sign-In initialization error:', error);
        onError('Failed to initialize Google Sign-In');
      }
    };

    const handleGoogleResponse = async (response) => {
      try {
        if (!response.credential) {
          throw new Error('No credential received from Google');
        }

        // Call your backend API
        const result = await AuthAPI.googleLogin(response.credential);
        onSuccess(result);
      } catch (error) {
        console.error('Google Sign-In error:', error);
        onError(error.message || 'Google Sign-In failed');
      }
    };

    loadGoogleScript();

    // Cleanup
    return () => {
      const script = document.querySelector('script[src="https://accounts.google.com/gsi/client"]');
      if (script) {
        script.remove();
      }
    };
  }, [onSuccess, onError]);

  return (
    <div style={{ width: '100%' }}>
      <div ref={googleButtonRef} style={{ width: '100%' }}></div>
    </div>
  );
};

export default GoogleSignIn;