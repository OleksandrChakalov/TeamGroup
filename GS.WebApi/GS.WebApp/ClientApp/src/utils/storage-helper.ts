import { ILogInResponse } from "../models/ILogInResponse";


const WriteToken = (response: ILogInResponse) => {
  localStorage.setItem('auth', JSON.stringify({
    id: response.id,
    email: response.email,
    token: response.token,
  }))
};

const GetAuthData = (): (ILogInResponse | undefined) => {
  const authInfo = localStorage.getItem('auth');
  return authInfo ? JSON.parse(authInfo) as ILogInResponse : undefined;
};

const CleanToken = () => {
  localStorage.removeItem('auth');
};

export { WriteToken, GetAuthData, CleanToken };