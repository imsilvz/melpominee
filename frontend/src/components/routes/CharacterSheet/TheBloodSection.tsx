import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './TheBloodSection.scss';

interface TheBloodSectionProps {}

const TheBloodSection = ({}: TheBloodSectionProps) => {
  return (
    <CharacterSheetSection
      className="charactersheet-theblood-inner"
      title="The Blood"
    />
  );
};
export default TheBloodSection;
