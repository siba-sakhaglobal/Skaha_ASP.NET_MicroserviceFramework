import axios from 'axios';

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});

// Request interceptor: attach auth token + tenant ID
api.interceptors.request.use((config) => {
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('token');
    const tenantId = localStorage.getItem('tenantId');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    if (tenantId) config.headers['X-Tenant-Id'] = tenantId;
  }
  return config;
});

// Response interceptor: handle 401 (token expired)
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token expired — redirect to login
      if (typeof window !== 'undefined') {
        localStorage.removeItem('token');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default api;
