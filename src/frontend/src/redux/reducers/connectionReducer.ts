import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';

export type ConnectionStatus =
  | 'idle'
  | 'connected'
  | 'reconnecting'
  | 'disconnected';

interface ConnectionState {
  status: ConnectionStatus;
  attempt: number;
  lastError: string | null;
  manualRetryTick: number; // bumped by requestManualRetry()
}

const initialState: ConnectionState = {
  status: 'idle',
  attempt: 0,
  lastError: null,
  manualRetryTick: 0,
};

export const connectionSlice = createSlice({
  name: 'connection',
  initialState,
  reducers: {
    setConnectionStatus: (
      state,
      action: PayloadAction<{
        status: ConnectionStatus;
        lastError?: string | null;
      }>,
    ) => {
      state.status = action.payload.status;
      if (action.payload.status === 'reconnecting') {
        state.attempt += 1;
      } else if (action.payload.status === 'connected') {
        state.attempt = 0;
      }
      if (action.payload.lastError !== undefined) {
        state.lastError = action.payload.lastError;
      }
    },
    requestManualRetry: (state) => {
      state.manualRetryTick += 1;
    },
    resetConnection: () => initialState,
  },
});

export const { setConnectionStatus, requestManualRetry, resetConnection } =
  connectionSlice.actions;

export const selectConnectionStatus = (state: RootState) => state.connection.status;
export const selectConnectionAttempt = (state: RootState) =>
  state.connection.attempt;
export const selectConnectionLastError = (state: RootState) =>
  state.connection.lastError;
export const selectManualRetryTick = (state: RootState) =>
  state.connection.manualRetryTick;

export default connectionSlice.reducer;
