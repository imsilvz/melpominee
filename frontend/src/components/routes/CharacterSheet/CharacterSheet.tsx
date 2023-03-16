import * as signalR from '@microsoft/signalr';
import React, { useEffect, useRef, useState } from 'react';
import { useParams } from 'react-router-dom';
import { cleanUpdate } from '../../../util/character';

// redux
import { useAppSelector } from '../../../redux/hooks';
import {
  selectDisciplines,
  selectDisciplinePowers,
} from '../../../redux/reducers/masterdataReducer';

// types
import {
  Character,
  CharacterAttributes,
  CharacterDisciplines,
  CharacterHeader,
  CharacterSecondaryStats,
  CharacterSkills,
  MeritBackgroundFlaw,
} from '../../../types/Character';

// local files
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import HeaderSection from './HeaderSection';
import AttributeSection from './AttributeSection';
import SkillsSection from './SkillsSection';
import SecondarySection from './SecondarySection';
import DisciplineSection from './DisciplineSection';
import BeliefsSection from './BeliefsSection';
import MeritFlawSection from './MeritFlawSection';
import TheBloodSection from './TheBloodSection';
import ProfileSection from './ProfileSection';
import './CharacterSheet.scss';

interface APICharacterSheetResponse {
  success: boolean;
  error?: string;
  character?: Character;
}

interface APICharacterUpdateResponse {
  success: boolean;
  error?: string;
  character?: CharacterHeader;
  attributes?: CharacterAttributes;
  skills?: CharacterSkills;
  disciplines?: CharacterDisciplines;
  powers?: string[];
}

