import React from 'react';
import './ToggleSwitch.scss';

interface ToggleSwitchProps {
  label: string;
  checked: boolean;
  onSwitch?: (currentState: boolean) => void;
}

const ToggleSwitch = ({ label, checked, onSwitch }: ToggleSwitchProps) => {
  const handleToggle = (event: React.MouseEvent | React.KeyboardEvent) => {
    event.preventDefault();
    if (onSwitch !== undefined) {
      onSwitch(!checked);
    }
  };

  return (
    <div
      className="toggle-switch-container"
      role="button"
      tabIndex={0}
      onClick={handleToggle}
      onKeyDown={(event) => {
        if (event.key === 'Enter' || event.key === ' ') {
          handleToggle(event);
        }
      }}
    >
      <p>{label}</p>
      {/* eslint-disable-next-line jsx-a11y/label-has-associated-control */}
      <label className="toggle-switch" aria-label={label}>
        <input readOnly type="checkbox" checked={checked} />
        <span className="toggle-switch-slider" />
      </label>
    </div>
  );
};
export default ToggleSwitch;
