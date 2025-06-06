import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';

export interface UserState {
  tooltipType: string;
  tooltipId: string;
}

const initialState: UserState = {
  tooltipType: '',
  tooltipId: '',
};

export const tooltipSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setTooltipData: (state, action: PayloadAction<UserState>) => {
      state.tooltipType = action.payload.tooltipType;
      state.tooltipId = action.payload.tooltipId;
    },
  },
});
export const { setTooltipData } = tooltipSlice.actions;
export const selectTooltipType = (state: RootState) => state.tooltip.tooltipType;
export const selectTooltipId = (state: RootState) => state.tooltip.tooltipId;
export default tooltipSlice.reducer;