const CharacterSheet = () => {
  const { id } = useParams();
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const debounceRef = useRef<Map<string, ReturnType<typeof setTimeout>> | null>(
    new Map<string, ReturnType<typeof setTimeout>>(),
  );
  const disciplines = useAppSelector(selectDisciplines);
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
  const [savedCharacter, setSavedCharacter] = useState<Character | null>(null);
  const [currCharacter, setCurrCharacter] = useState<Character | null>(null);

  useEffect(() => {
    if (id !== undefined) {
      const conn = new signalR.HubConnectionBuilder()
        .withUrl('/api/vtmv5/watch')
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Critical)
        .build();

      conn.on('onHeaderUpdate', (charId: number, update: CharacterHeader) => {
        console.log(`Header update for ${charId}`, update);
        const cleaned = cleanUpdate(update) as CharacterHeader;
        setCurrCharacter(
          (char) =>
            char && {
              ...char,
              ...cleaned,
            },
        );
        setSavedCharacter(
          (char) =>
            char && {
              ...char,
              ...cleaned,
            },
        );
      });

      conn.on('onAttributeUpdate', (charId: number, update: CharacterAttributes) => {
        console.log(`Attribute update for ${charId}`, update);
        const cleaned = cleanUpdate(update) as CharacterAttributes;
        setCurrCharacter(
          (char) =>
            char && {
              ...char,
              attributes: {
                ...char.attributes,
                ...cleaned,
              },
            },
        );
        setSavedCharacter(
          (char) =>
            char && {
              ...char,
              attributes: {
                ...char.attributes,
                ...cleaned,
              },
            },
        );
      });

      conn.on('onSkillUpdate', (charId: number, update: CharacterSkills) => {
        console.log(`Skill update for ${charId}`, update);
        const cleaned = cleanUpdate(update) as CharacterSkills;
        setCurrCharacter(
          (char) =>
            char && {
              ...char,
              skills: {
                ...char.skills,
                ...cleaned,
              },
            },
        );
        setSavedCharacter(
          (char) =>
            char && {
              ...char,
              skills: {
                ...char.skills,
                ...cleaned,
              },
            },
        );
      });

      conn
        .start()
        .then(async () => {
          console.log('Hello??');
          return conn.invoke('WatchCharacter', parseInt(id, 10));
        })
        .catch((err: Error) => {
          if (err.message !== 'The connection was stopped during negotiation.') {
            console.error(err);
          }
        });

      connectionRef.current = conn;
    }
    return () => {
      connectionRef.current?.stop().catch(console.error);
    };
  }, [id]);

  const updateHeader = (field: string, value: string) => {
    if (
      currCharacter &&
      Object.prototype.hasOwnProperty.call(currCharacter, field)
    ) {
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
        setTimeout(() => {
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
              },
            );
            if (updateResult.ok) {
              const updateJson =
                await (updateResult.json() as Promise<APICharacterUpdateResponse>);
              if (savedCharacter && updateJson.success && updateJson.character) {
                setSavedCharacter({
                  ...savedCharacter,
                  ...updateJson.character,
                });
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
        }, 250),
      );
    }
  };

  const updateGeneric = async <T extends APICharacterUpdateResponse>(
    endpoint: string,
    property: string,
    value: unknown,
    respProperty?: string,
  ) => {
    const result = await fetch(endpoint, {
      method: 'PUT',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(value),
    });
    if (result.ok) {
      const updateJson = await (result.json() as Promise<T>);
      if (
        savedCharacter &&
        updateJson.success &&
        updateJson[(respProperty || property) as keyof T]
      ) {
        setSavedCharacter({
          ...savedCharacter,
          [property]: {
            ...updateJson[(respProperty || property) as keyof T],
          },
        });
      } else {
        console.log(savedCharacter, updateJson.success, respProperty, property);
        if (savedCharacter) {
          setCurrCharacter({
            ...savedCharacter,
          });
        } else {
          setCurrCharacter(null);
        }
      }
    } else {
      console.log(savedCharacter, respProperty, property);
      if (savedCharacter) {
        setCurrCharacter({
          ...savedCharacter,
        });
      } else {
        setCurrCharacter(null);
      }
    }
  };

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
            onChange={(field, value) => updateHeader(field, value)}
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
                updateGeneric<APICharacterUpdateResponse>(
                  `/api/vtmv5/character/attributes/${currCharacter.id}/`,
                  'attributes',
                  { [attribute]: value },
                ).catch(console.error);
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
                    updateGeneric<APICharacterUpdateResponse>(
                      `/api/vtmv5/character/skills/${currCharacter.id}/`,
                      'skills',
                      { [skill]: skillData },
                    ).catch(console.error);
                  }, 250),
                );
              }
            }}
          />
          <SecondarySection
            character={currCharacter}
            onChangeHeaderField={(field, val) => updateHeader(field, val)}
            onChangeSecondaryStat={(field, val) => {
              setCurrCharacter({
                ...currCharacter,
                secondaryStats: {
                  ...currCharacter.secondaryStats,
                  [field as keyof CharacterSecondaryStats]: {
                    ...currCharacter.secondaryStats[
                      field as keyof CharacterSecondaryStats
                    ],
                    ...val,
                  },
                },
              });
              // fire network request
              updateGeneric<APICharacterUpdateResponse>(
                `/api/vtmv5/character/stats/${currCharacter.id}/`,
                'secondaryStats',
                { [field]: val },
                'stats',
              ).catch(console.error);
            }}
          />
          <DisciplineSection
            characterId={currCharacter.id}
            levels={currCharacter.disciplines}
            powers={currCharacter.disciplinePowers}
            onLevelChange={(school, oldVal, newVal) => {
              if (Object.keys(disciplines).includes(school)) {
                setCurrCharacter({
                  ...currCharacter,
                  disciplines: {
                    ...currCharacter.disciplines,
                    [school]: newVal,
                  },
                });
                // fire network request
                updateGeneric<APICharacterUpdateResponse>(
                  `/api/vtmv5/character/disciplines/${currCharacter.id}/`,
                  'disciplines',
                  { school, score: newVal },
                ).catch(console.error);
              }
            }}
            onPowerChange={(oldVal, newVal, schoolChange) => {
              if (schoolChange) {
                // entire discipline changed
                console.log(oldVal, newVal, schoolChange);
              } else {
                // single ability changed
                let newPowers = [...currCharacter.disciplinePowers];
                const changeData: { powerId: string; remove: boolean }[] = [];
                if (newPowers.includes(oldVal) && newPowers.includes(newVal)) {
                  // it's just an order swap, so take no action
                  return;
                }
                if (newPowers.includes(oldVal)) {
                  // remove old value
                  newPowers = newPowers.filter((val) => val !== oldVal);
                  changeData.push({ powerId: oldVal, remove: true });
                }
                if (newVal !== '' && !newPowers.includes(newVal)) {
                  // add new value, if applicable
                  newPowers.push(newVal);
                  changeData.push({ powerId: newVal, remove: false });
                }
                // update local state
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
                // configure payload and fire request
                if (changeData.length > 0) {
                  updateGeneric(
                    `/api/vtmv5/character/powers/${currCharacter.id}/`,
                    'disciplinePowers',
                    {
                      PowerIds: changeData,
                    },
                    'powers',
                  ).catch(console.error);
                }
              }
            }}
          />
          <BeliefsSection
            beliefs={currCharacter.beliefs}
            onChange={(field, val) => {
              if (Object.keys(currCharacter.beliefs).includes(field)) {
                setCurrCharacter({
                  ...currCharacter,
                  beliefs: {
                    ...currCharacter.beliefs,
                    [field]: val,
                  },
                });
                if (debounceRef?.current?.has(`beliefs_${field}`)) {
                  // clean up previous timeout
                  clearTimeout(debounceRef?.current?.get(`beliefs_${field}`));
                  debounceRef?.current?.delete(`beliefs_${field}`);
                }
                // create next timeout
                debounceRef?.current?.set(
                  `beliefs_${field}`,
                  setTimeout(() => {
                    // remove previous from map since it has triggered
                    debounceRef?.current?.delete(`beliefs_${field}`);
                    // fire network request
                    updateGeneric<APICharacterUpdateResponse>(
                      `/api/vtmv5/character/beliefs/${currCharacter.id}/`,
                      'beliefs',
                      { [field]: val },
                    ).catch(console.error);
                  }, 250),
                );
              }
            }}
          />
          <div className="charactersheet-panel-split">
            <div className="charactersheet-panel-split-column">
              <MeritFlawSection
                Backgrounds={currCharacter.backgrounds}
                Merits={currCharacter.merits}
                Flaws={currCharacter.flaws}
                onChange={(field, val) => {
                  if (Object.keys(currCharacter).includes(field)) {
                    const items = currCharacter[field as keyof Character];
                    const newItems = (items as MeritBackgroundFlaw[]).filter(
                      (item) => item.sortOrder !== val.sortOrder,
                    );
                    setCurrCharacter({
                      ...currCharacter,
                      [field]: [...newItems, val],
                    });
                    if (debounceRef?.current?.has(`${field}_${val.sortOrder}`)) {
                      // clean up previous timeout
                      clearTimeout(
                        debounceRef?.current?.get(`${field}_${val.sortOrder}`),
                      );
                      debounceRef?.current?.delete(`${field}_${val.sortOrder}`);
                    }
                    debounceRef?.current?.set(
                      `${field}_${val.sortOrder}`,
                      setTimeout(() => {
                        // remove previous from map since it has triggered
                        debounceRef?.current?.delete(`${field}_${val.sortOrder}`);
                        // fire network request
                        updateGeneric<APICharacterUpdateResponse>(
                          `/api/vtmv5/character/${field}/${currCharacter.id}/`,
                          field,
                          { [field]: [val] },
                        ).catch(console.error);
                      }, 250),
                    );
                  }
                }}
              />
            </div>
            <div className="charactersheet-panel-split-column">
              <TheBloodSection
                Clan={currCharacter.clan}
                BloodPotency={currCharacter.bloodPotency}
                XpSpent={currCharacter.xpSpent}
                XpTotal={currCharacter.xpTotal}
                onChange={(field, val) => updateHeader(field, val)}
              />
              <ProfileSection
                profile={currCharacter.profile}
                onChange={(field, val) => {
                  if (Object.keys(currCharacter.profile).includes(field)) {
                    setCurrCharacter({
                      ...currCharacter,
                      profile: {
                        ...currCharacter.profile,
                        [field]: val,
                      },
                    });
                    if (debounceRef?.current?.has(`profile_${field}`)) {
                      // clean up previous timeout
                      clearTimeout(debounceRef?.current?.get(`profile_${field}`));
                      debounceRef?.current?.delete(`profile_${field}`);
                    }
                    // create next timeout
                    debounceRef?.current?.set(
                      `profile_${field}`,
                      setTimeout(() => {
                        // remove previous from map since it has triggered
                        debounceRef?.current?.delete(`profile_${field}`);
                        // fire network request
                        updateGeneric<APICharacterUpdateResponse>(
                          `/api/vtmv5/character/profile/${currCharacter.id}/`,
                          'profile',
                          { [field]: val },
                        ).catch(console.error);
                      }, 250),
                    );
                  }
                }}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
