import {
  AnyAction,
  combineReducers,
  configureStore,
  ThunkAction,
} from '@reduxjs/toolkit';

import masterdata from './reducers/masterdataReducer';
import user from './reducers/userReducer';
import tooltip from './reducers/tooltipReducer';

const store = configureStore({
  reducer: combineReducers({
    masterdata,
    tooltip,
    user,
  }),
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  AnyAction
>;
export default store;
