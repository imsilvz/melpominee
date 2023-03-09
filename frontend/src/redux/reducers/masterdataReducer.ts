import { createSlice, PayloadAction } from '@reduxjs/toolkit';

// type imports
import { Clan } from '../../types/Clan';
import { Discipline, DisciplinePower } from '../../types/Discipline';
import { PredatorType } from '../../types/PredatorType';

// state imports
import type { RootState } from '../store';

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

export interface MasterdataState {
  clans: ClanMasterdata;
  disciplines: DisciplineMasterdata;
  disciplinePowers: DisciplinePowerMasterdata;
  predatorTypes: PredatorTypeMasterdata;
}

const initialState: MasterdataState = {
  clans: {},
  disciplines: {},
  disciplinePowers: {},
  predatorTypes: {},
};

export const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
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
      action: PayloadAction<DisciplinePowerMasterdata>
    ) => {
      state.disciplinePowers = action.payload;
    },
    setPredatorTypes: (
      state,
      action: PayloadAction<PredatorTypeMasterdata>
    ) => {
      state.predatorTypes = action.payload;
    },
  },
});
export const {
  setClans,
  setDisciplines,
  setDisciplinePowers,
  setPredatorTypes,
} = userSlice.actions;
export const selectClans = (state: RootState) => state.masterdata.clans;
export const selectDisciplines = (state: RootState) =>
  state.masterdata.disciplines;
export const selectDisciplinePowers = (state: RootState) =>
  state.masterdata.disciplinePowers;
export const selectPredatorTypes = (state: RootState) =>
  state.masterdata.predatorTypes;
export default userSlice.reducer;
