import React from 'react';
import { Outlet } from 'react-router-dom';

// redux
import { useAppSelector } from '../../redux/hooks';
import { selectUserEmail } from '../../redux/reducers/userReducer';

// local files
import ConnectionStatus from './ConnectionStatus/ConnectionStatus';
import './Layout.scss';

const Layout = () => {
  const userEmail = useAppSelector(selectUserEmail);
  return (
    <div className="layout">
      {(!userEmail || userEmail === '') && <div />}
      <ConnectionStatus />
      <Outlet />
    </div>
  );
};
export default Layout;
