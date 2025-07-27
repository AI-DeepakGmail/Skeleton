export const API_BASE_URL = 'https://localhost:7081/api';

export const API_ENDPOINTS = {
  auth: {
    login: `${API_BASE_URL}/auth/login`,
    refresh: `${API_BASE_URL}/auth/refresh`
  },
  weather: {
    forecast: `${API_BASE_URL}/weatherforecast`
  },
 /* user: {
    profile: `${API_BASE_URL}/user/profile`,
    roles: `${API_BASE_URL}/user/roles`
  }*/
  // Add more domains here
};
