import React, { useEffect, useState } from 'react';

// local files
import './StatDots.scss';

interface StatDotsProps {
  rootKey: string;
  dotCount?: number;
  value?: string | number;
  onChange?: (oldVal: number, newVal: number) => void;
}

const StatDots = ({ rootKey, dotCount, value, onChange }: StatDotsProps) => {
  const [dots, setDots] = useState<number>(
    (typeof value === 'string' ? parseInt(value, 10) : value) || 0,
  );
  useEffect(() => {
    if (value !== undefined) {
      setDots(typeof value === 'string' ? parseInt(value, 10) : value);
    }
  }, [value]);
  return (
    <div className="statdots-container">
      {Array.from(Array(dotCount || 5), (_, i) => i).map((_, idx) => (
        <input
          // eslint-disable-next-line react/no-array-index-key
          key={`${rootKey}_dot${idx}`}
          type="radio"
          className="statdots-dot"
          checked={(onChange ? (value as number) : dots) >= idx + 1}
          onChange={() => {}}
          onClick={() => {
            let newDots = 0;
            if (!(idx + 1 === dots)) {
              newDots = idx + 1;
            }
            if (onChange) {
              onChange(dots, newDots);
            } else {
              setDots(newDots);
            }
          }}
        />
      ))}
    </div>
  );
};
export default StatDots;
