import type { ReactNode } from "react";
import { useAuth } from "../hooks/useAuth";
import { Navigate } from "react-router-dom";

interface GuestRouteProps{
    children:ReactNode;
}

const GuestRoute = ({children}:GuestRouteProps) =>{
    const authContext = useAuth();
    if(authContext.user !== null){
        return <Navigate to="/" replace/>
    }

    return  children;
}

export default GuestRoute;