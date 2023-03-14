import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './ProfileSection.scss';

interface ProfileSectionProps {}

const ProfileSection = ({}: ProfileSectionProps) => {
  return (
    <CharacterSheetSection className="charactersheet-profile-inner" title="Profile">
      <div className="profile-header">
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>True Age:</span>
            <textarea rows={1} cols={3} maxLength={4} />
          </div>
        </div>
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>Apparent Age:</span>
            <textarea rows={1} cols={3} maxLength={3} />
          </div>
        </div>
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>Date of Birth:</span>
            <textarea rows={1} cols={12} maxLength={12} />
          </div>
        </div>
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>Date of Death:</span>
            <textarea rows={1} cols={12} maxLength={12} />
          </div>
        </div>
      </div>
      <div className="profile-section">
        <div className="profile-section-item">
          <div className="profile-section-textarea">
            <span>Descriptions & Features</span>
            <textarea rows={4} />
          </div>
        </div>
      </div>
      <div className="profile-section">
        <div className="profile-section-item">
          <div className="profile-section-textarea">
            <span>History</span>
            <textarea rows={4} />
          </div>
        </div>
      </div>
      <div className="profile-section">
        <div className="profile-section-item">
          <div className="profile-section-textarea">
            <span>Notes</span>
            <textarea rows={4} />
          </div>
        </div>
      </div>
    </CharacterSheetSection>
  );
};
export default ProfileSection;
