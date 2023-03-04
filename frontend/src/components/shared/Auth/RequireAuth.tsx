import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

// redux
import { useAppSelector } from '../../../redux/hooks';
import { selectUserEmail } from '../../../redux/reducers/userReducer';

const RequireAuth = ({ children }: { children: JSX.Element }) => {
  const location = useLocation();
  const userEmail = useAppSelector(selectUserEmail);
  if (!userEmail || userEmail === '') {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
  return children;
};
export default RequireAuth;
