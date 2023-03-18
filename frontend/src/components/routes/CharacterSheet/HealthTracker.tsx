import React, { useEffect, useState, useRef } from 'react';

// local files
import './HealthTracker.scss';

interface HealthTrackerProps {
  rootKey: string;
  dotCount?: number;
  health?: number;
  superficial?: number;
  aggravated?: number;
  onChange?: (aggravated?: number, superficial?: number) => void;
}

const HealthTracker = ({
  rootKey,
  dotCount,
  health,
  superficial,
  aggravated,
  onChange,
}: HealthTrackerProps) => {
  const clickRef = useRef<Map<number, ReturnType<typeof setTimeout>> | null>(null);
  const [prevSuperficial, setPrevSuperficial] = useState<number>(0);
  const [superficialDamage, setSuperficialDamage] = useState<number>(
    (typeof superficial === 'string' ? parseInt(superficial, 10) : superficial) || 0,
  );
  const [aggravatedDamage, setAggravatedDamage] = useState<number>(
    (typeof aggravated === 'string' ? parseInt(aggravated, 10) : aggravated) || 0,
  );

  useEffect(() => {
    clickRef.current = new Map<number, ReturnType<typeof setTimeout>>();
  }, []);

  useEffect(() => {
    if (superficial !== undefined) {
      setSuperficialDamage(
        typeof superficial === 'string' ? parseInt(superficial, 10) : superficial,
      );
    }
  }, [superficial]);

  useEffect(() => {
    if (aggravated !== undefined) {
      setAggravatedDamage(
        typeof aggravated === 'string' ? parseInt(aggravated, 10) : aggravated,
      );
    }
  }, [aggravated]);

  return (
    <div className="healthtracker-container">
      {Array.from(Array(dotCount || 10), (_, i) => i).map((_, idx) => {
        const filled =
          (((superficial === undefined || aggravated === undefined) && health) ||
            superficialDamage + aggravatedDamage) >=
          idx + 1;
        const isAggravated = aggravatedDamage >= idx + 1;
        const isSuperficial = superficialDamage + aggravatedDamage >= idx + 1;
        let style = 'healthtracker-dot';
        if (isAggravated) {
          style = 'healthtracker-dot aggravated-damage';
        } else if (isSuperficial) {
          style = 'healthtracker-dot superficial-damage';
        }
        return (
          <input
            // eslint-disable-next-line react/no-array-index-key
            key={`${rootKey}_dot${idx}`}
            type="radio"
            className={style}
            checked={filled}
            onChange={() => {}}
            onClick={() => {
              const clickMap = clickRef.current;
              // do not track damage on the same bar
              // as we track health!
              if (aggravated === undefined || superficial === undefined) {
                return;
              }
              // do not respond to clicks above max health
              if (idx + 1 > (health || 10)) {
                return;
              }
              // if we are not the health bar, track damage
              if (clickMap) {
                if (clickMap.has(idx)) {
                  // double click!
                  console.log('Double Click!');
                  let newAgg;
                  let newSuper;
                  if (aggravatedDamage === idx + 1) {
                    newAgg = 0;
                  } else {
                    newAgg = idx + 1;
                  }
                  // eslint-disable-next-line prefer-const
                  newSuper = prevSuperficial;
                  if (newAgg + newSuper > (health || 10)) {
                    newSuper = (health || 10) - newAgg;
                  }
                  if (onChange) {
                    if (newAgg !== undefined) {
                      onChange(newAgg, undefined);
                    }
                    if (newSuper !== undefined) {
                      onChange(undefined, newSuper);
                    }
                  } else {
                    if (newAgg !== undefined) {
                      setAggravatedDamage(newAgg);
                    }
                    if (newSuper !== undefined) {
                      setSuperficialDamage(newSuper);
                    }
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
                setPrevSuperficial(superficialDamage);
              }
              if (!isAggravated) {
                let newSuper;
                // this is not an aggravated damage tile
                if (isSuperficial) {
                  // subtract superficial damage
                  if (idx - aggravatedDamage + 1 === superficialDamage) {
                    newSuper = 0;
                  } else {
                    newSuper = idx - aggravatedDamage + 1;
                  }
                } else {
                  // add superficial damage
                  newSuper = idx - aggravatedDamage + 1;
                }
                if (newSuper !== undefined) {
                  if (onChange) {
                    onChange(undefined, newSuper);
                  } else {
                    setSuperficialDamage(newSuper);
                  }
                }
              }
            }}
          />
        );
      })}
    </div>
  );
};
export default HealthTracker;
