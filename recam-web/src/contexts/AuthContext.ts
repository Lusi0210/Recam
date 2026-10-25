import { createContext } from "react";
import type { AuthUser, LoginRequest } from "../types/auth";

export interface AuthContextType{
    user: AuthUser | null;
    login:(request: LoginRequest) => Promise<void>;
    logout:() => void;
}

export const AuthContext = createContext<AuthContextType| null>(null); 

