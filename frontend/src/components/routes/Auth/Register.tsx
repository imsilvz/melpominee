import React, { useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';

import './Register.scss';

interface RegisterPayload {
  email: string;
  password: string;
}

interface RegisterResponse {
  success: boolean;
}

const Register = () => {
  const userRef = useRef<HTMLInputElement>(null);
  const passwordRef = useRef<HTMLInputElement>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [registered, setRegistered] = useState<string>('');
  const navigate = useNavigate();

  return (
    <div className="login-container">
      {registered === '' ? (
        <form
          method="post"
          className="login-panel"
          // eslint-disable-next-line @typescript-eslint/no-misused-promises
          onSubmit={async (event) => {
            event.preventDefault();
            setLoading(true);

            const registerPayload: RegisterPayload = {
              email: userRef.current?.value as string,
              password: passwordRef.current?.value as string,
            };
            const registerRequest = await fetch(`/api/auth/register/`, {
              method: 'POST',
              headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
              },
              body: JSON.stringify(registerPayload),
            });

            if (registerRequest.ok) {
              const registerJson: RegisterResponse =
                await (registerRequest.json() as Promise<RegisterResponse>);
              setLoading(false);
              if (registerJson.success) {
                // we have registered successfully!
                setRegistered(registerPayload.email);
              }
            }
          }}
        >
          <div className="input-item">
            <h1>Register Account</h1>
          </div>
          <div className="input-item">
            <span className="subtitle">Email</span>
            <input ref={userRef} type="email" name="email" disabled={loading} />
          </div>
          <div className="input-item">
            <span className="subtitle">Password</span>
            <input
              ref={passwordRef}
              type="password"
              name="password"
              disabled={loading}
            />
          </div>
          <div className="input-item">
            <button type="submit" disabled={loading}>
              Register
            </button>
          </div>
          {loading && (
            <div
              style={{
                position: 'absolute',
                left: 0,
                top: 0,
                width: '100%',
                height: '100%',
                borderRadius: '0.5rem',
                backgroundColor: 'rgba(0, 0, 0, 0.5)',
              }}
            >
              <LoadingSpinner />
            </div>
          )}
        </form>
      ) : (
        <div className="login-panel">
          <div className="input-item">
            <h1>Welcome to Melpominee!</h1>
          </div>
          <div className="input-item" style={{ textAlign: 'center' }}>
            <span className="subtitle">
              Your account, {registered}, has been created.
            </span>
          </div>
          <div className="input-item" style={{ textAlign: 'center' }}>
            <span className="subtitle">
              Before you can log in, you must activate your account. An email
              has been sent to you containing a link that will perform the
              activation. Once this process has been completed, you will be able
              to login.
            </span>
          </div>
          <div className="input-item">
            <button
              type="submit"
              onClick={() => navigate('/login', { replace: true })}
            >
              Return to Login Screen
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
export default Register;