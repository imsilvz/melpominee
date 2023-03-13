import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './ProfileSection.scss';

interface ProfileSectionProps {}

const ProfileSection = ({}: ProfileSectionProps) => {
  return (
    <CharacterSheetSection
      className="charactersheet-profile-inner"
      title="Profile"
    />
  );
};
export default ProfileSection;
