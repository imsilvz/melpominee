import React from 'react';

// types
import { MeritBackgroundFlaw } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './MeritFlawSection.scss';
import StatDots from './StatDots';

interface MeritFlawSectionProps {
  Backgrounds: MeritBackgroundFlaw[];
  Merits: MeritBackgroundFlaw[];
  Flaws: MeritBackgroundFlaw[];
}

const MeritFlawSection = ({ Backgrounds, Merits, Flaws }: MeritFlawSectionProps) => {
  console.log(Backgrounds, Merits, Flaws);
  return (
    <CharacterSheetSection
      className="charactersheet-meritflaw-inner"
      title="Merits & Flaws"
    >
      <div className="meritflaw-group">
        <span>Backgrounds</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => {
            const background = Backgrounds.find((item) => item.sortOrder === rowIdx);
            return (
              <div key={`background-${rowIdx}`} className="meritflaw-row">
                <input value={background ? background.name : ''} />
                <StatDots
                  rootKey={`background-${rowIdx}`}
                  value={background ? background.score : 0}
                />
              </div>
            );
          })}
        </div>
      </div>
      <div className="meritflaw-group">
        <span>Merits</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => {
            const merit = Merits.find((item) => item.sortOrder === rowIdx);
            return (
              <div key={`merit-${rowIdx}`} className="meritflaw-row">
                <input value={merit ? merit.name : ''} />
                <StatDots
                  rootKey={`merit-${rowIdx}`}
                  value={merit ? merit.score : 0}
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
            const flaw = Flaws.find((item) => item.sortOrder === rowIdx);
            return (
              <div key={`flaw-${rowIdx}`} className="meritflaw-row">
                <input value={flaw ? flaw.name : ''} />
                <StatDots rootKey={`flaw-${rowIdx}`} value={flaw ? flaw.score : 0} />
              </div>
            );
          })}
        </div>
      </div>
    </CharacterSheetSection>
  );
};
export default MeritFlawSection;
