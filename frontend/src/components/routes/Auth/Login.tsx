import React, { useRef, useState } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { useAppDispatch } from '../../../redux/hooks';

// redux
import { setUserdata, UserState } from '../../../redux/reducers/userReducer';

// local files
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
  const dispatch = useAppDispatch();
  const location = useLocation();
  const navigate = useNavigate();

  console.log(searchParams);

  return (
    <div className="login-container">
      <form
        method="post"
        className="login-panel"
        // eslint-disable-next-line @typescript-eslint/no-misused-promises
        onSubmit={async (event) => {
          event.preventDefault();

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
            }
          }
        }}
      >
        <div className="input-item">
          <h1>Sign in.</h1>
        </div>
        <div className="input-item">
          {searchParams.has('confirmed') && (
            <div style={{ textAlign: 'center', marginBottom: '0.5rem' }}>
              <span style={{ color: 'lightgreen' }}>
                Your account has been activated successfully, and you may now
                log in!
              </span>
            </div>
          )}
          <span className="subtitle">Email</span>
          <input
            ref={userRef}
            type="email"
            name="email"
            defaultValue={
              searchParams.has('email')
                ? (searchParams.get('email') as string)
                : undefined
            }
          />
        </div>
        <div className="input-item">
          <span className="subtitle">Password</span>
          <input ref={passwordRef} type="password" name="password" />
        </div>
        <div className="input-item">
          <button type="submit">Continue to App</button>
          <div className="register-link">
            {/* eslint-disable-next-line jsx-a11y/anchor-is-valid, jsx-a11y/no-static-element-interactions, jsx-a11y/click-events-have-key-events */}
            <a onClick={() => navigate('/register')}>
              I don&#39;t have an account.
            </a>
          </div>
        </div>
      </form>
    </div>
  );
};
export default Login;