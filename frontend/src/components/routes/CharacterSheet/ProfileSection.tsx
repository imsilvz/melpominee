import React from 'react';

// types
import { CharacterProfile } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './ProfileSection.scss';

interface ProfileSectionProps {
  profile: CharacterProfile;
  onChange?: (field: string, value: string) => void;
}

const ProfileSection = ({ profile, onChange }: ProfileSectionProps) => {
  return (
    <CharacterSheetSection className="charactersheet-profile-inner" title="Profile">
      <div className="profile-header">
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>True Age:</span>
            <textarea
              rows={1}
              cols={3}
              maxLength={4}
              value={profile.trueAge}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = parseInt(event.target.value, 10) || 0;
                if (onChange) {
                  onChange('trueAge', newVal.toString());
                }
              }}
            />
          </div>
        </div>
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>Apparent Age:</span>
            <textarea
              rows={1}
              cols={3}
              maxLength={3}
              value={profile.apparentAge}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = parseInt(event.target.value, 10) || 0;
                if (onChange) {
                  onChange('apparentAge', newVal.toString());
                }
              }}
            />
          </div>
        </div>
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>Date of Birth:</span>
            <textarea
              rows={1}
              cols={12}
              maxLength={12}
              value={profile.dateOfBirth}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = event.target.value || '';
                if (onChange) {
                  onChange('dateOfBirth', newVal);
                }
              }}
            />
          </div>
        </div>
        <div className="profile-section-item">
          <div className="profile-section-input">
            <span>Date of Death:</span>
            <textarea
              rows={1}
              cols={12}
              maxLength={12}
              value={profile.dateOfDeath}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = event.target.value || '';
                if (onChange) {
                  onChange('dateOfDeath', newVal);
                }
              }}
            />
          </div>
        </div>
      </div>
      <div className="profile-section">
        <div className="profile-section-item">
          <div className="profile-section-textarea">
            <span>Descriptions & Features</span>
            <textarea
              rows={4}
              value={profile.description}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = event.target.value || '';
                if (onChange) {
                  onChange('description', newVal);
                }
              }}
            />
          </div>
        </div>
      </div>
      <div className="profile-section">
        <div className="profile-section-item">
          <div className="profile-section-textarea">
            <span>History</span>
            <textarea
              rows={4}
              value={profile.history}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = event.target.value || '';
                if (onChange) {
                  onChange('history', newVal);
                }
              }}
            />
          </div>
        </div>
      </div>
      <div className="profile-section">
        <div className="profile-section-item">
          <div className="profile-section-textarea">
            <span>Notes</span>
            <textarea
              rows={4}
              value={profile.notes}
              onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
                const newVal = event.target.value || '';
                if (onChange) {
                  onChange('notes', newVal);
                }
              }}
            />
          </div>
        </div>
      </div>
    </CharacterSheetSection>
  );
};
export default ProfileSection;
