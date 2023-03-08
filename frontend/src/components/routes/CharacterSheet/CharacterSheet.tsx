import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

// local files
import {
  Character,
  CharacterAttributes,
  CharacterSkills,
} from '../../../types/Character';
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import HeaderBrand from './HeaderBrand';
import StatDots from './StatDots';
import './CharacterSheet.scss';

// import { ReactComponent as Logo } from '../../../assets/VampireLogo.svg';

interface APICharacterSheetResponse {
  success: boolean;
  error?: string;
  character?: Character;
}

const toTitleCase = (str: string) => {
  return str
    .toLowerCase()
    .split(' ')
    .map((s) => s.charAt(0).toUpperCase() + s.substring(1))
    .join(' ');
};

const HeaderBlock = ({ character }: { character: Character }) => {
  return (
    <div className="charactersheet-header">
      <HeaderBrand clan={character.clan} />
      <div className="charactersheet-header-inner">
        <div className="charactersheet-header-column">
          <div className="charactersheet-header-row">
            <span>Name:</span>
          </div>
          <div className="charactersheet-header-row">
            <span>Concept:</span>
          </div>
          <div className="charactersheet-header-row">
            <span>Chronicle:</span>
          </div>
        </div>
        <div className="charactersheet-header-column">
          <div className="charactersheet-header-row">
            <span>Ambition:</span>
          </div>
          <div className="charactersheet-header-row">
            <span>Desire:</span>
          </div>
          <div className="charactersheet-header-row">
            <span>Predator Type:</span>
          </div>
        </div>
        <div className="charactersheet-header-column">
          <div className="charactersheet-header-row">
            <span>Clan:</span>
          </div>
          <div className="charactersheet-header-row">
            <span>Generation:</span>
          </div>
          <div className="charactersheet-header-row">
            <span>Sire:</span>
          </div>
        </div>
      </div>
    </div>
  );
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
                  rootKey={`attributes_physical_${attr}`}
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
                  rootKey={`attributes_social_${attr}`}
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
                  rootKey={`attributes_mental_${attr}`}
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
          <div
            // eslint-disable-next-line react/no-array-index-key
            key={`skills_column${columnIdx}`}
            className="charactersheet-statblock-section"
          >
            {column.map((skill) => (
              <div
                key={`skills_item_${skill}`}
                className="charactersheet-skill-item"
              >
                <div className="charactersheet-statblock-item-info">
                  <span>{toTitleCase(skill)}</span>
                </div>
                <div className="charactersheet-skillblock-speciality">
                  <input type="text" />
                </div>
                <div className="charactersheet-statblock-item-score">
                  <StatDots
                    rootKey={`skills_item_${skill}_dots`}
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
          <HeaderBlock character={character} />
          <AttributeBlock attributes={character.attributes} />
          <SkillBlock skills={character.skills} />
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
