// Redux Imports
import { AnyAction } from 'redux';
import { ThunkAction } from 'redux-thunk';

// State Imports
import { RootState } from '../store';

export default (): ThunkAction<void, RootState, unknown, AnyAction> =>
  async (dispatch) => {
    // Make Requests
    const clanRequest = fetch(`/api/vtmv5/masterdata/clan/list`);
    const powerRequest = fetch(`/api/vtmv5/masterdata/discipline/list`);
    const predatorRequest = fetch(`/api/vtmv5/masterdata/predatortype/list`);
    const responses = await Promise.all([
      clanRequest,
      powerRequest,
      predatorRequest,
    ]);
    console.log(responses);
  };
