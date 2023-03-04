import React, { useState } from 'react';
import './Login.scss';

const Login = () => {
  return (
    <div className="login-container">
      <div className="login-panel">
        <h1>Login Here</h1>
        <div className="input-item">
          <input type="email" name="email" placeholder="Email" />
        </div>
        <div className="input-item">
          <input type="password" name="password" placeholder="Password" />
        </div>
        <div className="input-item">
          <p>Remember Me</p>
        </div>
        <div className="input-item">
          <button type="button">Login</button>
        </div>
        <div className="input-item">
          <p>Forgot Password</p>
        </div>
      </div>
    </div>
  );
};
export default Login;
