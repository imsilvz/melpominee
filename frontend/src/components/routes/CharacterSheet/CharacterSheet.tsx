import * as signalR from '@microsoft/signalr';
import React, { useEffect, useRef, useState } from 'react';
import { useParams } from 'react-router-dom';
import { cleanUpdate, handleUpdate } from '../../../util/character';

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
  CharacterBeliefs,
  CharacterDisciplines,
  CharacterHeader,
  CharacterProfile,
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

      conn.on(
        'onHeaderUpdate',
        (charId: number, timestamp: string, update: CharacterHeader) => {
          const cleaned = cleanUpdate(update) as CharacterHeader;
          console.log(timestamp, `Header update for ${charId}`, cleaned);
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
        },
      );

      conn.on(
        'onAttributeUpdate',
        (charId: number, timestamp: string, update: CharacterAttributes) => {
          const cleaned = cleanUpdate(update) as CharacterAttributes;
          console.log(timestamp, `Attribute update for ${charId}`, cleaned);
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
        },
      );

      conn.on(
        'onSkillUpdate',
        (charId: number, timestamp: string, update: CharacterSkills) => {
          const cleaned = cleanUpdate(update) as CharacterSkills;
          console.log(timestamp, `Skill update for ${charId}`, cleaned);
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
        },
      );

      conn.on(
        'onSecondaryUpdate',
        (charId: number, timestamp: string, update: CharacterSecondaryStats) => {
          const cleaned = cleanUpdate(update) as CharacterSecondaryStats;
          console.log(timestamp, `Secondary Stat update for ${charId}`, cleaned);
          setCurrCharacter((char) => {
            // handle null
            if (!char) {
              return char;
            }
            // we need to build a suitable object to handle this update
            const newSecondaries = {
              ...char.secondaryStats,
            };
            Object.keys(cleaned).forEach((statName) => {
              newSecondaries[statName as keyof CharacterSecondaryStats] = {
                ...char.secondaryStats[statName as keyof CharacterSecondaryStats],
                ...cleaned[statName as keyof CharacterSecondaryStats],
              };
            });
            return {
              ...char,
              secondaryStats: newSecondaries,
            };
          });
        },
      );

      conn.on(
        'onDisciplineUpdate',
        (
          charId: number,
          timestamp: string,
          update: { school: string; score: number },
        ) => {
          console.log(timestamp, `Discipline update for ${charId}`, update);
          setCurrCharacter(
            (char) =>
              char && {
                ...char,
                disciplines: {
                  ...char.disciplines,
                  [update.school as keyof CharacterDisciplines]: update.score,
                },
              },
          );
          setSavedCharacter(
            (char) =>
              char && {
                ...char,
                disciplines: {
                  ...char.disciplines,
                  [update.school as keyof CharacterDisciplines]: update.score,
                },
              },
          );
        },
      );

      conn.on(
        'onBeliefsUpdate',
        (charId: number, timestamp: string, update: CharacterBeliefs) => {
          const cleaned = cleanUpdate(update) as CharacterBeliefs;
          console.log(timestamp, `Beliefs update for ${charId}`, cleaned);
          setCurrCharacter(
            (char) =>
              char && {
                ...char,
                beliefs: {
                  ...char.beliefs,
                  ...cleaned,
                },
              },
          );
          setSavedCharacter(
            (char) =>
              char && {
                ...char,
                beliefs: {
                  ...char.beliefs,
                  ...cleaned,
                },
              },
          );
        },
      );

      conn.on(
        'onBackgroundMeritFlawUpdate',
        (
          charId: number,
          timestamp: string,
          update: {
            backgrounds: {
              [key: number]: MeritBackgroundFlaw;
            };
            merits: {
              [key: number]: MeritBackgroundFlaw;
            };
            flaws: {
              [key: number]: MeritBackgroundFlaw;
            };
          },
        ) => {
          const cleaned = cleanUpdate(update) as Character;
          console.log(timestamp, `BMF update for ${charId}`, cleaned);
          Object.keys(cleaned).forEach((name) => {
            const nameUpdates = cleaned[name as keyof Character] as {
              [key: number]: MeritBackgroundFlaw;
            };
            setCurrCharacter((char) => {
              if (!char) {
                return char;
              }
              // insert into state
              return {
                ...char,
                [name as keyof Character]: {
                  ...(char[name as keyof Character] as {
                    [key: number]: MeritBackgroundFlaw;
                  }),
                  ...nameUpdates,
                },
              };
            });
          });
        },
      );

      conn.on(
        'onProfileUpdate',
        (charId: number, timestamp: string, update: CharacterProfile) => {
          const cleaned = cleanUpdate(update) as CharacterProfile;
          console.log(timestamp, `Profile update for ${charId}`, cleaned);
          setCurrCharacter(
            (char) =>
              char && {
                ...char,
                profile: {
                  ...char.profile,
                  ...cleaned,
                },
              },
          );
          setSavedCharacter(
            (char) =>
              char && {
                ...char,
                profile: {
                  ...char.profile,
                  ...cleaned,
                },
              },
          );
        },
      );

      conn
        .start()
        .then(async () => {
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
            onChange={(field, value) =>
              handleUpdate(
                `/api/vtmv5/character/${currCharacter.id}/`,
                currCharacter,
                { [field]: value },
                setCurrCharacter,
                {
                  debounceOptions: {
                    enable: true,
                    delay: 250,
                  },
                },
              )
            }
          />
          <AttributeSection
            attributes={currCharacter.attributes}
            onChange={(attribute, value) =>
              handleUpdate(
                `/api/vtmv5/character/attributes/${currCharacter.id}/`,
                currCharacter,
                { [attribute]: value },
                setCurrCharacter,
                {
                  property: 'attributes',
                },
              )
            }
          />
          <SkillsSection
            skills={currCharacter.skills}
            onChange={(skill, value) =>
              handleUpdate(
                `/api/vtmv5/character/skills/${currCharacter.id}/`,
                currCharacter,
                { [skill]: value },
                setCurrCharacter,
                {
                  property: 'skills',
                  debounceOptions: {
                    enable: true,
                    delay: 250,
                  },
                },
              )
            }
          />
          <SecondarySection
            character={currCharacter}
            onChangeHeaderField={(field, value) =>
              handleUpdate(
                `/api/vtmv5/character/${currCharacter.id}/`,
                currCharacter,
                { [field]: value },
                setCurrCharacter,
              )
            }
            onChangeSecondaryStat={(field, value) =>
              handleUpdate(
                `/api/vtmv5/character/stats/${currCharacter.id}/`,
                currCharacter,
                { [field]: value },
                setCurrCharacter,
                {
                  property: 'secondaryStats',
                  debounceOptions: {
                    enable: true,
                    delay: 250,
                  },
                },
              )
            }
          />
          <DisciplineSection
            characterId={currCharacter.id}
            levels={currCharacter.disciplines}
            powers={currCharacter.disciplinePowers}
            onLevelChange={(school, oldVal, newVal) =>
              handleUpdate(
                `/api/vtmv5/character/disciplines/${currCharacter.id}/`,
                currCharacter,
                { [school]: newVal },
                setCurrCharacter,
                {
                  property: 'disciplines',
                },
              )
            }
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
            onChange={(field, value) =>
              handleUpdate(
                `/api/vtmv5/character/beliefs/${currCharacter.id}/`,
                currCharacter,
                { [field]: value },
                setCurrCharacter,
                {
                  property: 'beliefs',
                  debounceOptions: {
                    enable: true,
                    delay: 250,
                  },
                },
              )
            }
          />
          <div className="charactersheet-panel-split">
            <div className="charactersheet-panel-split-column">
              <MeritFlawSection
                Backgrounds={currCharacter.backgrounds}
                Merits={currCharacter.merits}
                Flaws={currCharacter.flaws}
                onChange={(field, value) =>
                  handleUpdate(
                    `/api/vtmv5/character/${field}/${currCharacter.id}/`,
                    currCharacter,
                    {
                      [field]: {
                        [value.sortOrder]: value,
                      },
                    },
                    setCurrCharacter,
                    {
                      debounceOptions: {
                        enable: true,
                        delay: 250,
                      },
                    },
                  )
                }
              />
            </div>
            <div className="charactersheet-panel-split-column">
              <TheBloodSection
                Clan={currCharacter.clan}
                BloodPotency={currCharacter.bloodPotency}
                XpSpent={currCharacter.xpSpent}
                XpTotal={currCharacter.xpTotal}
                onChange={(field, value) =>
                  handleUpdate(
                    `/api/vtmv5/character/${currCharacter.id}/`,
                    currCharacter,
                    { [field]: value },
                    setCurrCharacter,
                  )
                }
              />
              <ProfileSection
                profile={currCharacter.profile}
                onChange={(field, value) =>
                  handleUpdate(
                    `/api/vtmv5/character/profile/${currCharacter.id}/`,
                    currCharacter,
                    { [field]: value },
                    setCurrCharacter,
                    {
                      property: 'profile',
                      debounceOptions: {
                        enable: true,
                        delay: 250,
                      },
                    },
                  )
                }
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
export default CharacterSheet;
