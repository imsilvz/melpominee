import React from 'react';
import { useAppSelector } from '../../../redux/hooks';
import {
  selectBloodPotencies,
  selectClans,
} from '../../../redux/reducers/masterdataReducer';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import StatDots from './StatDots';
import './TheBloodSection.scss';

interface TheBloodSectionProps {
  Clan: string;
  BloodPotency: number;
  XpSpent: number;
  XpTotal: number;
  onChange?: (field: string, val: string) => void;
}

const TheBloodSection = ({
  Clan,
  BloodPotency,
  XpSpent,
  XpTotal,
  onChange,
}: TheBloodSectionProps) => {
  const clans = useAppSelector(selectClans);
  const potencies = useAppSelector(selectBloodPotencies);
  const clan = clans[Clan];
  const potency = potencies[BloodPotency];
  return (
    potency && (
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
                onChange={(oldVal, newVal) => {
                  if (onChange) {
                    console.log(newVal);
                    onChange('bloodPotency', newVal.toString());
                  }
                }}
              />
            </div>
          </div>
        </div>
        <div className="charactersheet-theblood-section">
          <div className="charactersheet-theblood-item">
            <div className="charactersheet-theblood-input">
              <span>Blood Surge</span>
              <input
                disabled
                value={`Add ${potency.bloodSurge} ${
                  potency.bloodSurge <= 2 ? 'die' : 'dice'
                }`}
              />
            </div>
            <div className="charactersheet-theblood-input">
              <span>Power Bonus</span>
              <input
                disabled
                value={
                  potency.powerBonus === 0
                    ? 'None'
                    : `Add ${potency.powerBonus} ${
                        potency.powerBonus <= 2 ? 'die' : 'dice'
                      }`
                }
              />
            </div>
            <div className="charactersheet-theblood-input">
              <span>Mend Amount</span>
              <input disabled value={`${potency.damageMend} Superficial damage`} />
            </div>
            <div className="charactersheet-theblood-input">
              <span>Rouse Re-Roll</span>
              <input
                disabled
                value={
                  potency.rouseReroll === 0
                    ? 'None'
                    : `Level ${potency.rouseReroll} ${
                        potency.rouseReroll >= 2 ? 'and below' : ''
                      }`
                }
              />
            </div>
          </div>
          <div className="charactersheet-theblood-item">
            <div className="charactersheet-theblood-input">
              <span>Bane Severity</span>
              <input disabled value={potency.baneSeverity} />
            </div>
            <div className="charactersheet-theblood-textarea">
              <span>Feeding Penalty</span>
              <textarea disabled value={potency.feedingPenalty.join('\n')} />
            </div>
          </div>
        </div>
        <div className="charactersheet-theblood-section">
          <div className="charactersheet-theblood-item">
            <div className="charactersheet-theblood-textarea">
              <span>Clan Bane</span>
              <textarea disabled rows={6} value={clan && clan.bane} />
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
              <input
                type="number"
                value={XpSpent}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  const val = parseInt(event.target.value, 10) || 0;
                  if (onChange) {
                    onChange('xpSpent', val.toString());
                  }
                }}
              />
            </div>
          </div>
          <div className="charactersheet-theblood-item">
            <div className="charactersheet-theblood-input">
              <span>Total XP</span>
              <input
                type="number"
                value={XpTotal}
                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                  const val = parseInt(event.target.value, 10) || 0;
                  if (onChange) {
                    onChange('xpTotal', val.toString());
                  }
                }}
              />
            </div>
          </div>
        </div>
      </CharacterSheetSection>
    )
  );
};
export default TheBloodSection;
