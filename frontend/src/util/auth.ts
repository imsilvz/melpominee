/* eslint-disable import/prefer-default-export */
export const validatePassword = (password: string) => {
  // at least 8 characters long!
  if (!password || password.length < 8) {
    return false;
  }

  // let validRegex = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[-+_!@#$%^&*.,?])/;

  return true;
};
