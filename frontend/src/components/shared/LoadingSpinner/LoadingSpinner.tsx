import React from 'react';
import './LoadingSpinner.scss';

const LoadingSpinner: React.FunctionComponent = () => {
  return (
    <div className="lds-spinner">
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
      <div />
    </div>
  );
};
export default LoadingSpinner;
