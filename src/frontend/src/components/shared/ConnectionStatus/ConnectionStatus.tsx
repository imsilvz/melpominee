import React from 'react';
import './ConnectionStatus.scss';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import {
  requestManualRetry,
  selectConnectionAttempt,
  selectConnectionLastError,
  selectConnectionStatus,
} from '../../../redux/reducers/connectionReducer';

const ConnectionStatus = () => {
  const dispatch = useAppDispatch();
  const status = useAppSelector(selectConnectionStatus);
  const attempt = useAppSelector(selectConnectionAttempt);
  const lastError = useAppSelector(selectConnectionLastError);

  if (status === 'idle' || status === 'connected') {
    return null;
  }

  if (status === 'reconnecting') {
    return (
      <div
        className="connectionstatus-banner connectionstatus-banner--reconnecting"
        role="status"
        aria-live="polite"
      >
        <div className="connectionstatus-spinner-wrapper" aria-hidden="true" />
        <span className="connectionstatus-text">Reconnecting to server&hellip;</span>
        <span className="connectionstatus-attempt">
          {attempt <= 10 ? `(attempt ${attempt})` : '(still trying\u2026)'}
        </span>
      </div>
    );
  }

  const handleRetry = () => {
    dispatch(requestManualRetry());
  };

  return (
    <div
      className="connectionstatus-banner connectionstatus-banner--disconnected"
      role="alert"
      aria-atomic="true"
    >
      <span className="connectionstatus-text">Connection lost</span>
      {lastError && <span className="connectionstatus-error">{lastError}</span>}
      <button
        type="button"
        className="connectionstatus-retry"
        onClick={handleRetry}
        aria-label="Retry connection"
      >
        Retry
      </button>
    </div>
  );
};

export default ConnectionStatus;
