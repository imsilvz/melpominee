import React from 'react';
import { Outlet } from 'react-router-dom';

// redux
import { useAppSelector } from '../../redux/hooks';
import { selectUserEmail } from '../../redux/reducers/userReducer';

// local files
import './Layout.scss';

const Layout = () => {
  const userEmail = useAppSelector(selectUserEmail);
  return (
    <div className="layout">
      {(!userEmail || userEmail === '') && <div />}
      <Outlet />
    </div>
  );
};
export default Layout;
