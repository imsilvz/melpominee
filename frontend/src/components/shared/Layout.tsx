import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import './Layout.scss';

const Layout = () => {
  return (
    <div className="layout">
      <Outlet />
    </div>
  );
};
export default Layout;
