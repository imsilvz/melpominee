import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';

export interface UserState {
  email: string;
  role: string;
  ready?: boolean;
}

const initialState: UserState = {
  email: '',
  role: 'user',
  ready: false,
};

export const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setUserdata: (state, action: PayloadAction<UserState>) => {
      state.email = action.payload.email;
      state.role = action.payload.role;
      state.ready = true;
    },
  },
});
export const { setUserdata } = userSlice.actions;
export const selectUserEmail = (state: RootState) => state.user.email;
export const selectUserRole = (state: RootState) => state.user.role;
export const selectUserReady = (state: RootState) => state.user.ready;
export default userSlice.reducer;
