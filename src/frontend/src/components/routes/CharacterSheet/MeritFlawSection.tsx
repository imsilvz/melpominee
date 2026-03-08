import React from 'react';

// types
import { MeritBackgroundFlaw } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './MeritFlawSection.scss';
import StatDots from './StatDots';

interface MeritFlawSectionProps {
  Backgrounds: {
    [key: number]: MeritBackgroundFlaw;
  };
  Merits: {
    [key: number]: MeritBackgroundFlaw;
  };
  Flaws: {
    [key: number]: MeritBackgroundFlaw;
  };
  onChange?: (field: string, val: MeritBackgroundFlaw) => void;
}

const MeritFlawSection = ({
  Backgrounds,
  Merits,
  Flaws,
  onChange,
}: MeritFlawSectionProps) => {
  return (
    <CharacterSheetSection
      className="charactersheet-meritflaw-inner"
      title="Merits & Flaws"
    >
      <div className="meritflaw-group">
        <span>Advantages</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => {
            const background = Backgrounds[rowIdx];
            return (
              <div key={`background-${rowIdx}`} className="meritflaw-row">
                <input
                  value={background ? background.name : ''}
                  onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                    const text = event.target.value || '';
                    if (onChange) {
                      onChange('backgrounds', {
                        sortOrder: rowIdx,
                        name: text,
                        score: background ? background.score : 0,
                      });
                    }
                  }}
                />
                <StatDots
                  rootKey={`background-${rowIdx}`}
                  value={background ? background.score : 0}
                  onChange={(oldVal, newVal) => {
                    if (onChange) {
                      onChange('backgrounds', {
                        sortOrder: rowIdx,
                        name: background ? background.name : '',
                        score: newVal,
                      });
                    }
                  }}
                />
              </div>
            );
          })}
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => {
            const merit = Merits[rowIdx];
            return (
              <div key={`merit-${rowIdx}`} className="meritflaw-row">
                <input
                  value={merit ? merit.name : ''}
                  onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                    const text = event.target.value || '';
                    if (onChange) {
                      onChange('merits', {
                        sortOrder: rowIdx,
                        name: text,
                        score: merit ? merit.score : 0,
                      });
                    }
                  }}
                />
                <StatDots
                  rootKey={`merit-${rowIdx}`}
                  value={merit ? merit.score : 0}
                  onChange={(oldVal, newVal) => {
                    if (onChange) {
                      onChange('merits', {
                        sortOrder: rowIdx,
                        name: merit ? merit.name : '',
                        score: newVal,
                      });
                    }
                  }}
                />
              </div>
            );
          })}
        </div>
      </div>
      <div className="meritflaw-group">
        <span>Flaws</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => {
            const flaw = Flaws[rowIdx];
            return (
              <div key={`flaw-${rowIdx}`} className="meritflaw-row">
                <input
                  value={flaw ? flaw.name : ''}
                  onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                    const text = event.target.value || '';
                    if (onChange) {
                      onChange('flaws', {
                        sortOrder: rowIdx,
                        name: text,
                        score: flaw ? flaw.score : 0,
                      });
                    }
                  }}
                />
                <StatDots
                  rootKey={`flaw-${rowIdx}`}
                  value={flaw ? flaw.score : 0}
                  onChange={(oldVal, newVal) => {
                    if (onChange) {
                      onChange('flaws', {
                        sortOrder: rowIdx,
                        name: flaw ? flaw.name : '',
                        score: newVal,
                      });
                    }
                  }}
                />
              </div>
            );
          })}
        </div>
      </div>
    </CharacterSheetSection>
  );
};
export default MeritFlawSection;
