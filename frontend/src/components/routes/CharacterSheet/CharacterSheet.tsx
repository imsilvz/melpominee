import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

// local files
import {
  Character,
  CharacterAttributes,
  CharacterSkills,
} from '../../../types/Character';
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import StatDots from './StatDots';
import './CharacterSheet.scss';

// import { ReactComponent as Logo } from '../../../assets/VampireLogo.svg';

interface APICharacterSheetResponse {
  success: boolean;
  error?: string;
  character?: Character;
}

interface StatBlock {
  title?: string;
  dividers?: boolean;
  groups: StatGroup[];
}

interface StatGroup {
  groupName?: string;
  items: StatItem[];
}

interface StatItem {
  name: string;
}

const StatPanel = ({ stats }: { stats: StatBlock }) => {
  return (
    <div className="charactersheet-statblock">
      {stats.title && (
        <div className="charactersheet-statblock-header">
          <div className="charactersheet-statblock-header-title">
            <h2>{stats.title}</h2>
          </div>
          <div className="charactersheet-statblock-header-divider" />
        </div>
      )}
      <div className="charactersheet-statblock-inner">
        {stats.groups.map((group, groupIdx) => (
          <>
            <div
              // eslint-disable-next-line react/no-array-index-key
              key={`${groupIdx}_section`}
              className="charactersheet-statblock-section"
            >
              {group.groupName && (
                <div className="charactersheet-statblock-item">
                  <h3>{group.groupName}</h3>
                </div>
              )}
              {group.items.map((item) => (
                <div
                  // eslint-disable-next-line react/no-array-index-key
                  key={`${groupIdx}_item_${item.name}`}
                  className="charactersheet-statblock-item"
                >
                  <div className="charactersheet-statblock-item-info">
                    <span>{item.name}</span>
                  </div>
                  <div className="charactersheet-statblock-item-score">
                    <span>dot dot dot dot dot</span>
                  </div>
                </div>
              ))}
            </div>
            {stats.dividers && groupIdx !== stats.groups.length - 1 && (
              <div
                // eslint-disable-next-line react/no-array-index-key
                key={`${groupIdx}_divider`}
                className="charactersheet-statblock-divider"
              >
                <div className="charactersheet-statblock-divider-inner" />
              </div>
            )}
          </>
        ))}
      </div>
    </div>
  );
};

const toTitleCase = (str: string) => {
  return str
    .toLowerCase()
    .split(' ')
    .map((s) => s.charAt(0).toUpperCase() + s.substring(1))
    .join(' ');
};

