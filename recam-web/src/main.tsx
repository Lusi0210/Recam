import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import { router } from './routes/router';
import './index.css';
import AuthProvider from './contexts/AuthProvider';
import ToastProvider from './contexts/ToastProvider';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider>
        <ToastProvider>
            <RouterProvider router={router} />
        </ToastProvider>
    </AuthProvider>
  </StrictMode>
);