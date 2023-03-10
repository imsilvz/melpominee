import React, { useEffect, useState } from 'react';

// local files
import './HealthTracker.scss';

interface StatDotsProps {
  rootKey: string;
  dotCount?: number;
  value?: number;
  onChange?: (oldVal: number, newVal: number) => void;
}

const HealthTracker = ({
  rootKey,
  dotCount,
  value,
  onChange,
}: StatDotsProps) => {
  const [dots, setDots] = useState<number>(value || 0);
  useEffect(() => {
    if (value !== undefined) {
      setDots(value);
    }
  }, [value]);
  return (
    <div className="healthtracker-container">
      {Array.from(Array(dotCount || 10), (_, i) => i).map((_, idx) => (
        <input
          // eslint-disable-next-line react/no-array-index-key
          key={`${rootKey}_dot${idx}`}
          type="radio"
          className="healthtracker-dot"
          checked={dots >= idx + 1}
          onChange={() => {}}
          onClick={() => {
            if (idx + 1 === dots) {
              if (value === undefined) {
                setDots(0);
              }
            } else if (value === undefined) {
              setDots(idx + 1);
            }
          }}
        />
      ))}
    </div>
  );
};
export default HealthTracker;
