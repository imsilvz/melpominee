// Redux Imports
import { AnyAction, ThunkAction } from '@reduxjs/toolkit';

// State Imports
import { RootState } from '../store';
import { setUserdata, UserState } from '../reducers/userReducer';

export default (): ThunkAction<void, RootState, unknown, AnyAction> =>
  async (dispatch) => {
    // Make Requests
    const authRequest = await fetch(`/api/auth/`);
    // await Promise.all([authRequest]);
    if (authRequest.ok) {
      const user: UserState = await (authRequest.json() as Promise<UserState>);
      dispatch(
        setUserdata({
          ...user,
        }),
      );
    }
  };
