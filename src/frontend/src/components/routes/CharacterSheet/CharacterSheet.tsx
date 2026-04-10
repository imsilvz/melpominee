import * as signalR from '@microsoft/signalr';
import React, { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { handleUpdate } from '../../../util/character';
import type {
  CharacterCommandTypeString,
  UpdateOptions,
} from '../../../util/character';

// redux
import { useAppSelector } from '../../../redux/hooks';
import { selectDisciplinePowers } from '../../../redux/reducers/masterdataReducer';

// types
import type { Character } from '../../../types/Character';

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

const getCommandUpdateOpts = (type: string): UpdateOptions => {
  switch (type) {
    case 'powers':
      return {
        updateHandler: (char, payload) => {
          if (!char) return char;
          let powers = [...char.disciplinePowers];
          const { powerIds } = payload as {
            powerIds: { powerId: string; remove: boolean }[];
          };
          powerIds.forEach(({ powerId, remove }) => {
            if (remove) {
              powers = powers.filter((val) => val !== powerId);
            } else {
              powers.push(powerId);
            }
          });
          return { ...char, disciplinePowers: powers };
        },
      };
    case 'attributes':
      return { property: 'attributes' };
    case 'skills':
      return { property: 'skills' };
    case 'stats':
      return { property: 'secondaryStats' };
    case 'disciplines':
      return { property: 'disciplines' };
    case 'beliefs':
      return { property: 'beliefs' };
    case 'profile':
      return { property: 'profile' };
    // backgrounds, merits, and flaws merge at root level because
    // the data shape includes the field name: { backgrounds: { 0: {...} } }
    case 'backgrounds':
    case 'merits':
    case 'flaws':
    default:
      return {};
  }
};

const CharacterSheet = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
  const [currCharacter, setCurrCharacter] = useState<Character | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    if (id !== undefined) {
      const conn = new signalR.HubConnectionBuilder()
        .withUrl('/api/vtmv5/watch')
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Critical)
        .build();

      conn.on('WatcherUpdate', () => {
        // watchers list received; no action needed
      });

      conn.on(
        'onCharacterUpdate',
        (
          charId: number,
          updateId: string | null,
          timestamp: string,
          commands: { type: CharacterCommandTypeString; data: object }[],
        ) => {
          commands.forEach((cmd) => {
            const opts = getCommandUpdateOpts(cmd.type);
            handleUpdate(
              charId,
              updateId,
              { type: cmd.type, data: cmd.data },
              setCurrCharacter,
              opts,
            );
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
      setLoading(true);
      setCurrCharacter(null);
      if (id) {
        const characterRequest = await fetch(`/api/vtmv5/character/${id}/`);
        if (characterRequest.ok) {
          const characterJson: APICharacterSheetResponse =
            await (characterRequest.json() as Promise<APICharacterSheetResponse>);
          if (characterJson.success && characterJson.character) {
            setLoading(false);
            setCurrCharacter(characterJson.character);
          }
        } else if (characterRequest.status === 403) {
          // not authorized!
          setLoading(false);
          navigate('/', { replace: true });
        }
      }
    };
    fetchCharacter().catch(console.error);
  }, [navigate, id]);

  useEffect(() => {
    const prevTitle = document.title;
    if (currCharacter) {
      document.title = `Melpominee - ${currCharacter.name || ''}`;
    }
    return () => {
      document.title = prevTitle;
    };
  }, [currCharacter]);

  return (
    <div className="charactersheet-container">
      {loading ? (
        <LoadingSpinner />
      ) : (
        // eslint-disable-next-line react/jsx-no-useless-fragment
        <>
          {!currCharacter ? (
            <div className="charactersheet-panel">
              <p>hello</p>
            </div>
          ) : (
            <div className="charactersheet-panel">
              <HeaderSection
                character={currCharacter}
                onChange={(field, value) =>
                  handleUpdate(
                    currCharacter.id,
                    null,
                    { type: 'header', data: { [field]: value } },
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
                    currCharacter.id,
                    null,
                    { type: 'attributes', data: { [attribute]: value } },
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
                    currCharacter.id,
                    null,
                    { type: 'skills', data: { [skill]: value } },
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
                    currCharacter.id,
                    null,
                    { type: 'header', data: { [field]: value } },
                    setCurrCharacter,
                  )
                }
                onChangeSecondaryStat={(field, value) =>
                  handleUpdate(
                    currCharacter.id,
                    null,
                    { type: 'stats', data: { [field]: value } },
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
                    currCharacter.id,
                    null,
                    { type: 'disciplines', data: { [school]: newVal } },
                    setCurrCharacter,
                    {
                      property: 'disciplines',
                    },
                  )
                }
                onPowerChange={(oldVal, newVal, schoolChange) => {
                  if (schoolChange) {
                    // entire discipline changed — not yet implemented
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
                      currCharacter.id,
                      null,
                      { type: 'powers', data: { disciplinePowers: newPowers } },
                      setCurrCharacter,
                      {
                        apiPayload: {
                          powerIds: changeData,
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
                    currCharacter.id,
                    null,
                    { type: 'beliefs', data: { [field]: value } },
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
                        currCharacter.id,
                        null,
                        {
                          type: field as CharacterCommandTypeString,
                          data: { [field]: { [value.sortOrder]: value } },
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
                        currCharacter.id,
                        null,
                        { type: 'header', data: { [field]: value } },
                        setCurrCharacter,
                      )
                    }
                  />
                  <ProfileSection
                    profile={currCharacter.profile}
                    onChange={(field, value) =>
                      handleUpdate(
                        currCharacter.id,
                        null,
                        { type: 'profile', data: { [field]: value } },
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
        </>
      )}
    </div>
  );
};
export default CharacterSheet;
