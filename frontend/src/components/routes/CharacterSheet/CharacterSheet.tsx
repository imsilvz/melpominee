import * as signalR from '@microsoft/signalr';
import React, { useEffect, useRef, useState } from 'react';
import { useParams } from 'react-router-dom';
import { cleanUpdate, handleUpdate } from '../../../util/character';

// redux
import { useAppSelector } from '../../../redux/hooks';
import { selectDisciplinePowers } from '../../../redux/reducers/masterdataReducer';

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

const CharacterSheet = () => {
  const { id } = useParams();
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
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
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: CharacterHeader,
        ) => {
          const cleaned = cleanUpdate(update) as CharacterHeader;
          console.log(updateId, timestamp, `Header update for ${charId}`, cleaned);
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter);
        },
      );

      conn.on(
        'onAttributeUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: CharacterAttributes,
        ) => {
          const cleaned = cleanUpdate(update) as CharacterAttributes;
          console.log(
            updateId,
            timestamp,
            `Attribute update for ${charId}`,
            cleaned,
          );
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            property: 'attributes',
          });
        },
      );

      conn.on(
        'onSkillUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: CharacterSkills,
        ) => {
          const cleaned = cleanUpdate(update) as CharacterSkills;
          console.log(updateId, timestamp, `Skill update for ${charId}`, cleaned);
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            property: 'skills',
          });
        },
      );

      conn.on(
        'onSecondaryUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: CharacterSecondaryStats,
        ) => {
          const cleaned = cleanUpdate(update) as CharacterSecondaryStats;
          console.log(
            updateId,
            timestamp,
            `Secondary Stat update for ${charId}`,
            cleaned,
          );
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            property: 'secondaryStats',
          });
        },
      );

      conn.on(
        'onDisciplineUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: { school: string; score: number },
        ) => {
          const cleaned = cleanUpdate(update) as CharacterDisciplines;
          console.log(
            updateId,
            timestamp,
            `Discipline update for ${charId}`,
            cleaned,
          );
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            property: 'disciplines',
          });
        },
      );

      conn.on(
        'onPowersUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: {
            powerIds: {
              powerId: string;
              remove: boolean;
            }[];
          },
        ) => {
          const cleaned = cleanUpdate(update) as CharacterDisciplines;
          console.log(updateId, timestamp, `Powers update for ${charId}`, cleaned);
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            updateHandler: (char, payload) => {
              if (!char) {
                return char;
              }
              // build update
              let powers = [...char.disciplinePowers];
              const { powerIds } = payload as {
                powerIds: {
                  powerId: string;
                  remove: boolean;
                }[];
              };
              powerIds.forEach(({ powerId, remove }) => {
                if (remove) {
                  powers = powers.filter((val) => val !== powerId);
                } else {
                  powers.push(powerId);
                }
              });
              console.log(char.disciplinePowers, powers);
              return {
                ...char,
                disciplinePowers: powers,
              };
            },
          });
        },
      );

      conn.on(
        'onBeliefsUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: CharacterBeliefs,
        ) => {
          const cleaned = cleanUpdate(update) as CharacterBeliefs;
          console.log(updateId, timestamp, `Beliefs update for ${charId}`, cleaned);
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            property: 'beliefs',
          });
        },
      );

      conn.on(
        'onBackgroundMeritFlawUpdate',
        (
          charId: number,
          updateId: string | null,
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
          console.log(updateId, timestamp, `BMF update for ${charId}`, cleaned);
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter);
        },
      );

      conn.on(
        'onProfileUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          update: CharacterProfile,
        ) => {
          const cleaned = cleanUpdate(update) as CharacterProfile;
          console.log(updateId, timestamp, `Profile update for ${charId}`, cleaned);
          handleUpdate(null, charId, updateId, cleaned, setCurrCharacter, {
            property: 'profile',
          });
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
            setCurrCharacter(characterJson.character);
          }
        }
      }
    };
    // setup header text debounce
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
                currCharacter.id,
                null,
                { [field]: value },
                setCurrCharacter,
                {
                  debounceOptions: {
                    enable: true,
                    delay: 100,
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
                currCharacter.id,
                null,
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
                currCharacter.id,
                null,
                { [skill]: value },
                setCurrCharacter,
                {
                  property: 'skills',
                  debounceOptions: {
                    enable: true,
                    delay: 100,
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
                currCharacter.id,
                null,
                { [field]: value },
                setCurrCharacter,
              )
            }
            onChangeSecondaryStat={(field, value) =>
              handleUpdate(
                `/api/vtmv5/character/stats/${currCharacter.id}/`,
                currCharacter.id,
                null,
                { [field]: value },
                setCurrCharacter,
                {
                  property: 'secondaryStats',
                  debounceOptions: {
                    enable: true,
                    delay: 300,
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
                currCharacter.id,
                null,
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
                newPowers = newPowers.sort().sort((a, b) => {
                  const aInfo = disciplinePowers[a];
                  const bInfo = disciplinePowers[b];
                  if (aInfo.level < bInfo.level) {
                    return -1;
                  }
                  if (aInfo.level > bInfo.level) {
                    return 1;
                  }
                  return 0;
                });
                handleUpdate(
                  `/api/vtmv5/character/powers/${currCharacter.id}/`,
                  currCharacter.id,
                  null,
                  { disciplinePowers: newPowers },
                  setCurrCharacter,
                  {
                    apiPayload: {
                      PowerIds: changeData,
                    },
                  },
                );
              }
            }}
          />
          <BeliefsSection
            beliefs={currCharacter.beliefs}
            onChange={(field, value) =>
              handleUpdate(
                `/api/vtmv5/character/beliefs/${currCharacter.id}/`,
                currCharacter.id,
                null,
                { [field]: value },
                setCurrCharacter,
                {
                  property: 'beliefs',
                  debounceOptions: {
                    enable: true,
                    delay: 100,
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
                    currCharacter.id,
                    null,
                    {
                      [field]: {
                        [value.sortOrder]: value,
                      },
                    },
                    setCurrCharacter,
                    {
                      debounceOptions: {
                        enable: true,
                        delay: 100,
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
                    currCharacter.id,
                    null,
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
                    currCharacter.id,
                    null,
                    { [field]: value },
                    setCurrCharacter,
                    {
                      property: 'profile',
                      debounceOptions: {
                        enable: true,
                        delay: 100,
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
