import React from 'react';
import { useAppSelector } from '../../../redux/hooks';
import { selectResonances } from '../../../redux/reducers/masterdataReducer';

// types
import { Character, CharacterStat } from '../../../types/Character';

// local files
import HealthTracker from './HealthTracker';
import HumanityTracker from './HumanityTracker';
import './SecondarySection.scss';
import StatDots from './StatDots';

interface CharacterStatPayload {
  baseValue?: number;
  superficialDamage?: number;
  aggravatedDamage?: number;
}

interface SecondarySectionProps {
  character: Character;
  onChangeHeaderField?: (field: string, val: string) => void;
  onChangeSecondaryStat?: (field: string, val: CharacterStatPayload) => void;
}

const SecondarySection = ({
  character,
  onChangeHeaderField,
  onChangeSecondaryStat,
}: SecondarySectionProps) => {
  const resonances = useAppSelector(selectResonances);
  const maxHealth = character.attributes.stamina + 3;
  const superficialHealth = character.secondaryStats.health.superficialDamage;
  const aggravatedHealth = character.secondaryStats.health.aggravatedDamage;
  const maxWillpower = character.attributes.composure + character.attributes.resolve;
  const superficialWillpower = character.secondaryStats.willpower.superficialDamage;
  const aggravatedWillpower = character.secondaryStats.willpower.aggravatedDamage;
  const humanityValue = character.secondaryStats.humanity.baseValue;
  const humanityStains = character.secondaryStats.humanity.superficialDamage;
  const humanityLoss = character.secondaryStats.humanity.aggravatedDamage;
  return (
    <div className="charactersheet-secondary">
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Health</h4>
          <div className="healthrow-trackers">
            <HealthTracker
              rootKey="secondarystat-health"
              health={maxHealth}
              dotCount={maxHealth > 10 ? maxHealth : 10}
            />
            <HealthTracker
              rootKey="secondarystat-health-damage"
              dotCount={maxHealth > 10 ? maxHealth : 10}
              health={maxHealth}
              superficial={superficialHealth}
              aggravated={aggravatedHealth}
              onChange={(aggravated, superficial) => {
                if (onChangeSecondaryStat) {
                  const newStat: CharacterStatPayload = {};
                  if (aggravated !== undefined) {
                    newStat.aggravatedDamage = aggravated;
                  }
                  if (superficial !== undefined) {
                    newStat.superficialDamage = superficial;
                  }
                  onChangeSecondaryStat('health', newStat);
                }
              }}
            />
          </div>
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Willpower</h4>
          <div className="healthrow-trackers">
            <HealthTracker rootKey="secondarystat-willpower" health={maxWillpower} />
            <HealthTracker
              rootKey="secondarystat-willpower-damage"
              dotCount={maxWillpower > 10 ? maxWillpower : 10}
              health={maxWillpower}
              superficial={superficialWillpower}
              aggravated={aggravatedWillpower}
              onChange={(aggravated, superficial) => {
                if (onChangeSecondaryStat) {
                  const newStat: CharacterStatPayload = {};
                  if (aggravated !== undefined) {
                    newStat.aggravatedDamage = aggravated;
                  }
                  if (superficial !== undefined) {
                    newStat.superficialDamage = superficial;
                  }
                  onChangeSecondaryStat('willpower', newStat);
                }
              }}
            />
          </div>
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Humanity</h4>
          <HumanityTracker
            rootKey="secondarystat-humanity"
            maxValue={humanityValue}
            stains={humanityStains}
            loss={humanityLoss}
            onChange={(loss, stains) => {
              if (onChangeSecondaryStat) {
                const newStat: CharacterStatPayload = {};
                if (loss !== undefined) {
                  newStat.aggravatedDamage = loss;
                }
                if (stains !== undefined) {
                  newStat.superficialDamage = stains;
                }
                onChangeSecondaryStat('humanity', newStat);
              }
            }}
          />
        </div>
      </div>
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-hungerrow-item">
          <h4>Resonance</h4>
          <div className="hungerrow-select">
            <select
              value={character.resonance || ''}
              onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
                const newRes = event.target.value || '';
                if (onChangeHeaderField) {
                  onChangeHeaderField('resonance', newRes);
                }
              }}
            >
              {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
              <option value="" />
              {resonances &&
                Object.keys(resonances).map((resonance) => {
                  if (resonance === '') {
                    return null;
                  }
                  return (
                    <option
                      key={`charactersheet-secondary-option-${resonance}`}
                      value={resonance}
                    >
                      {resonances[resonance].name}
                    </option>
                  );
                })}
            </select>
            <span className="select-dropdown" />
          </div>
        </div>
        <div className="charactersheet-secondary-hungerrow-item">
          <h4>Hunger</h4>
          <StatDots
            rootKey="secondarystat-hunger"
            dotCount={5}
            value={character.hunger}
            onChange={(oldVal, newVal) => {
              if (onChangeHeaderField) {
                onChangeHeaderField('hunger', newVal.toString());
              }
            }}
          />
        </div>
      </div>
    </div>
  );
};
export default SecondarySection;
