import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';

export interface UserState {
  email: string;
  ready?: boolean;
}

const initialState: UserState = {
  email: '',
  ready: false,
};

export const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setUserdata: (state, action: PayloadAction<UserState>) => {
      state.email = action.payload.email;
      state.ready = true;
    },
  },
});
export const { setUserdata } = userSlice.actions;
export const selectUserEmail = (state: RootState) => state.user.email;
export const selectUserReady = (state: RootState) => state.user.ready;
export default userSlice.reducer;
