import React from 'react';
import './ToggleSwitch.scss';
import { current } from '@reduxjs/toolkit';

interface ToggleSwitchProps {
  label: string;
  checked: boolean;
  onSwitch?: (currentState: boolean) => void;
}

const ToggleSwitch = ({ label, checked, onSwitch }: ToggleSwitchProps) => {
  return (
    <div className="toggle-switch-container" onClick={async (event) => {
      event.preventDefault();
      if (onSwitch !== undefined) {
        onSwitch(!!!checked);
      }
    }}>
      <p>{label}</p>
      <label className="toggle-switch">
        <input readOnly type="checkbox" checked={checked} />
        <span className="toggle-switch-slider" />
      </label>
    </div>
  );
}
export default ToggleSwitch;
