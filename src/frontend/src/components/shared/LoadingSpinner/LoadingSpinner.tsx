import React from 'react';
import './LoadingSpinner.scss';

interface SpinnerWithClass {
  className?: string;
}

const LoadingSpinner = ({ className }: SpinnerWithClass) => {
  let trueClassName = 'lds-spinner';
  if (className) {
    trueClassName = `${trueClassName} ${className}`;
  }
  return (
    <div className={trueClassName}>
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
