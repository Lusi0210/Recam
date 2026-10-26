import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth"
import type { ReactNode } from "react";
import type { RoleName } from "../types/auth";

interface ProtectedRouteProps {
    children: ReactNode;
    allowedRoles?: RoleName[];
}


const ProtectedRoute = ({ children,allowedRoles }: ProtectedRouteProps) => {
    const authContext = useAuth();
    if(authContext.user === null){
        return <Navigate to="/login" replace/>;
    }
    
    if (allowedRoles && !allowedRoles.includes(authContext.user.role)) {
        return <Navigate to="/unauthorized" replace />;
    }


    return children;
}

export default ProtectedRoute;