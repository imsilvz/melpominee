// Redux Imports
import { AnyAction } from 'redux';
import { ThunkAction } from 'redux-thunk';
import { BloodPotency } from '../../types/BloodPotency';

// Type Imports
import { Clan } from '../../types/Clan';
import { Discipline, DisciplinePower } from '../../types/Discipline';
import { PredatorType } from '../../types/PredatorType';
import { Resonance } from '../../types/Resonance';
import {
  setBloodPotencies,
  setClans,
  setDisciplinePowers,
  setDisciplines,
  setMasterdataLoaded,
  setPredatorTypes,
  setResonances,
} from '../reducers/masterdataReducer';

// State Imports
import { RootState } from '../store';

interface ClanResponse {
  success: boolean;
  error?: string;
  clans?: Clan[];
}

interface DisciplineResponse {
  success: boolean;
  error?: string;
  disciplines?: {
    [key: string]: DisciplinePower[];
  };
}

interface PredatorTypeResponse {
  success: boolean;
  error?: string;
  predatorTypes?: PredatorType[];
}

interface BloodPotencyResponse {
  success: boolean;
  error?: string;
  bloodPotencies?: {
    [key: number]: BloodPotency;
  };
}

interface ResonanceResponse {
  success: boolean;
  error?: string;
  resonances?: Resonance[];
}

export default (): ThunkAction<void, RootState, unknown, AnyAction> =>
  async (dispatch) => {
    // Make Requests
    const clanRequest = fetch(`/api/vtmv5/masterdata/clan/list`);
    const powerRequest = fetch(`/api/vtmv5/masterdata/discipline/list`);
    const predatorRequest = fetch(`/api/vtmv5/masterdata/predatortype/list`);
    const potencyRequest = fetch(`/api/vtmv5/masterdata/bloodpotency/list`);
    const resonanceRequest = fetch(`/api/vtmv5/masterdata/resonance/list`);
    const responses = await Promise.all([
      clanRequest,
      powerRequest,
      predatorRequest,
      potencyRequest,
      resonanceRequest,
    ]);

    // clans
    if (responses[0].ok) {
      const clanDict: { [key: string]: Clan } = {};
      const clanJson = await (responses[0].json() as Promise<ClanResponse>);
      if (clanJson.clans) {
        for (let i = 0; i < clanJson.clans?.length; i++) {
          const clan = clanJson.clans[i];
          clanDict[clan.id] = clan;
        }
        dispatch(setClans({ ...clanDict }));
      }
    }

    // disciplines
    if (responses[1].ok) {
      const powerDict: { [key: string]: DisciplinePower } = {};
      const disciplineDict: { [key: string]: Discipline } = {};
      const disciplineJson =
        await (responses[1].json() as Promise<DisciplineResponse>);
      if (disciplineJson.disciplines) {
        const disciplineKeys = Object.keys(disciplineJson.disciplines);
        for (let i = 0; i < disciplineKeys.length; i++) {
          const disciplineId = disciplineKeys[i];
          const disciplinePowers = disciplineJson.disciplines[disciplineId];
          const disciplineObject: Discipline = {
            id: disciplineId,
            name: disciplineId.replace(/([A-Z])/g, ' $1').trim(),
            powers: [],
          };
          for (let j = 0; j < disciplinePowers.length; j++) {
            powerDict[disciplinePowers[j].id] = disciplinePowers[j];
            disciplineObject.powers.push(disciplinePowers[j]);
          }
          disciplineDict[disciplineId] = disciplineObject;
        }
        dispatch(setDisciplines(disciplineDict));
        dispatch(setDisciplinePowers(powerDict));
      }
    }

    // predator types
    if (responses[2].ok) {
      const predatorDict: { [key: string]: PredatorType } = {};
      const predatorJson =
        await (responses[2].json() as Promise<PredatorTypeResponse>);
      if (predatorJson.predatorTypes) {
        for (let i = 0; i < predatorJson.predatorTypes.length; i++) {
          const predator = predatorJson.predatorTypes[i];
          predatorDict[predator.id] = predator;
        }
        dispatch(setPredatorTypes(predatorDict));
      }
    }

    // blood potency
    if (responses[3].ok) {
      const bpJson = await (responses[3].json() as Promise<BloodPotencyResponse>);
      if (bpJson.success && bpJson.bloodPotencies) {
        dispatch(setBloodPotencies(bpJson.bloodPotencies));
      }
    }

    // resonances
    if (responses[4].ok) {
      const resDict: { [key: string]: Resonance } = {};
      const resJson = await (responses[4].json() as Promise<ResonanceResponse>);
      if (resJson.success && resJson.resonances) {
        for (let i = 0; i < resJson.resonances?.length; i++) {
          const resonance = resJson.resonances[i];
          resDict[resonance.id] = resonance;
        }
        dispatch(setResonances(resDict));
      }
    }
    dispatch(setMasterdataLoaded(true));
  };
