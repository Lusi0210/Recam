import { createBrowserRouter } from 'react-router-dom';
import LoginPage from '../pages/LoginPage';
import ListingCasesPage from '../pages/ListingCasesPage';
import NotFoundPage from '../pages/NotFoundPage';
import RegisterPage from '../pages/RegisterPage';

export const router = createBrowserRouter([
  { path: '/', element: <ListingCasesPage /> },
  { path: '/login', element: <LoginPage /> },
  { path: '*', element: <NotFoundPage /> },
  { path: '/register', element: <RegisterPage /> },
]);