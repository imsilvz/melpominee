import React, { useEffect, useRef, useState } from 'react';

// types
import { CharacterStat } from '../../../types/Character';

// local files
import './HumanityTracker.scss';

interface HumanityTrackerProps {
  rootKey: string;
  dotCount?: number;
  maxValue?: number;
  stains?: number;
  loss?: number;
  onChange?: (loss?: number, stains?: number) => void;
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
  const [currStains, setCurrStains] = useState<number>(
    (typeof stains === 'string' ? parseInt(stains, 10) : stains) || 0,
  );
  const [currLoss, setCurrLoss] = useState<number>(
    (typeof loss === 'string' ? parseInt(loss, 10) : loss) || 0,
  );
  const currentHumanity = (maxValue || 0) - (currLoss || 0);

  useEffect(() => {
    clickRef.current = new Map<number, ReturnType<typeof setTimeout>>();
  }, []);

  useEffect(() => {
    if (loss !== undefined) {
      setCurrLoss(typeof loss === 'string' ? parseInt(loss, 10) : loss);
    }
  }, [loss]);

  useEffect(() => {
    if (stains !== undefined) {
      setCurrStains(typeof stains === 'string' ? parseInt(stains, 10) : stains);
    }
  }, [stains]);

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
                  if (onChange) {
                    onChange((maxValue || 0) - (idx + 1), prevStains);
                  } else {
                    setCurrLoss((maxValue || 0) - (idx + 1));
                    setCurrStains(prevStains);
                  }
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
                if (idx === (dotCount || 10) - currStains) {
                  if (onChange) {
                    onChange(undefined, 0);
                  } else {
                    setCurrStains(0);
                  }
                } else {
                  if (onChange) {
                    onChange(undefined, (dotCount || 10) - idx);
                  } else {
                    setCurrStains((dotCount || 10) - idx);
                  }
                }
              } else if (!filled && !stained) {
                // add stain
                if (onChange) {
                  onChange(undefined, (dotCount || 10) - idx);
                } else {
                  setCurrStains((dotCount || 10) - idx);
                }
              }
            }}
          />
        );
      })}
    </div>
  );
};
export default HumanityTracker;
