import React, { useState } from 'react';

// local files
import './StatDots.scss';

interface StatDotsProps {
  rootKey: string;
  initialValue?: number;
}

const StatDots = ({ rootKey, initialValue }: StatDotsProps) => {
  const [dots, setDots] = useState<number>(initialValue || 0);
  return (
    <div className="statdots-container">
      {Array.from(Array(5), (_, i) => i).map((_, idx) => (
        <input
          // eslint-disable-next-line react/no-array-index-key
          key={`${rootKey}_dot${idx}`}
          type="radio"
          className="statdots-dot"
          checked={dots >= idx + 1}
          onChange={() => {}}
          onClick={() => {
            if (idx + 1 === dots) {
              setDots(0);
            } else {
              setDots(idx + 1);
            }
          }}
        />
      ))}
    </div>
  );
};
export default StatDots;
