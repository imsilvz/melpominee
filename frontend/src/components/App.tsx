import React, { useEffect } from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

// redux
import { useAppDispatch, useAppSelector } from '../redux/hooks';
import initialThunk from '../redux/thunk/initial';
import masterdataThunk from '../redux/thunk/masterdata';
import { selectUserReady, selectUserEmail } from '../redux/reducers/userReducer';
import { selectMasterdataLoaded } from '../redux/reducers/masterdataReducer';

// local files
import Layout from './shared/Layout';
import Login from './routes/Auth/Login';
import ForgotPassword from './routes/Auth/ForgotPassword';
import Register from './routes/Auth/Register';
import RequireAuth from './shared/Auth/RequireAuth';
import TooltipProvider from './shared/Tooltip/TooltipProvider';
import CharacterList from './routes/CharacterList/CharacterList';
import CharacterSheet from './routes/CharacterSheet/CharacterSheet';
import LoadingSpinner from './shared/LoadingSpinner/LoadingSpinner';
import './App.scss';

const App = () => {
  const dispatch = useAppDispatch();
  const userReady = useAppSelector(selectUserReady);
  const userEmail = useAppSelector(selectUserEmail);
  const masterdataLoaded = useAppSelector(selectMasterdataLoaded);

  useEffect(() => {
    dispatch(initialThunk());
  }, [dispatch]);

  useEffect(() => {
    // only perform this action once we are logged in
    if (!masterdataLoaded && userEmail && userEmail !== '') {
      dispatch(masterdataThunk());
    }
  }, [dispatch, userEmail, masterdataLoaded]);

  return userReady ? (
    <div className="app">
      <TooltipProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Layout />}>
              <Route
                index
                element={
                  <RequireAuth>
                    <CharacterList />
                  </RequireAuth>
                }
              />
              <Route
                path="/character/:id"
                element={
                  <RequireAuth>
                    <CharacterSheet />
                  </RequireAuth>
                }
              />
              <Route path="/login" element={<Login />} />
              <Route path="/forgot-password" element={<ForgotPassword />} />
              <Route path="/register" element={<Register />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </TooltipProvider>
    </div>
  ) : (
    <div className="layout">
      <div style={{ width: '100%', height: '100%' }}>
        <LoadingSpinner />
      </div>
    </div>
  );
};
export default App;
