import React from 'react';
import { useAppSelector } from '../../../redux/hooks';
import { selectResonances } from '../../../redux/reducers/masterdataReducer';

// types
import { Character, CharacterStat } from '../../../types/Character';

// local files
import HealthTracker from './HealthTracker';
import HumanityTracker from './HumanityTracker';
import './SecondarySection.scss';

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
  const maxWillpower = character.attributes.composure + character.attributes.resolve;
  const humanityValue = character.secondaryStats.humanity.baseValue;
  const humanityStains = character.secondaryStats.humanity.superficialDamage;
  const humanityLoss = character.secondaryStats.humanity.aggravatedDamage;
  return (
    <div className="charactersheet-secondary">
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Health</h4>
          <HealthTracker rootKey="secondarystat-health" value={maxHealth} />
          <HealthTracker rootKey="secondarystat-health-damage" value={0} />
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Willpower</h4>
          <HealthTracker rootKey="secondarystat-willpower" value={maxWillpower} />
          <HealthTracker rootKey="secondarystat-willpower-damage" value={0} />
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
          <HealthTracker
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
