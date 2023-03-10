import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

// types
import { Character, CharacterDisciplines } from '../../../types/Character';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import HeaderSection from './HeaderSection';
import AttributeSection from './AttributeSection';
import SkillsSection from './SkillsSection';
import DisciplineSection from './DisciplineSection';
import HealthTracker from './HealthTracker';
import './CharacterSheet.scss';

interface APICharacterSheetResponse {
  success: boolean;
  error?: string;
  character?: Character;
}

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
          <AttributeSection attributes={currCharacter.attributes} />
          <SkillsSection skills={currCharacter.skills} />
          <SecondaryBlock character={currCharacter} />
          <DisciplineSection
            characterId={currCharacter.id}
            levels={currCharacter.disciplines}
            powers={currCharacter.disciplinePowers}
            onLevelChange={(school, oldVal, newVal) => {
              if (Object.keys(currCharacter.disciplines).includes(school)) {
                currCharacter.disciplines[
                  school as keyof CharacterDisciplines
                ] = newVal;
                setCurrCharacter(
                  (char) =>
                    char && {
                      ...char,
                    }
                );
              }
            }}
            onPowerChange={(oldVal, newVal, schoolChange) => {
              if (schoolChange) {
                // entire discipline changed
              } else {
                // single ability changed
                console.log(oldVal, newVal, schoolChange);
              }
            }}
          />
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