const AttributeBlock = ({
  attributes,
}: {
  attributes: CharacterAttributes;
}) => {
  const physicalAttributes = ['strength', 'dexterity', 'stamina'];
  const socialAttributes = ['charisma', 'manipulation', 'composure'];
  const mentalAttributes = ['intelligence', 'wits', 'resolve'];
  return (
    <div className="charactersheet-statblock">
      <div className="charactersheet-statblock-header">
        <div className="charactersheet-statblock-header-title">
          <h2>Attributes</h2>
        </div>
        <div className="charactersheet-statblock-header-divider" />
      </div>
      <div className="charactersheet-statblock-inner">
        {/* Physical Attributes */}
        <div className="charactersheet-statblock-section">
          <div className="charactersheet-statblock-item">
            <h3>Physical</h3>
          </div>
          {physicalAttributes.map((attr) => (
            <div
              key={`attributes_physical_${attr}`}
              className="charactersheet-attribute-item"
            >
              <div className="charactersheet-statblock-item-info">
                <span>{toTitleCase(attr)}</span>
              </div>
              <div className="charactersheet-statblock-item-score">
                <StatDots
                  key={`attributes_physical_${attr}`}
                  initialValue={attributes[attr as keyof CharacterAttributes]}
                />
              </div>
            </div>
          ))}
        </div>
        <div className="charactersheet-statblock-divider">
          <div className="charactersheet-statblock-divider-inner" />
        </div>
        {/* Social Attributes */}
        <div className="charactersheet-statblock-section">
          <div className="charactersheet-statblock-item">
            <h3>Social</h3>
          </div>
          {socialAttributes.map((attr) => (
            <div
              key={`attributes_social_${attr}`}
              className="charactersheet-attribute-item"
            >
              <div className="charactersheet-statblock-item-info">
                <span>{toTitleCase(attr)}</span>
              </div>
              <div className="charactersheet-statblock-item-score">
                <StatDots
                  key={`attributes_social_${attr}`}
                  initialValue={attributes[attr as keyof CharacterAttributes]}
                />
              </div>
            </div>
          ))}
        </div>
        <div className="charactersheet-statblock-divider">
          <div className="charactersheet-statblock-divider-inner" />
        </div>
        {/* Mental Attributes */}
        <div className="charactersheet-statblock-section">
          <div className="charactersheet-statblock-item">
            <h3>Mental</h3>
          </div>
          {mentalAttributes.map((attr) => (
            <div
              key={`attributes_mental_${attr}`}
              className="charactersheet-attribute-item"
            >
              <div className="charactersheet-statblock-item-info">
                <span>{toTitleCase(attr)}</span>
              </div>
              <div className="charactersheet-statblock-item-score">
                <StatDots
                  key={`attributes_mental_${attr}`}
                  initialValue={attributes[attr as keyof CharacterAttributes]}
                />
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

const CharacterSkillsList = [
  'athletics',
  'brawl',
  'craft',
  'drive',
  'firearms',
  'melee',
  'larceny',
  'stealth',
  'survival',
  'animalKen',
  'ettiquette',
  'insight',
  'intimidation',
  'leadership',
  'performance',
  'persuasion',
  'streetwise',
  'subterfuge',
  'academics',
  'awareness',
  'finance',
  'investigation',
  'medicine',
  'occult',
  'politics',
  'science',
  'technology',
];
const SkillBlock = ({ skills }: { skills: CharacterSkills }) => {
  // break into columns
  let skillColumnIndex = -1;
  const skillColumns: string[][] = [];
  for (let i = 0; i < CharacterSkillsList.length; i++) {
    if (i % (CharacterSkillsList.length / 3) === 0) {
      skillColumnIndex += 1;
      skillColumns.push([]);
    }
    skillColumns[skillColumnIndex].push(CharacterSkillsList[i]);
  }

  return (
    <div className="charactersheet-statblock">
      <div className="charactersheet-statblock-header">
        <div className="charactersheet-statblock-header-title">
          <h2>Skills</h2>
        </div>
        <div className="charactersheet-statblock-header-divider" />
      </div>
      <div className="charactersheet-statblock-inner">
        {skillColumns.map((column, columnIdx) => (
          <div className="charactersheet-statblock-section">
            {column.map((skill, skillIdx) => (
              <div
                key={`attributes_social_${skill}`}
                className="charactersheet-skill-item"
              >
                <div className="charactersheet-statblock-item-info">
                  <span>{toTitleCase(skill)}</span>
                </div>
                <div className="charactersheet-skillblock-speciality" />
                <div className="charactersheet-statblock-item-score">
                  <StatDots
                    key={`attributes_social_${skill}`}
                    initialValue={skills[skill as keyof CharacterSkills].score}
                  />
                </div>
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
};

const CharacterSheet = () => {
  const { id } = useParams();
  const [character, setCharacter] = useState<Character | null>(null);

  useEffect(() => {
    const fetchCharacter = async () => {
      if (id) {
        const characterRequest = await fetch(`/api/vtmv5/character/${id}/`);
        if (characterRequest.ok) {
          const characterJson: APICharacterSheetResponse =
            await (characterRequest.json() as Promise<APICharacterSheetResponse>);
          if (characterJson.character) {
            console.log(characterJson.character);
            setCharacter(characterJson.character);
          }
        }
      }
    };
    // eslint-disable-next-line no-console
    fetchCharacter().catch(console.error);
  }, [id]);

  return (
    <div className="charactersheet-container">
      {!character ? (
        <LoadingSpinner />
      ) : (
        <div className="charactersheet-panel">
          <h1 style={{ color: 'white', textAlign: 'center' }}>
            Under Construction 😊
          </h1>
          <AttributeBlock attributes={character.attributes} />
          <SkillBlock skills={character.skills} />
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
