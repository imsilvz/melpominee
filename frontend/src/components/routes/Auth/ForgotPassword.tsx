import React, { useRef, useState } from 'react';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';

const ForgotPassword = () => {
  const emailRef = useRef<HTMLInputElement>(null);
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
            Continue
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
export default ForgotPassword;
