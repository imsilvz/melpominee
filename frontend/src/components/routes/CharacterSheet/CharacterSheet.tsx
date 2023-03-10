import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

// types
import {
  Character,
  CharacterAttributes,
  CharacterSkills,
} from '../../../types/Character';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import HeaderSection from './HeaderSection';
import DisciplineSection from './DisciplineSection';
import HealthTracker from './HealthTracker';
import StatDots from './StatDots';
import './CharacterSheet.scss';

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
                  value={attributes[attr as keyof CharacterAttributes]}
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
                  value={attributes[attr as keyof CharacterAttributes]}
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
                  value={attributes[attr as keyof CharacterAttributes]}
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
                  <input
                    type="text"
                    value={skills[skill as keyof CharacterSkills].speciality}
                    onChange={(
                      event: React.ChangeEvent<HTMLInputElement>
                    ) => {}}
                  />
                </div>
                <div className="charactersheet-statblock-item-score">
                  <StatDots
                    rootKey={`skills_item_${skill}_dots`}
                    value={skills[skill as keyof CharacterSkills].score}
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

const SecondaryBlock = ({ character }: { character: Character }) => {
  const maxHealth = character.attributes.stamina + 3;
  const maxWillpower =
    character.attributes.composure + character.attributes.resolve;
  const humanityValue = character.secondaryStats.humanity.baseValue;
  return (
    <div className="charactersheet-secondary">
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Health</h4>
          <HealthTracker rootKey="secondarystat-health" value={maxHealth} />
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Willpower</h4>
          <HealthTracker
            rootKey="secondarystat-willpower"
            value={maxWillpower}
          />
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Humanity</h4>
          <HealthTracker
            rootKey="secondarystat-humanity"
            value={humanityValue}
          />
        </div>
      </div>
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-hungerrow-item">
          <h4>Resonance</h4>
          <span style={{ fontSize: '0.875rem' }}>{character.resonance}</span>
        </div>
        <div className="charactersheet-secondary-hungerrow-item">
          <h4>Hunger</h4>
          <HealthTracker
            rootKey="secondarystat-hunger"
            dotCount={5}
            value={character.hunger}
          />
        </div>
      </div>
    </div>
  );
};

const CharacterSheet = () => {
  const { id } = useParams();
  const [savedCharacter, setSavedCharacter] = useState<Character | null>(null);
  const [currCharacter, setCurrCharacter] = useState<Character | null>(null);

  useEffect(() => {
    setCurrCharacter(savedCharacter);
  }, [savedCharacter]);

  useEffect(() => {
    const fetchCharacter = async () => {
      if (id) {
        const characterRequest = await fetch(`/api/vtmv5/character/${id}/`);
        if (characterRequest.ok) {
          const characterJson: APICharacterSheetResponse =
            await (characterRequest.json() as Promise<APICharacterSheetResponse>);
          if (characterJson.character) {
            setSavedCharacter(characterJson.character);
          }
        }
      }
    };
    // eslint-disable-next-line no-console
    fetchCharacter().catch(console.error);
  }, [id]);

  return (
    <div className="charactersheet-container">
      {!currCharacter ? (
        <LoadingSpinner />
      ) : (
        <div className="charactersheet-panel">
          <HeaderSection
            character={currCharacter}
            onChange={(field, value) => {
              if (Object.prototype.hasOwnProperty.call(currCharacter, field)) {
                setCurrCharacter(
                  (char) =>
                    char && {
                      ...char,
                      [field]: value,
                    }
                );
              }
            }}
          />
          <AttributeBlock attributes={currCharacter.attributes} />
          <SkillBlock skills={currCharacter.skills} />
          <SecondaryBlock character={currCharacter} />
          <DisciplineSection
            characterId={currCharacter.id}
            levels={currCharacter.disciplines}
            powers={currCharacter.disciplinePowers}
          />
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
