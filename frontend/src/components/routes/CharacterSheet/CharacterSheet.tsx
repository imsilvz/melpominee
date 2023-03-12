import React, { useEffect, useRef, useState } from 'react';
import { useParams } from 'react-router-dom';

// redux
import { useAppSelector } from '../../../redux/hooks';
import { selectDisciplinePowers } from '../../../redux/reducers/masterdataReducer';

// types
import { Character } from '../../../types/Character';

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

interface APICharacterUpdateResponse {
  success: boolean;
  error?: string;
  character?: Character;
}

const CharacterSheet = () => {
  const { id } = useParams();
  const debounceRef = useRef<Map<string, ReturnType<typeof setTimeout>> | null>(
    new Map<string, ReturnType<typeof setTimeout>>()
  );
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
  const [savedCharacter, setSavedCharacter] = useState<Character | null>(null);
  const [currCharacter, setCurrCharacter] = useState<Character | null>(null);

  useEffect(() => {
    // on id change, try to update the character
    const fetchCharacter = async () => {
      setCurrCharacter(null);
      if (id) {
        const characterRequest = await fetch(`/api/vtmv5/character/${id}/`);
        if (characterRequest.ok) {
          const characterJson: APICharacterSheetResponse =
            await (characterRequest.json() as Promise<APICharacterSheetResponse>);
          if (characterJson.character) {
            setSavedCharacter(characterJson.character);
            setCurrCharacter(characterJson.character);
          }
        }
      }
    };
    // setup header text debounce
    debounceRef.current = new Map<string, ReturnType<typeof setTimeout>>();
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
                const debounceExempt = ['clan', 'predatorType'];
                setCurrCharacter({
                  ...currCharacter,
                  [field]: value,
                });
                // this is a simple debounce system
                // this will be used to prevent a network request occuring
                // every single time that someone makes a change
                if (debounceRef?.current?.has(field)) {
                  // clean up previous timeout
                  clearTimeout(debounceRef?.current?.get(field));
                  debounceRef?.current?.delete(field);
                }
                // create next timeout
                debounceRef?.current?.set(
                  field,
                  setTimeout(
                    () => {
                      // remove previous from map since it has triggered
                      debounceRef?.current?.delete(field);
                      // fire function
                      (async () => {
                        const updateResult = await fetch(
                          `/api/vtmv5/character/${currCharacter.id}/`,
                          {
                            method: 'PUT',
                            headers: {
                              Accept: 'application/json',
                              'Content-Type': 'application/json',
                            },
                            body: JSON.stringify({ [field]: value }),
                          }
                        );
                        if (updateResult.ok) {
                          const updateJson =
                            await (updateResult.json() as Promise<APICharacterUpdateResponse>);
                          if (updateJson.success && updateJson.character) {
                            setSavedCharacter(updateJson.character);
                          } else {
                            if (savedCharacter) {
                              setCurrCharacter({
                                ...savedCharacter,
                              });
                            } else {
                              setCurrCharacter(null);
                            }
                          }
                        }
                      })().catch(console.error);
                    },
                    debounceExempt.includes(field) ? 0 : 250
                  )
                );
              }
            }}
          />
          <AttributeSection
            attributes={currCharacter.attributes}
            onChange={(attribute, value) => {
              if (Object.keys(currCharacter.attributes).includes(attribute)) {
                setCurrCharacter({
                  ...currCharacter,
                  attributes: {
                    ...currCharacter.attributes,
                    [attribute]: value,
                  },
                });
                // fire network request
                (async () => {
                  const updateResult = await fetch(
                    `/api/vtmv5/character/attributes/${currCharacter.id}/`,
                    {
                      method: 'PUT',
                      headers: {
                        Accept: 'application/json',
                        'Content-Type': 'application/json',
                      },
                      body: JSON.stringify({ [attribute]: value }),
                    }
                  );
                  if (updateResult.ok) {
                    const updateJson =
                      await (updateResult.json() as Promise<APICharacterUpdateResponse>);
                    if (updateJson.success && updateJson.character) {
                      setSavedCharacter(updateJson.character);
                    } else {
                      if (savedCharacter) {
                        setCurrCharacter({
                          ...savedCharacter,
                        });
                      } else {
                        setCurrCharacter(null);
                      }
                    }
                  }
                })().catch(console.error);
              }
            }}
          />
          <SkillsSection
            skills={currCharacter.skills}
            onChange={(skill, skillData) => {
              if (Object.keys(currCharacter.skills).includes(skill)) {
                setCurrCharacter({
                  ...currCharacter,
                  skills: {
                    ...currCharacter.skills,
                    [skill]: skillData,
                  },
                });
                if (debounceRef?.current?.has(`skills_${skill}`)) {
                  // clean up previous timeout
                  clearTimeout(debounceRef?.current?.get(`skills_${skill}`));
                  debounceRef?.current?.delete(`skills_${skill}`);
                }
                // create next timeout
                debounceRef?.current?.set(
                  `skills_${skill}`,
                  setTimeout(() => {
                    // remove previous from map since it has triggered
                    debounceRef?.current?.delete(`skills_${skill}`);
                    // fire function
                    (async () => {
                      const updateResult = await fetch(
                        `/api/vtmv5/character/skills/${currCharacter.id}/`,
                        {
                          method: 'PUT',
                          headers: {
                            Accept: 'application/json',
                            'Content-Type': 'application/json',
                          },
                          body: JSON.stringify({ [skill]: skillData }),
                        }
                      );
                      if (updateResult.ok) {
                        const updateJson =
                          await (updateResult.json() as Promise<APICharacterUpdateResponse>);
                        if (updateJson.success && updateJson.character) {
                          setSavedCharacter(updateJson.character);
                        } else {
                          if (savedCharacter) {
                            setCurrCharacter({
                              ...savedCharacter,
                            });
                          } else {
                            setCurrCharacter(null);
                          }
                        }
                      }
                    })().catch(console.error);
                  }, 250)
                );
              }
            }}
          />
          <SecondarySection character={currCharacter} />
          <DisciplineSection
            characterId={currCharacter.id}
            levels={currCharacter.disciplines}
            powers={currCharacter.disciplinePowers}
            onLevelChange={(school, oldVal, newVal) => {
              if (Object.keys(currCharacter.disciplines).includes(school)) {
                setCurrCharacter({
                  ...currCharacter,
                  disciplines: {
                    ...currCharacter.disciplines,
                    [school]: newVal,
                  },
                });
              }
            }}
            onPowerChange={(oldVal, newVal, schoolChange) => {
              if (schoolChange) {
                // entire discipline changed
                console.log(oldVal, newVal, schoolChange);
              } else {
                // single ability changed
                let newPowers = [...currCharacter.disciplinePowers];
                if (newPowers.includes(oldVal) && newPowers.includes(newVal)) {
                  // it's just an order swap, so take no action
                  return;
                }
                if (newPowers.includes(oldVal)) {
                  // remove old value
                  newPowers = newPowers.filter((val) => val !== oldVal);
                }
                if (newVal !== '' && !newPowers.includes(newVal)) {
                  // add new value, if applicable
                  newPowers.push(newVal);
                }
                setCurrCharacter({
                  ...currCharacter,
                  disciplinePowers: newPowers.sort().sort((a, b) => {
                    const aInfo = disciplinePowers[a];
                    const bInfo = disciplinePowers[b];
                    if (aInfo.level < bInfo.level) {
                      return -1;
                    }
                    if (aInfo.level > bInfo.level) {
                      return 1;
                    }
                    return 0;
                  }),
                });
              }
            }}
          />
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
