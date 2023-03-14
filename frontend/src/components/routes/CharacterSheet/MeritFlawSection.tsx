import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './MeritFlawSection.scss';
import StatDots from './StatDots';

interface MeritFlawSectionProps {}

const MeritFlawSection = ({}: MeritFlawSectionProps) => {
  return (
    <CharacterSheetSection
      className="charactersheet-meritflaw-inner"
      title="Merits & Flaws"
    >
      <div className="meritflaw-group">
        <span>Backgrounds</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => (
            <div className="meritflaw-row">
              <input />
              <StatDots rootKey={`merit-${rowIdx}`} />
            </div>
          ))}
        </div>
      </div>
      <div className="meritflaw-group">
        <span>Merits</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => (
            <div className="meritflaw-row">
              <input />
              <StatDots rootKey={`merit-${rowIdx}`} />
            </div>
          ))}
        </div>
      </div>
      <div className="meritflaw-group">
        <span>Flaws</span>
        <div className="meritflaw-list">
          {Array.from(Array(10), (_skip, i) => i).map((_skipRow, rowIdx) => (
            <div className="meritflaw-row">
              <input />
              <StatDots rootKey={`merit-${rowIdx}`} />
            </div>
          ))}
        </div>
      </div>
    </CharacterSheetSection>
  );
};
export default MeritFlawSection;
