import React, { useEffect } from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

// redux
import { useAppDispatch, useAppSelector } from '../redux/hooks';
import initialThunk from '../redux/thunk/initial';
import { selectUserReady } from '../redux/reducers/userReducer';

// local files
import Layout from './shared/Layout';
import Login from './routes/Login/Login';
import RequireAuth from './shared/Auth/RequireAuth';
import LoadingSpinner from './shared/LoadingSpinner/LoadingSpinner';
import GameList from './routes/Games/GameList/GameList';
import './App.scss';

const App = () => {
  const dispatch = useAppDispatch();
  const userReady = useAppSelector(selectUserReady);

  useEffect(() => {
    dispatch(initialThunk());
  }, [dispatch]);

  return userReady ? (
    <div className="app">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route
              index
              element={
                <RequireAuth>
                  <GameList />
                </RequireAuth>
              }
            />
            <Route path="/login" element={<Login />} />
          </Route>
        </Routes>
      </BrowserRouter>
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
