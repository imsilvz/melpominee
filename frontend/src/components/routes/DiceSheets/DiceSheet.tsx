import React from 'react';
import './DiceSheet.scss';

// import { ReactComponent as Logo } from '../../../assets/VampireLogo.svg';

interface StatItem {
  name: string;
  speciality?: string;
  score: number;
}

interface StatGroup {
  category?: string;
  allowSpecialities?: boolean;
  items: StatItem[];
}

interface StatBlock {
  title?: string;
  dividers?: boolean;
  groups: StatGroup[];
}

const StatPanel = ({ stats }: { stats: StatBlock }) => {
  return (
    <div className="dicesheet-statblock">
      {stats.title && (
        <div className="dicesheet-statblock-header">
          <div className="dicesheet-statblock-header-title">
            <h2>{stats.title}</h2>
          </div>
          <div className="dicesheet-statblock-header-divider" />
        </div>
      )}
      <div className="dicesheet-statblock-inner">
        {stats.groups.map((group, groupIdx) => (
          <>
            <div className="dicesheet-statblock-section">
              {group.category && (
                <div className="dicesheet-statblock-item">
                  <h3>{group.category}</h3>
                </div>
              )}
              {group.items.map((item, itemIdx) => (
                <div
                  className="dicesheet-statblock-item"
                  style={{
                    justifyContent: group.allowSpecialities
                      ? 'space-between'
                      : undefined,
                  }}
                >
                  <div
                    className="dicesheet-statblock-item-info"
                    style={{
                      flex: group.allowSpecialities ? 'unset' : '1',
                    }}
                  >
                    <span>{item.name}</span>
                  </div>
                  <div className="dicesheet-statblock-item-score">
                    <span>dot dot dot dot dot</span>
                  </div>
                </div>
              ))}
            </div>
            {stats.dividers && groupIdx !== stats.groups.length - 1 && (
              <div className="dicesheet-statblock-divider">
                <div className="dicesheet-statblock-divider-inner" />
              </div>
            )}
          </>
        ))}
      </div>
    </div>
  );
};

const DiceSheet = () => {
  return (
    <div className="dicesheet-container">
      <div className="dicesheet-panel">
        <h1 style={{ color: 'white', textAlign: 'center' }}>
          Under Construction 😊
        </h1>
        <StatPanel
          stats={{
            title: 'Attributes',
            dividers: true,
            groups: [
              {
                category: 'Physical',
                items: [
                  { name: 'Strength', score: 1 },
                  { name: 'Dexterity', score: 1 },
                  { name: 'Stamina', score: 1 },
                ],
              },
              {
                category: 'Social',
                items: [
                  { name: 'Charisma', score: 1 },
                  { name: 'Manipulation', score: 1 },
                  { name: 'Composure', score: 1 },
                ],
              },
              {
                category: 'Mental',
                items: [
                  { name: 'Intelligence', score: 1 },
                  { name: 'Wits', score: 1 },
                  { name: 'Resolve', score: 1 },
                ],
              },
            ],
          }}
        />
        <StatPanel
          stats={{
            title: 'Skills',
            dividers: false,
            groups: [
              {
                allowSpecialities: true,
                items: [
                  { name: 'Athletics', score: 1 },
                  { name: 'Brawl', score: 1 },
                  { name: 'Craft', score: 1 },
                  { name: 'Drive', score: 1 },
                  { name: 'Firearms', score: 1 },
                  { name: 'Melee', score: 1 },
                  { name: 'Larceny', score: 1 },
                  { name: 'Stealth', score: 1 },
                  { name: 'Survival', score: 1 },
                ],
              },
              {
                allowSpecialities: true,
                items: [
                  { name: 'Animal Ken', score: 1 },
                  { name: 'Etiquette', score: 1 },
                  { name: 'Insight', score: 1 },
                  { name: 'Intimidation', score: 1 },
                  { name: 'Leadership', score: 1 },
                  { name: 'Performance', score: 1 },
                  { name: 'Persuasion', score: 1 },
                  { name: 'Streetwise', score: 1 },
                  { name: 'Subterfuge', score: 1 },
                ],
              },
              {
                allowSpecialities: true,
                items: [
                  { name: 'Academics', score: 1 },
                  { name: 'Awareness', score: 1 },
                  { name: 'Finance', score: 1 },
                  { name: 'Investigation', score: 1 },
                  { name: 'Medicine', score: 1 },
                  { name: 'Occult', score: 1 },
                  { name: 'Politics', score: 1 },
                  { name: 'Science', score: 1 },
                  { name: 'Technology', score: 1 },
                ],
              },
            ],
          }}
        />
      </div>
    </div>
  );
};
export default DiceSheet;
