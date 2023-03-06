import React, { useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { useAppDispatch } from '../../../redux/hooks';

// redux
import { setUserdata, UserState } from '../../../redux/reducers/userReducer';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import './Login.scss';

interface LoginPayload {
  email: string;
  password: string;
}

interface LoginResponse {
  success: boolean;
  user?: UserState;
}

const Login = () => {
  const userRef = useRef<HTMLInputElement>(null);
  const passwordRef = useRef<HTMLInputElement>(null);
  const [searchParams, setSearchParams] = useSearchParams();
  const [loginNotice, setLoginNotice] = useState<string>('');
  const [loginError, setLoginError] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);
  const dispatch = useAppDispatch();
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const messageType = searchParams.get('notice');
    if (messageType === 'confirmed') {
      setLoginNotice(
        'Your account has been activated successfully and you may now log in!'
      );
    } else {
      setLoginNotice('');
    }
  }, [searchParams]);

  return (
    <div className="login-container">
      <form
        method="post"
        className="login-panel"
        // eslint-disable-next-line @typescript-eslint/no-misused-promises
        onSubmit={async (event) => {
          event.preventDefault();
          setLoading(true);

          const loginPayload: LoginPayload = {
            email: userRef.current?.value as string,
            password: passwordRef.current?.value as string,
          };
          const loginRequest = await fetch(`/api/auth/login/`, {
            method: 'POST',
            headers: {
              Accept: 'application/json',
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginPayload),
          });

          setLoading(false);
          if (loginRequest.ok) {
            const loginJson: LoginResponse =
              await (loginRequest.json() as Promise<LoginResponse>);
            if (loginJson.success) {
              const userData = loginJson.user as UserState;
              dispatch(setUserdata(userData));
              if (userData.email && userData.email !== '') {
                // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
                const from = (location.state?.from?.pathname || '/') as string;
                navigate(from, { replace: true });
              }
            } else {
              searchParams.delete('message');
              setSearchParams(searchParams);
              setLoginError(
                'Unable to login, please check your email and password!'
              );
            }
          }
        }}
      >
        <div className="input-item">
          <h1>Sign in.</h1>
        </div>
        <div className="input-item">
          {loginNotice !== '' && (
            <div style={{ textAlign: 'center', marginBottom: '0.5rem' }}>
              <span style={{ color: 'lightgreen' }}>{loginNotice}</span>
            </div>
          )}
          {loginError !== '' && (
            <div style={{ textAlign: 'center', marginBottom: '0.5rem' }}>
              <span style={{ color: '#CF6679' }}>{loginError}</span>
            </div>
          )}
          <span className="subtitle">Email</span>
          <input
            ref={userRef}
            type="email"
            name="email"
            disabled={loading}
            defaultValue={
              searchParams.has('email')
                ? (searchParams.get('email') as string)
                : undefined
            }
            onBlur={() => setLoginError('')}
          />
        </div>
        <div className="input-item">
          <span className="subtitle">Password</span>
          <input
            ref={passwordRef}
            type="password"
            name="password"
            disabled={loading}
            onBlur={() => setLoginError('')}
          />
          <div className="forgot-link">
            {/* eslint-disable-next-line jsx-a11y/anchor-is-valid, jsx-a11y/no-static-element-interactions, jsx-a11y/click-events-have-key-events */}
            <a onClick={() => navigate('/forgot-password', { replace: true })}>
              I forgot my password.
            </a>
          </div>
        </div>
        <div className="input-item">
          <button type="submit" disabled={loading}>
            Continue to App
          </button>
          <div className="register-link">
            {/* eslint-disable-next-line jsx-a11y/anchor-is-valid, jsx-a11y/no-static-element-interactions, jsx-a11y/click-events-have-key-events */}
            <a onClick={() => navigate('/register', { replace: true })}>
              I don&#39;t have an account.
            </a>
          </div>
        </div>
        {loading && (
          <div className="loading-overlay">
            <LoadingSpinner />
          </div>
        )}
      </form>
    </div>
  );
};
export default Login;
