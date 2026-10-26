import { createBrowserRouter } from 'react-router-dom';
import LoginPage from '../pages/LoginPage';
import ListingCasesPage from '../pages/ListingCasesPage';
import NotFoundPage from '../pages/NotFoundPage';
import RegisterPage from '../pages/RegisterPage';
import ProtectedRoute from '../components/ProtectedRoute';
import UnauthorizedPage from '../pages/UnauthorizedPage';
import GuestRoute from '../components/GuestRoute';
import MainLayout from '../layouts/MainLayout';
import UserManagementPage from '../pages/UserManagementPage';

export const router = createBrowserRouter([
  {
      path: '/',
      element: (
          <ProtectedRoute>
              <MainLayout />
          </ProtectedRoute>
      ),
      children: [
        { index: true, element: <ListingCasesPage /> },
        {
            path: 'users',
            element: (
                <ProtectedRoute allowedRoles={['PhotographyCompany']}>
                    <UserManagementPage />
                </ProtectedRoute>
            ),
        },
    ],
  },
  { path: '/login', element: 
    <GuestRoute>
      <LoginPage />
    </GuestRoute>},
  { path: '*', element: <NotFoundPage /> },
  { path: '/register', element: (
    <GuestRoute>
      <RegisterPage />
    </GuestRoute>
  )},
  { path: '/unauthorized', element: <UnauthorizedPage/>}
]);