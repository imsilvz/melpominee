import React, { useState } from 'react';
import { Routes, Route } from 'react-router-dom';

import Layout from './shared/Layout';
import Login from './routes/Login/Login';
import './App.scss';

const App = () => {
  return (
    <div className="app">
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Login />} />
        </Route>
      </Routes>
    </div>
  );
};
export default App;
