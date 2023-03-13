import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './MeritFlawSection.scss';

interface MeritFlawSectionProps {}

const MeritFlawSection = ({}: MeritFlawSectionProps) => {
  return (
    <CharacterSheetSection
      className="charactersheet-meritflaw-inner"
      title="Merits & Flaws"
    />
  );
};
export default MeritFlawSection;
