import * as signalR from '@microsoft/signalr';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { handleUpdate } from '../../../util/character';
import type {
  CharacterCommandTypeString,
  UpdateOptions,
} from '../../../util/character';

// redux
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import { selectDisciplinePowers } from '../../../redux/reducers/masterdataReducer';
import {
  resetConnection,
  selectManualRetryTick,
  setConnectionStatus,
} from '../../../redux/reducers/connectionReducer';

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

// Tuned for ~3s backend cold starts: recover fast in the first 16s, then ease off.
// Replaces the default SignalR reconnect schedule (0, 2s, 10s, 30s, 30s, ...).
const retryPolicy: signalR.IRetryPolicy = {
  nextRetryDelayInMilliseconds: (ctx: signalR.RetryContext): number | null => {
    const schedule = [500, 1000, 2000, 4000, 8000];
    return ctx.previousRetryCount < schedule.length
      ? schedule[ctx.previousRetryCount]
      : 15000;
  },
};

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
  const dispatch = useAppDispatch();
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
  const manualRetryTick = useAppSelector(selectManualRetryTick);
  const [currCharacter, setCurrCharacter] = useState<Character | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  // Extracted from the id-change effect so that onreconnected() can call it too.
  // Memoized on [id, navigate] so effect deps stay stable.
  const fetchCharacter = useCallback(async (): Promise<void> => {
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
  }, [id, navigate]);

  useEffect(() => {
    // Scoped per-effect-run so each connection's onclose handler only sees
    // the flag set by its own cleanup. On manual retry, the cleanup flips
    // this true before calling .stop(); without it, the async onclose fires
    // after the new connection has already been built and flickers the
    // banner through 'disconnected' at the wrong moment.
    let isIntentionalStop = false;
    if (id !== undefined) {
      const conn = new signalR.HubConnectionBuilder()
        .withUrl('/api/vtmv5/watch')
        .withAutomaticReconnect(retryPolicy)
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

      // Reconnect lifecycle handlers MUST be registered before .start().
      // The reducer owns the `attempt` counter so handlers only dispatch status.
      conn.onreconnecting((err?: Error) => {
        dispatch(
          setConnectionStatus({
            status: 'reconnecting',
            lastError: err?.message ?? null,
          }),
        );
      });

      const handleReconnected = async (): Promise<void> => {
        try {
          // CRITICAL: backend in-process group state is lost on pod restart.
          // Must re-invoke WatchCharacter to rejoin the character_{id} group.
          await conn.invoke('WatchCharacter', parseInt(id, 10));
          // Belt-and-braces: refetch character state to catch updates missed
          // during the reconnect gap.
          await fetchCharacter();
          dispatch(
            setConnectionStatus({
              status: 'connected',
              lastError: null,
            }),
          );
        } catch (e) {
          dispatch(
            setConnectionStatus({
              status: 'disconnected',
              lastError: (e as Error).message,
            }),
          );
        }
      };
      conn.onreconnected(() => {
        // SignalR's onreconnected expects (connectionId?: string) => void.
        // Fire-and-forget the async recovery; errors are handled internally.
        handleReconnected().catch(console.error);
      });

      conn.onclose((err?: Error) => {
        if (isIntentionalStop) {
          return;
        }
        dispatch(
          setConnectionStatus({
            status: 'disconnected',
            lastError: err?.message ?? null,
          }),
        );
      });

      conn
        .start()
        .then(async () => {
          await conn.invoke('WatchCharacter', parseInt(id, 10));
          dispatch(
            setConnectionStatus({
              status: 'connected',
              lastError: null,
            }),
          );
          return undefined;
        })
        .catch((err: Error) => {
          if (err.message !== 'The connection was stopped during negotiation.') {
            console.error(err);
            dispatch(
              setConnectionStatus({
                status: 'disconnected',
                lastError: err.message,
              }),
            );
          }
        });

      connectionRef.current = conn;
    }
    return () => {
      isIntentionalStop = true;
      connectionRef.current?.stop().catch(console.error);
      // Clear banner state when leaving the character sheet so a stale
      // reconnecting/disconnected status doesn't bleed into other routes.
      dispatch(resetConnection());
    };
    // manualRetryTick is included so that requestManualRetry() re-runs this
    // effect: the cleanup stops the failed connection, then the body builds
    // and starts a fresh one.
  }, [id, manualRetryTick, dispatch, fetchCharacter]);

  useEffect(() => {
    // on id change, try to update the character
    fetchCharacter().catch(console.error);
  }, [fetchCharacter]);

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
