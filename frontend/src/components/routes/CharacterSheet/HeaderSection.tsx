import React, { useState } from 'react';

// redux
import { useAppSelector } from '../../../redux/hooks';
import {
  selectClans,
  selectPredatorTypes,
} from '../../../redux/reducers/masterdataReducer';

// types
import { Character } from '../../../types/Character';

// local files
import HeaderBrand from './HeaderBrand';
import './HeaderSection.scss';

interface HeaderSectionProps {
  character: Character;
  onChange?: (field: string, value: string) => void;
}

const HeaderSection = ({ character, onChange }: HeaderSectionProps) => {
  const clanData = useAppSelector(selectClans);
  const predatorData = useAppSelector(selectPredatorTypes);
  const [genState, setGenState] = useState<number | null>(null);

  // format generation
  let generationText = '';
  const ordinalRules = new Intl.PluralRules('en', { type: 'ordinal' });
  const suffixes: { [key: string]: string } = {
    one: 'st',
    two: 'nd',
    few: 'rd',
    other: 'th',
  };
  if (character.generation) {
    const rule = ordinalRules.select(character.generation);
    generationText = `${character.generation}${suffixes[rule as string]}`;
  }

  return (
    <div className="charactersheet-header">
      <HeaderBrand clan={character.clan} />
      <div className="charactersheet-header-inner">
        <div className="charactersheet-header-column">
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Name:</span>
            <span className="charactersheet-header-row-field">
              <input
                type="text"
                value={character.name}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  if (onChange) {
                    onChange('name', event.target.value);
                  }
                }}
              />
            </span>
          </div>
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Concept:</span>
            <span className="charactersheet-header-row-field">
              <input
                type="text"
                value={character.concept}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  if (onChange) {
                    onChange('concept', event.target.value);
                  }
                }}
              />
            </span>
          </div>
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Chronicle:</span>
            <span className="charactersheet-header-row-field">
              <input
                type="text"
                value={character.chronicle}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  if (onChange) {
                    onChange('chronicle', event.target.value);
                  }
                }}
              />
            </span>
          </div>
        </div>
        <div className="charactersheet-header-column">
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Ambition:</span>
            <span className="charactersheet-header-row-field">
              <input
                type="text"
                value={character.ambition}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  if (onChange) {
                    onChange('ambition', event.target.value);
                  }
                }}
              />
            </span>
          </div>
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Desire:</span>
            <span className="charactersheet-header-row-field">
              <input
                type="text"
                value={character.desire}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  if (onChange) {
                    onChange('desire', event.target.value);
                  }
                }}
              />
            </span>
          </div>
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">
              Predator Type:
            </span>
            <span className="charactersheet-header-row-field">
              <select
                value={character.predatorType || ''}
                onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
                  if (onChange) {
                    onChange('predatorType', event.target.value);
                  }
                }}
              >
                {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
                <option value="" />
                {predatorData &&
                  Object.keys(predatorData).map((type) => (
                    <option
                      key={`charactersheet-clanselect-option-${type}`}
                      value={type}
                    >
                      {predatorData[type].name}
                    </option>
                  ))}
              </select>
              <span className="select-dropdown" />
            </span>
          </div>
        </div>
        <div className="charactersheet-header-column">
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Clan:</span>
            <span className="charactersheet-header-row-field">
              <select
                value={character.clan || ''}
                onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
                  if (onChange) {
                    onChange('clan', event.target.value);
                  }
                }}
              >
                {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
                <option value="" />
                {clanData &&
                  Object.keys(clanData).map((clan) => (
                    <option
                      key={`charactersheet-clanselect-option-${clan}`}
                      value={clan}
                    >
                      {clanData[clan].name}
                    </option>
                  ))}
              </select>
              <span className="select-dropdown" />
            </span>
          </div>
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Generation:</span>
            <span className="charactersheet-header-row-field">
              <input
                type={genState !== null ? 'number' : 'text'}
                max={genState !== null ? '15' : undefined}
                min={genState !== null ? '1' : undefined}
                value={genState !== null ? genState : generationText}
                onBlur={() => {
                  if (genState && genState >= 1 && genState <= 15) {
                    if (onChange) {
                      onChange('generation', genState.toString());
                    }
                  }
                  setGenState(null);
                }}
                onFocus={() => {
                  setGenState(character.generation);
                }}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  setGenState(parseInt(event.target.value, 10));
                }}
              />
            </span>
          </div>
          <div className="charactersheet-header-row">
            <span className="charactersheet-header-row-label">Sire:</span>
            <span className="charactersheet-header-row-field">
              <input
                type="text"
                value={character.sire}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  if (onChange) {
                    onChange('sire', event.target.value);
                  }
                }}
              />
            </span>
          </div>
        </div>
      </div>
    </div>
  );
};
export default HeaderSection;
