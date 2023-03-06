import React, { useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';

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
  const confirmPasswordRef = useRef<HTMLInputElement>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [registered, setRegistered] = useState<string>('');
  const [registrationError, setRegistrationError] = useState<string>('');
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

            // check for empty inputs
            if (
              !userRef.current?.value ||
              userRef.current.value === '' ||
              !passwordRef.current?.value ||
              passwordRef.current.value === '' ||
              !confirmPasswordRef.current?.value ||
              confirmPasswordRef.current.value === ''
            ) {
              setLoading(false);
              setRegistrationError(
                'All fields on this page are required. Please do not leave any blank!'
              );
              return;
            }

            // check password and confirm password match
            if (
              passwordRef.current?.value !== confirmPasswordRef.current?.value
            ) {
              setLoading(false);
              setRegistrationError(
                'Your password does not match your confirm password!'
              );
              return;
            }

            const registerPayload: RegisterPayload = {
              email: userRef.current?.value,
              password: passwordRef.current?.value,
            };
            const registerRequest = await fetch(`/api/auth/register/`, {
              method: 'POST',
              headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
              },
              body: JSON.stringify(registerPayload),
            });

            setLoading(false);
            if (registerRequest.ok) {
              const registerJson: RegisterResponse =
                await (registerRequest.json() as Promise<RegisterResponse>);
              if (registerJson.success) {
                // we have registered successfully!
                setRegistered(registerPayload.email);
              } else {
                setRegistrationError(
                  'An error has occurred: a user with this email already exists.'
                );
              }
            }
          }}
        >
          <div className="input-item">
            <h1>Register Account</h1>
          </div>
          <div className="input-item">
            {registrationError !== '' && (
              <div style={{ textAlign: 'center', marginBottom: '0.5rem' }}>
                <span style={{ color: '#CF6679' }}>{registrationError}</span>
              </div>
            )}
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
            <span className="subtitle">Confirm Password</span>
            <input
              ref={confirmPasswordRef}
              type="password"
              name="confirm_password"
              disabled={loading}
            />
          </div>
          <div className="input-item">
            <button type="submit" disabled={loading}>
              Register
            </button>
            <div className="register-link">
              {/* eslint-disable-next-line jsx-a11y/anchor-is-valid, jsx-a11y/no-static-element-interactions, jsx-a11y/click-events-have-key-events */}
              <a onClick={() => navigate('/login', { replace: true })}>
                Return to Login Page
              </a>
            </div>
          </div>
          {loading && (
            <div className="loading-overlay">
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
              onClick={() =>
                navigate(`/login?email=${registered}`, { replace: true })
              }
            >
              Return to Login Page
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
export default Register;
