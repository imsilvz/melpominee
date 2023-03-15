import React, { useEffect, useRef, useState } from 'react';

// local files
import './HumanityTracker.scss';

interface HumanityTrackerProps {
  rootKey: string;
  dotCount?: number;
  maxValue?: number;
  stains?: number;
  loss?: number;
  onChange?: (oldVal: number, newVal: number) => void;
}

const HumanityTracker = ({
  rootKey,
  dotCount,
  maxValue,
  stains,
  loss,
  onChange,
}: HumanityTrackerProps) => {
  const clickRef = useRef<Map<number, ReturnType<typeof setTimeout>> | null>(null);
  const [prevStains, setPrevStains] = useState<number>(0);
  const [currStains, setCurrStains] = useState<number>(stains || 0);
  const [currLoss, setCurrLoss] = useState<number>(loss || 0);
  const currentHumanity = (maxValue || 0) - (currLoss || 0);

  useEffect(() => {
    clickRef.current = new Map<number, ReturnType<typeof setTimeout>>();
  }, []);

  return (
    <div className="humanitytracker-container">
      {Array.from(Array(dotCount || 10), (_, i) => i).map((_, idx) => {
        const filled = currentHumanity >= idx + 1;
        const stained = (dotCount || 10) - currStains <= idx;
        return (
          <input
            // eslint-disable-next-line react/no-array-index-key
            key={`${rootKey}_dot${idx}`}
            type="radio"
            className={`humanitytracker-dot ${
              stained ? 'humanitytracker-stained' : ''
            }`}
            checked={filled || stained}
            onChange={() => {}}
            onClick={() => {
              const clickMap = clickRef.current;
              if (clickMap) {
                if (clickMap.has(idx)) {
                  // double click!
                  console.log('Double Click!');
                  setCurrLoss((maxValue || 0) - (idx + 1));
                  setCurrStains(prevStains);
                  return;
                }
                // mark this cell as having been clicked on
                clickMap.set(
                  idx,
                  setTimeout(() => {
                    clickMap.delete(idx);
                  }, 250),
                );
                setPrevStains(currStains);
              }
              if (stained) {
                // remove stain
                console.log(idx, (dotCount || 10) - currStains);
                if (idx === (dotCount || 10) - currStains) {
                  setCurrStains(0);
                } else {
                  setCurrStains((dotCount || 10) - idx);
                }
              } else if (!filled && !stained) {
                // add stain
                setCurrStains((dotCount || 10) - idx);
              }
            }}
          />
        );
      })}
    </div>
  );
};
export default HumanityTracker;
