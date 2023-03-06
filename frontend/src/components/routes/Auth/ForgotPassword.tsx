import React, { useRef, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';

interface ConfirmPasswordProps {
  email: string;
  resetKey: string;
}

const ConfirmForgotPassword = ({ email, resetKey }: ConfirmPasswordProps) => {
  return (
    <div className="login-container">
      <form
        method="post"
        className="login-panel"
        // eslint-disable-next-line @typescript-eslint/no-misused-promises, @typescript-eslint/require-await
        onSubmit={async (event) => {
          event.preventDefault();
        }}
      >
        <div className="input-item">
          <h1>e</h1>
        </div>
      </form>
    </div>
  );
};

const ForgotPassword = () => {
  const navigate = useNavigate();
  const emailRef = useRef<HTMLInputElement>(null);
  const [searchParams, setSearchParams] = useSearchParams();
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
              An email has been sent to your email address with instructions how
              to reset your password. You should receive it within a few
              minutes. If the email does not arrive within a reasonable
              timeframe, please be sure to check your spam or junk inboxes.
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
