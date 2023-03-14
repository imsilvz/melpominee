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
      <div className="meritflaw-list">
        {Array.from(Array(25), (_skip, i) => i).map((_skipRow, rowIdx) => (
          <div className="meritflaw-row">
            <input />
            <StatDots rootKey={`merit-${rowIdx}`} />
          </div>
        ))}
      </div>
    </CharacterSheetSection>
  );
};
export default MeritFlawSection;
