/* eslint-disable import/prefer-default-export */
export const validatePassword = (password: string) => {
  return !!password && password.length >= 12;
};
