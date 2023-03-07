import React, { useRef, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import { validatePassword } from '../../../util/auth';

interface StartResetPasswordPayload {
  email: string;
}

interface StartResetPasswordResponse {
  success: boolean;
  error: string;
}

interface ConfirmResetPasswordPayload {
  key: string;
  email: string;
  password: string;
}

interface ConfirmResetPasswordResponse {
  success: boolean;
  error: string;
}

interface ConfirmPasswordProps {
  email: string;
  resetKey: string;
}

const ConfirmForgotPassword = ({ email, resetKey }: ConfirmPasswordProps) => {
  const navigate = useNavigate();
  const passwordRef = useRef<HTMLInputElement>(null);
  const confirmPasswordRef = useRef<HTMLInputElement>(null);
  const [forgotError, setForgotError] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);
  return (
    <div className="login-container">
      <form
        method="post"
        className="login-panel"
        // eslint-disable-next-line @typescript-eslint/no-misused-promises, @typescript-eslint/require-await
        onSubmit={async (event) => {
          event.preventDefault();
          setLoading(true);

          // check for empty inputs
          const password = passwordRef.current?.value;
          const confirmPassword = confirmPasswordRef.current?.value;
          if (
            !password ||
            password === '' ||
            !confirmPassword ||
            confirmPassword === ''
          ) {
            setLoading(false);
            setForgotError('All fields on this page are required!');
            return;
          }

          // validate password
          if (!validatePassword(password)) {
            setLoading(false);
            setForgotError(
              'Your password must be at least 8 characters long and have at least one number, uppercase letter, or special character.'
            );
            return;
          }

          if (password !== confirmPassword) {
            setLoading(false);
            setForgotError('Your password must match your confirm password!');
            return;
          }

          // make request!
          const resetPayload: ConfirmResetPasswordPayload = {
            key: resetKey,
            email,
            password,
          };
          const resetRequest = await fetch(
            `/api/auth/reset-password/confirmation`,
            {
              method: 'POST',
              headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
              },
              body: JSON.stringify(resetPayload),
            }
          );

          setLoading(false);
          if (resetRequest.ok) {
            const resetJson: ConfirmResetPasswordResponse =
              await (resetRequest.json() as Promise<ConfirmResetPasswordResponse>);
            if (resetJson.success) {
              // success
              navigate(`/login?notice=reset&email=${email}`, { replace: true });
            } else {
              // failure
              setForgotError(
                'An error has occurred during the password reset.'
              );
            }
          }
        }}
      >
        <div className="input-item">
          <h1>Reset Password.</h1>
        </div>
        <div className="input-item">
          {forgotError !== '' && (
            <div style={{ textAlign: 'center', marginBottom: '0.5rem' }}>
              <span style={{ color: '#CF6679' }}>{forgotError}</span>
            </div>
          )}
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
            Reset my Password
          </button>
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

const ForgotPassword = () => {
  const navigate = useNavigate();
  const emailRef = useRef<HTMLInputElement>(null);
  const [searchParams] = useSearchParams();
  const [resetSent, setResetSent] = useState<boolean>(false);
  const [forgotError, setForgotError] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);

  // separate handling for confirmation
  if (searchParams.has('email') && searchParams.has('key')) {
    return (
      <ConfirmForgotPassword
        email={searchParams.get('email') as string}
        resetKey={searchParams.get('key') as string}
      />
    );
  }

  return (
    <div className="login-container">
      {resetSent ? (
        <form
          method="get"
          className="login-panel"
          // eslint-disable-next-line @typescript-eslint/no-misused-promises, @typescript-eslint/require-await
          onSubmit={async (event) => {
            event.preventDefault();
            navigate('/login', { replace: true });
          }}
        >
          <div className="input-item">
            <h1>Email Sent!</h1>
          </div>
          <div className="input-item">
            <span style={{ color: 'rgba(255, 255, 255, 0.9)' }}>
              An email has been sent with instructions how to reset your
              password. You should receive it within a few minutes. If the email
              does not arrive within a reasonable timeframe, please be sure to
              check your spam or junk inboxes.
            </span>
          </div>
          <div className="input-item">
            <button type="submit">Return to Login</button>
          </div>
        </form>
      ) : (
        <form
          method="post"
          className="login-panel"
          // eslint-disable-next-line @typescript-eslint/no-misused-promises, @typescript-eslint/require-await
          onSubmit={async (event) => {
            event.preventDefault();
            setLoading(true);

            // check for empty inputs
            const email = emailRef.current?.value;
            if (!email || email === '') {
              setLoading(false);
              setForgotError('Invalid email address!');
              return;
            }

            // basic input validation
            const emailRegex = /^\S+@\S+$/;
            if (!emailRegex.test(email)) {
              setLoading(false);
              setForgotError('Invalid email address!');
              return;
            }

            // make request!
            const resetPayload: StartResetPasswordPayload = {
              email,
            };
            const resetRequest = await fetch(`/api/auth/reset-password/`, {
              method: 'POST',
              headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
              },
              body: JSON.stringify(resetPayload),
            });

            setLoading(false);
            if (resetRequest.ok) {
              const resetJson: StartResetPasswordResponse =
                await (resetRequest.json() as Promise<StartResetPasswordResponse>);
              if (resetJson.success) {
                // request successful, provide instructions
                setResetSent(true);
              } else if (resetJson.error === 'missing_email') {
                setForgotError('Invalid email address!');
              } else if (resetJson.error === 'not_found') {
                setForgotError('No user with this email address was found.');
              }
            }
          }}
        >
          <div className="input-item">
            <h1>Forgot Password.</h1>
          </div>
          <div className="input-item">
            {forgotError !== '' && (
              <div style={{ textAlign: 'center', marginBottom: '0.5rem' }}>
                <span style={{ color: '#CF6679' }}>{forgotError}</span>
              </div>
            )}
            <span className="subtitle">Email</span>
            <input
              ref={emailRef}
              type="email"
              name="email"
              disabled={loading}
              onBlur={() => setForgotError('')}
            />
          </div>
          <div className="input-item">
            <button type="submit" disabled={loading}>
              Reset Password
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
      )}
    </div>
  );
};
export default ForgotPassword;
