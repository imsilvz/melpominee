import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

// types
import {
  Character,
  CharacterAttributes,
  CharacterDisciplines,
} from '../../../types/Character';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import HeaderSection from './HeaderSection';
import AttributeSection from './AttributeSection';
import SkillsSection from './SkillsSection';
import SecondarySection from './SecondarySection';
import DisciplineSection from './DisciplineSection';
import './CharacterSheet.scss';

interface APICharacterSheetResponse {
  success: boolean;
  error?: string;
  character?: Character;
}

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
          <AttributeSection
            attributes={currCharacter.attributes}
            onChange={(attribute, value) => {
              if (Object.keys(currCharacter.attributes).includes(attribute)) {
                currCharacter.attributes[
                  attribute as keyof CharacterAttributes
                ] = value;
                setCurrCharacter(
                  (char) =>
                    char && {
                      ...char,
                    }
                );
              }
            }}
          />
          <SkillsSection skills={currCharacter.skills} />
          <SecondarySection character={currCharacter} />
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
