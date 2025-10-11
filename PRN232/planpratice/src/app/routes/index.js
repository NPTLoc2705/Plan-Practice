import { AuthRoutes, PublicRoute, ProtectedRoute } from './AuthRoutes';
import { StudentRoutes, StudentProtectedRoute } from './StudentRoutes';
import { TeacherRoutes, TeacherProtectedRoute } from './TeacherRoutes';
import { AdminRoutes, AdminRoute } from './AdminRoutes';

import Landing from '../pages/Landing';

export {
    // Pages
    Landing,

    // Route Components
    AuthRoutes,
    StudentRoutes,
    TeacherRoutes,
    AdminRoutes,
    
    // Route Protection Components
    PublicRoute,
    ProtectedRoute,
    StudentProtectedRoute,
    TeacherProtectedRoute,
    AdminRoute,
};