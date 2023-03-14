import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import StatDots from './StatDots';
import './TheBloodSection.scss';

interface TheBloodSectionProps {
  BloodPotency: number;
}

const TheBloodSection = ({ BloodPotency }: TheBloodSectionProps) => {
  return (
    <CharacterSheetSection
      className="charactersheet-theblood-inner"
      title="The Blood"
    >
      <div className="charactersheet-theblood-section">
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-bloodpotency">
            <span>Blood Potency</span>
            <StatDots
              rootKey="theblood-bloodpotency"
              dotCount={10}
              value={BloodPotency}
            />
          </div>
        </div>
      </div>
      <div className="charactersheet-theblood-section">
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-input">
            <span>Blood Surge</span>
            <input />
          </div>
          <div className="charactersheet-theblood-input">
            <span>Power Bonus</span>
            <input />
          </div>
          <div className="charactersheet-theblood-input">
            <span>Mend Amount</span>
            <input />
          </div>
          <div className="charactersheet-theblood-input">
            <span>Rouse Re-Roll</span>
            <input />
          </div>
        </div>
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-input">
            <span>Rouse Re-Roll</span>
            <input />
          </div>
          <div className="charactersheet-theblood-textarea">
            <span>Feeding Penalty</span>
            <textarea />
          </div>
        </div>
      </div>
      <div className="charactersheet-theblood-section">
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-textarea">
            <span>Clan Bane</span>
            <textarea rows={6} />
          </div>
        </div>
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-textarea">
            <span>Clan Compulsion</span>
            <textarea />
          </div>
        </div>
      </div>
      <div className="charactersheet-theblood-section">
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-input">
            <span>XP Spent</span>
            <input />
          </div>
        </div>
        <div className="charactersheet-theblood-item">
          <div className="charactersheet-theblood-input">
            <span>Total XP</span>
            <input />
          </div>
        </div>
      </div>
    </CharacterSheetSection>
  );
};
export default TheBloodSection;
