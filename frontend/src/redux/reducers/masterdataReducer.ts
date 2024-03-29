import { createSlice, PayloadAction } from '@reduxjs/toolkit';

// type imports
import { BloodPotency } from '../../types/BloodPotency';
import { Clan } from '../../types/Clan';
import { Discipline, DisciplinePower } from '../../types/Discipline';
import { PredatorType } from '../../types/PredatorType';
import { Resonance } from '../../types/Resonance';

// state imports
import type { RootState } from '../store';

interface BloodPotencyMasterdata {
  [key: number]: BloodPotency;
}

interface ClanMasterdata {
  [key: string]: Clan;
}

interface DisciplineMasterdata {
  [key: string]: Discipline;
}

interface DisciplinePowerMasterdata {
  [key: string]: DisciplinePower;
}

interface PredatorTypeMasterdata {
  [key: string]: PredatorType;
}

interface ResonanceMasterdata {
  [key: string]: Resonance;
}

export interface MasterdataState {
  bloodPotencies: BloodPotencyMasterdata;
  clans: ClanMasterdata;
  disciplines: DisciplineMasterdata;
  disciplinePowers: DisciplinePowerMasterdata;
  predatorTypes: PredatorTypeMasterdata;
  resonances: ResonanceMasterdata;
  loaded: boolean;
}

const initialState: MasterdataState = {
  bloodPotencies: {},
  clans: {},
  disciplines: {},
  disciplinePowers: {},
  predatorTypes: {},
  resonances: {},
  loaded: false,
};

export const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setBloodPotencies: (state, action: PayloadAction<BloodPotencyMasterdata>) => {
      state.bloodPotencies = action.payload;
    },
    setClans: (state, action: PayloadAction<ClanMasterdata>) => {
      state.clans = action.payload;
    },
    setDisciplines: (state, action: PayloadAction<DisciplineMasterdata>) => {
      // sort for later use
      Object.keys(action.payload).forEach((key) => {
        const powerList = action.payload[key].powers;
        action.payload[key].powers = powerList.sort().sort((a, b) => {
          if (a.level < b.level) {
            return -1;
          }
          if (a.level > b.level) {
            return 1;
          }
          return 0;
        });
      });
      state.disciplines = action.payload;
    },
    setDisciplinePowers: (
      state,
      action: PayloadAction<DisciplinePowerMasterdata>,
    ) => {
      state.disciplinePowers = action.payload;
    },
    setPredatorTypes: (state, action: PayloadAction<PredatorTypeMasterdata>) => {
      state.predatorTypes = action.payload;
    },
    setResonances: (state, action: PayloadAction<ResonanceMasterdata>) => {
      state.resonances = action.payload;
    },
    setMasterdataLoaded: (state, action: PayloadAction<boolean>) => {
      state.loaded = action.payload;
    },
  },
});
export const {
  setBloodPotencies,
  setClans,
  setDisciplines,
  setDisciplinePowers,
  setPredatorTypes,
  setResonances,
  setMasterdataLoaded,
} = userSlice.actions;
export const selectBloodPotencies = (state: RootState) =>
  state.masterdata.bloodPotencies;
export const selectClans = (state: RootState) => state.masterdata.clans;
export const selectDisciplines = (state: RootState) => state.masterdata.disciplines;
export const selectDisciplinePowers = (state: RootState) =>
  state.masterdata.disciplinePowers;
export const selectPredatorTypes = (state: RootState) =>
  state.masterdata.predatorTypes;
export const selectResonances = (state: RootState) => state.masterdata.resonances;
export const selectMasterdataLoaded = (state: RootState) => state.masterdata.loaded;
export default userSlice.reducer;
